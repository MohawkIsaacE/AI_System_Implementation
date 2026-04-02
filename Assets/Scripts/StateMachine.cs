using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [Range(0, 360)] public float viewAngle = 290f;

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
    
    #endregion
}
