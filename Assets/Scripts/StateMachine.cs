using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class StateMachine : MonoBehaviour
{
    enum State { Idle, Wander, Graze, Alert, Flee, ReturnHome }

    [Header("Scene References")]
    public GameObject player;
    public Transform[] wanderWaypoints;
    public TextMeshProUGUI stateText;

    [Header("Config")]
    public float speedNormal;
    public float speedFlee;

    [Header("Vision")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 340f;
    bool canSeePlayer = false;

    State state;

    #region Unity Functions
    private void Start()
    {
        // Always start in the idle state
        state = State.Idle;
    }

    void Update()
    {
        // States
        switch (state)
        {
            case State.Idle:
                break;

            case State.Wander:
                break;

            case State.Graze:
                break;

            case State.Alert:
                break;

            case State.Flee:
                break;

            case State.ReturnHome:
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
    }
    void Idle()
    {
        
    }

    void EnterWander()
    {
        state = State.Wander;
    }
    void Wander()
    {
        
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
    }

    void Flee()
    {
        
    }

    void EnterReturnHome()
    {
        state = State.ReturnHome;
    }

    void ReturnHome()
    {
        
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
        // Draw the view cone of the NPC
        //if (state != State.Idle)
        //{
            Handles.color = new Color(0f, 1f, 1f, 0.25f);
            if (canSeePlayer) Handles.color = new Color(1f, 0f, 0f, 0.25f);

            Vector3 forward = transform.forward;
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, viewAngle / 2f, viewRadius);
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, -viewAngle / 2f, viewRadius);
        //}
    }
    #endregion
}
