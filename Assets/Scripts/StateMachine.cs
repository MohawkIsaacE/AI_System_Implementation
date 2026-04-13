using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class StateMachine : MonoBehaviour
{
    enum State { Idle, Wander, Graze, Alert, Flee, ReturnHome }

    [Header("Scene References")]
    public GameObject player;
    public Transform wanderWaypointsParent;
    private Transform[] wanderWaypoints;
    public Transform homeWaypoint;
    public TextMeshProUGUI stateText;

    [Header("Config")]
    public float speedNormal = 4f;
    public float speedFlee = 10f;

    [Header("Idle")]
    public float idleTimeElapsed = 0f;
    public float idleTimeThreshold = 3f;

    [Header("Wander")]
    public int chosenWanderWaypoint;
    public float wanderDistanceThreshold = 1f;
    public int grazeChance = 45;
    public int goHomeChance = 90;

    [Header("Graze")]
    public float grazeTimeElapsed = 0f;
    public float grazeTimeThreshold = 3f;
    public float grazeViewAngle = 280f;

    [Header("Alert")]
    public float alertTimeElapsed = 0f;
    public float alertTimeThreshold = 4f;
    public float continuousAlertTimer = 0f;
    public Vector3 playerPositionOnAlert;
    Quaternion lookAtAlert;

    [Header("Flee")]
    public float fleeTimeElapsed = 0f;
    public float fleeTimeThreshold = 1f;
    private Vector3 positionBehindNPC;

    [Header("Home")]
    public float homeDistanceThreshold = 1f;

    [Header("Vision")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 340f;
    bool canSeePlayer = false;

    [Header("Sound")]
    bool heardSound;
    int heardSoundAmount;
    Vector3 soundPosition;

    // NPC
    State state;
    NavMeshAgent agent;

    #region Unity Functions
    private void Awake()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        // Set the wander waypoints based on the parent group
        int waypointIndex = 0;
        wanderWaypoints = new Transform[wanderWaypointsParent.childCount];
        foreach (Transform waypoint in wanderWaypointsParent)
        {
            wanderWaypoints[waypointIndex] = waypoint;
            waypointIndex++;
        }
    }

    private void Start()
    {
        // Always start in the idle state
        EnterIdle();
    }

    private void Update()
    {
        // Debug
        stateText.text = $"NPC State: {state}";
        canSeePlayer = IsInView();
        // States
        switch (state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Wander:
                Wander();
                break;

            case State.Graze:
                Graze();
                break;

            case State.Alert:
                Alert();
                break;

            case State.Flee:
                Flee();
                break;

            case State.ReturnHome:
                ReturnHome();
                break;

            default:
                Debug.Log("Error [StateMachine]: Missing state");
                break;
        }
    }
    #endregion

    #region State Machine
    void EnterIdle()
    {
        state = State.Idle;
        agent.speed = speedNormal;
        idleTimeElapsed = 0f;
    }

    void Idle()
    {
        // Idle for a set amount of time
        idleTimeElapsed += Time.deltaTime;

        if (idleTimeElapsed >= idleTimeThreshold)
        {
            EnterWander();
        }

        if (IsInView() || heardSound)
        {
            EnterAlert();
        }
    }

    void EnterWander()
    {
        state = State.Wander;
        // Choose a random position to walk towards
        chosenWanderWaypoint = Random.Range(0, wanderWaypoints.Length - 1);
        agent.SetDestination(wanderWaypoints[chosenWanderWaypoint].transform.position);
        heardSoundAmount = 0;
    }

    void Wander()
    {
        // Check how close the agent is to the chosen waypoint
        bool isCloseToWaypoint = Vector3.Distance(transform.position, wanderWaypoints[chosenWanderWaypoint].position) < wanderDistanceThreshold;

        if (isCloseToWaypoint)
        {
            int stateChance = Random.Range(0, 101);

            if (stateChance >= goHomeChance) EnterReturnHome();
            else if (stateChance >= grazeChance) EnterGraze();
            else EnterIdle();
        }

        if (IsInView() || heardSound)
        {
            EnterAlert();
        }
    }

    void EnterGraze()
    {
        state = State.Graze;
        grazeTimeElapsed = 0f;
    }

    void Graze()
    {
        // Graze for a set amount of time
        grazeTimeElapsed += Time.deltaTime;

        if (grazeTimeElapsed >= grazeTimeThreshold)
        {
            EnterWander();
        }

        if (IsInView() || heardSound)
        {
            EnterAlert();
        }
    }

    void EnterAlert()
    {
        state = State.Alert;
        alertTimeElapsed = 0f;
        continuousAlertTimer = 0f;
        // Halt agent movement
        agent.SetDestination(agent.transform.position);
        playerPositionOnAlert = player.transform.position;
    }

    void Alert()
    {
        // Calculate how far the player has moved since entering alert
        float playerMoveDistance = (player.transform.position - playerPositionOnAlert).magnitude;

        // Constantly look at the player if they are in view and has moved far enough
        if (IsInView() && playerMoveDistance >= 1f)
        {
            Vector3 dirToAlert = (player.transform.position - transform.position).normalized;
            lookAtAlert = Quaternion.LookRotation(dirToAlert);
            // Decrease the alert timer
            alertTimeElapsed = 0;
            continuousAlertTimer += Time.deltaTime;
        }
        // Look at the sound that triggered the alert state
        else if (heardSound)
        {
            heardSound = false;
            heardSoundAmount += 1;
            lookAtAlert = Quaternion.LookRotation(soundPosition);
        }
        else
        {
            alertTimeElapsed += Time.deltaTime;
        }

        // Look at the position of the thing that alerted the npc
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtAlert, 2 * Time.deltaTime);

        // Be alert for a set amount of time
        if (alertTimeElapsed > alertTimeThreshold)
        {
            EnterWander();
        }
        else if (continuousAlertTimer >= 0.5f || heardSoundAmount >= 2)
        {
            // Flee if the player continues to alert the npc
            EnterFlee();
        }
    }

    void EnterFlee()
    {
        state = State.Flee;
        agent.speed = speedFlee;
        fleeTimeElapsed = 0f;
        heardSoundAmount = 0;
        positionBehindNPC = -agent.transform.forward * 2;
    }

    void Flee()
    {
        // Should look like it is running away from the alert
        //agent.SetDestination(positionBehindNPC);

        // Stay away from player if they are in the view area
        //if (IsInView())
        //{
        //    fleeTimeElapsed = 0f;
        //}
        //else if (fleeTimeElapsed >= fleeTimeThreshold)
        //{
        //    EnterReturnHome();
        //}
        EnterReturnHome();

        //fleeTimeElapsed += Time.deltaTime;
    }

    void EnterReturnHome()
    {
        state = State.ReturnHome;
        agent.SetDestination(homeWaypoint.position);
    }

    void ReturnHome()
    {
        // Check how close the agent is to the home waypoint
        bool isCloseToHome = Vector3.Distance(transform.position, homeWaypoint.position) < homeDistanceThreshold;

        if (isCloseToHome)
        {
            EnterIdle();
        }

        // Flee if the player is in the view area
        //if (IsInView())
        //{
        //    EnterFlee();
        //}
    }
    #endregion

    #region Support Methods
    bool IsInView()
    {
        // Adujust the raycast position so that it doesn't hit the ground immediately
        Vector3 npcPosition = transform.position;
        npcPosition.y = 1f;

        Vector3 playerPosition = player.transform.position;
        playerPosition.y = 1f;

        Vector3 toPlayer = playerPosition - npcPosition;
        float distToPlayer = toPlayer.magnitude;

        // 1. Distance check
        if (distToPlayer > viewRadius) return false;

        // 2. Angle check
        Vector3 dirToPlayer = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        // 3. Check if player is moving
        //if (player.GetComponent<CharacterController>().velocity.magnitude <= 0) return false;

        // Determine which view angle to use based on state
        float currentViewAngle = state == State.Graze ? grazeViewAngle : viewAngle;
        if (angle > currentViewAngle * 0.5f) return false;

        // 4. Raycast
        if (Physics.Raycast(npcPosition, dirToPlayer, out RaycastHit hit, viewRadius))
        {
            return hit.transform == player.transform;
        }
        return false;
    }

    public void SoundTrigger(SoundObject sound)
    {
        heardSound = true;
        soundPosition = sound.transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Debug Methods
    private void OnDrawGizmos()
    {
        // Draw the wander waypoints
        Gizmos.color = Color.red;
        foreach (Transform wanderTransform in wanderWaypointsParent)
        {
            Gizmos.DrawWireSphere(wanderTransform.position, 0.5f);
        }

        // Draw the home waypoint
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(homeWaypoint.position, 0.5f);

        // Draw the view cone of the NPC
        Handles.color = new Color(0f, 1f, 1f, 0.25f);
        if (canSeePlayer) Handles.color = new Color(1f, 0f, 0f, 0.25f);

        Vector3 forward = transform.forward;
        // Determine which view angle to use based on state
        float currentViewAngle = state == State.Graze ? grazeViewAngle : viewAngle;
        Handles.DrawSolidArc(transform.position, Vector3.up, forward, currentViewAngle / 2f, viewRadius);
        Handles.DrawSolidArc(transform.position, Vector3.up, forward, -currentViewAngle / 2f, viewRadius);
    }
    #endregion
}
