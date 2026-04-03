using System.Collections;
using System.Collections.Generic;
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
    public float speedFlee = 7f;

    [Header("Idle")]
    public float idleTimeElapsed = 0f;
    public float idleTimeThreshold = 5f;

    [Header("Wander")]
    public int chosenWanderWaypoint;
    public float wanderDistanceThreshold = 1f;
    public int grazeChance = 45;
    public int goHomeChance = 90;

    [Header("Home")]
    public float homeDistanceThreshold = 1f;

    [Header("Vision")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 340f;
    bool canSeePlayer = false;

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
        idleTimeElapsed += Time.deltaTime;

        if (idleTimeElapsed >= idleTimeThreshold)
        {
            EnterWander();
        }
    }

    void EnterWander()
    {
        state = State.Wander;
        // Choose a random position to walk towards
        chosenWanderWaypoint = Random.Range(0, wanderWaypoints.Length - 1);
        agent.SetDestination(wanderWaypoints[chosenWanderWaypoint].transform.position);
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
    }

    void EnterGraze()
    {
        state = State.Graze;
    }

    void Graze()
    {
        
    }

    void EnterAlert()
    {
        state = State.Alert;
    }

    void Alert()
    {

    }

    void EnterFlee()
    {
        state = State.Flee;
        agent.speed = speedFlee;
    }

    void Flee()
    {
        
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
    }
    #endregion

    #region Support Methods
    bool IsInView()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        float distToPlayer = toPlayer.magnitude;

        // 1. Distance check
        if (distToPlayer > viewRadius) return false;

        // 2. Angle check
        Vector3 dirToPlayer = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > viewAngle * 0.5f) return false;

        // 3. Raycast
        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, viewRadius))
        {
            return hit.transform == player.transform;
        }
        return false;
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
        Handles.DrawSolidArc(transform.position, Vector3.up, forward, viewAngle / 2f, viewRadius);
        Handles.DrawSolidArc(transform.position, Vector3.up, forward, -viewAngle / 2f, viewRadius);
    }
    #endregion
}
