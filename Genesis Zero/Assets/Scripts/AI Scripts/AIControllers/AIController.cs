using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Justin Couch
 * AIControl is the base class controlling the behavior states of enemies and how they interact with the player.
 */
public class AIController : Pawn
{
    public AIPropertyObject BehaviorProperties;

    public enum AIState { Idle, Patrol, Follow, Charge, Attack, Cooldown }
    protected AIState state = AIState.Patrol; // Current behavior state

    public Transform Target; // Target or player object to follow and attack
    protected Vector3 targetPosition = Vector3.zero; // Position to move to
    protected Vector3 alertPoint = Vector3.zero; // Position to move to when alerted
    protected bool alertTracking = false; // Whether the target following type is alert-based (moving to alert point)
    private float alertTrackTime = 0.0f;
    protected bool targetVisible = false;

    protected float stateTime = 0.0f; // Duration of current state

    public AIStateEvent StateChangeEvent; // Invoked whenever the state is changed and passes in the new state to called methods

    protected ObjectTracker tracker;

    new protected void Start()
    {
        base.Start();

        if (BehaviorProperties == null)
        {
            Debug.LogError("No AI Properties assigned to " + transform.name, gameObject);
            //return;
        }

        tracker = GetComponent<ObjectTracker>();
        if (Target == null)
        {
            GameObject playerSearch = GameObject.FindGameObjectWithTag("Player");
            if (playerSearch != null)
            {
                SetTarget(playerSearch.transform);
            }
        }
    }

    protected virtual void SetTarget(Transform tr)
    {
        Target = tr;
        if (tracker != null)
        {
            tracker.Target = Target;
        }
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();

        if (Target != null)
        {
            CheckTargetVisibility();
        }
        targetPosition = GetTargetFollowPoint();

        if (IsStunned())
        {
            ChangeState(AIState.Idle);
        }
        else if (Target != null)
        {
            StateUpdate();
        }
        else
        {
            ChangeState(AIState.Idle);
        }

        if (tracker != null)
        {
            if (state == AIState.Patrol || state == AIState.Idle || targetVisible)
            {
                tracker.StopTracking();
                tracker.Reset();
            }
            else if (!alertTracking)
            {
                tracker.StartTracking();
                if (tracker.HasReachedEnd() || tracker.GiveUpCondition())
                {
                    ChangeState(AIState.Patrol);
                }
            }
        }

        if (alertTracking)
        {
            alertTrackTime += Time.fixedDeltaTime;
            if (targetVisible)
            {
                alertTracking = false;
            }

            if (alertTrackTime >= BehaviorProperties.MaxAlertTrackTime)
            {
                alertTracking = false;
                ChangeState(AIState.Patrol);
            }
        }
        else
        {
            alertTrackTime = 0.0f;
        }

        //Debug.Log(state);
    }

    /**
     * Returns the current state of the AI
     */
    public AIState GetState()
    {
        return state;
    }

    /**
     * State logic meant to be called in FixedUpdate
     */
    private void StateUpdate()
    {
        if (state == AIState.Idle)
        {
            ChangeState(AIState.Patrol);
        }

        if (state == AIState.Patrol) // State when moving around while not following player
        {
            if ((GetDistanceToTarget() <= BehaviorProperties.DetectRadius && targetVisible) || alertTracking)
            {
                ChangeState(AIState.Follow);
            }
        }
        else if (state == AIState.Follow) // State when following player to attack
        {
            if (BehaviorProperties.StopFollowingWhenOutOfRange && (GetDistanceToTarget() > BehaviorProperties.DetectRadius || !targetVisible))
            {
                ChangeState(AIState.Patrol);
            }
            else if (GetDistanceToTarget() <= BehaviorProperties.AttackRadius && targetVisible)
            {
                ChangeState(AIState.Charge);
            }
        }
        else if (state == AIState.Charge) // State when charging up an attack
        {
            if (stateTime >= BehaviorProperties.AttackChargeTime)
            {
                ChangeState(AIState.Attack);
            }
        }
        else if (state == AIState.Attack) // State when performing actual attack
        {
            if (stateTime >= BehaviorProperties.AttackDuration)
            {
                ChangeState(AIState.Cooldown);
            }
        }
        else if (state == AIState.Cooldown) // State when cooling down after attack
        {
            if (stateTime >= BehaviorProperties.AttackCooldownTime)
            {
                ChangeState(AIState.Follow);
            }
        }

        stateTime += Time.fixedDeltaTime;
    }

    /**
     * This is used to change the current state of the AI
     */
    public void ChangeState(AIState newState)
    {
        bool stateDifferent = state != newState;
        state = newState;
        if (stateDifferent)
        {
            stateTime = 0.0f;
            StateChangeEvent.Invoke(state);
        }

        if (tracker != null)
        {
            if (newState != AIState.Patrol && newState != AIState.Idle)
            {
                //tracker.StartTracking();
            }
            else
            {
                //tracker.StopTracking();
                tracker.Reset();
            }
        }

        if (newState == AIState.Patrol || newState == AIState.Charge)
        {
            alertTracking = false;
        }
    }

    /**
     * Returns the distance to the target/player
     */
    public float GetDistanceToTarget()
    {
        if (Target != null)
        {
            return Vector3.Distance(transform.position, Target.position);
        }
        return 0.0f;
    }

    /**
     * Returns the distance that the target has entered into the avoid radius
     */
    public float GetAvoidCloseness()
    {
        if (Target != null && BehaviorProperties != null)
        {
            return targetVisible ? Mathf.Max(0.0f, BehaviorProperties.AvoidRadius - GetDistanceToTarget()) : 0.0f;
        }
        return 0.0f;
    }

    /**
     * Returns whether the target is visible
     */
    private void CheckTargetVisibility()
    {
        if (Target != null && BehaviorProperties != null)
        {
            if (BehaviorProperties.UseLineOfSight)
            {
                Vector3 toTarget = Target.position - transform.position;
                RaycastHit[] sightHits = new RaycastHit[BehaviorProperties.MaxSightCastHits];
                if (Physics.RaycastNonAlloc(transform.position, toTarget.normalized, sightHits, toTarget.magnitude, BehaviorProperties.SightMask, QueryTriggerInteraction.Ignore) > 0)
                {
                    for (int i = 0; i < sightHits.Length; i++)
                    {
                        RaycastHit curHit = sightHits[i];
                        if (curHit.collider != null)
                        {
                            //Debug.Log(curHit.transform.name);
                            if (!curHit.transform.IsChildOf(transform) && !curHit.transform.IsChildOf(Target))
                            {
                                targetVisible = false;
                                return;
                            }
                        }
                    }
                }
            }
            targetVisible = true;
            return;
        }
        targetVisible = false;
    }

    /**
     * Calculates the best point to go in order to follow the target
     */
    protected Vector3 GetTargetFollowPoint()
    {
        if (Target == null) { return Vector3.zero; }

        if (targetVisible)
        {
            return Target.position;
        }

        if (alertTracking)
        {
            return alertPoint;
        }
        else if (tracker != null)
        {
            alertTracking = false;
            return tracker.PeekFirstPoint();
        }
        return Vector3.zero;
    }

    /**
     * Draw visual representations of properties
     */
    protected void OnDrawGizmos()
    {
        if (targetVisible)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Target.position);
        }

        if (alertTracking)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, alertPoint);
        }

        if (BehaviorProperties != null)
        {
            Gizmos.color = Color.cyan;
            GizmosExtra.DrawWireCircle(transform.position, Vector3.forward, BehaviorProperties.DetectRadius);
            Gizmos.color = Color.yellow;
            GizmosExtra.DrawWireCircle(transform.position, Vector3.forward, BehaviorProperties.AvoidRadius);
            Gizmos.color = Color.red;
            GizmosExtra.DrawWireCircle(transform.position, Vector3.forward, BehaviorProperties.AttackRadius);
        }
    }

    public override float TakeDamage(float amount, Pawn source)
    {
        Debug.Log("Enemy Damaged");
        if (source && (state == AIState.Patrol || state == AIState.Idle || alertTracking))
        {
            alertPoint = source.transform.position;
            alertTracking = true;
        }
        return base.TakeDamage(amount, source);
    }
}

// Class for events where an AI state is passed as an argument
[System.Serializable]
public class AIStateEvent : UnityEvent<AIController.AIState> { }