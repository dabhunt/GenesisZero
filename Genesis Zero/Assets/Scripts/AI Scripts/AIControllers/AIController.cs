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

    protected float stateTime = 0.0f; // Duration of current state

    public AIStateEvent StateChangeEvent; // Invoked whenever the state is changed and passes in the new state to called methods

    new protected void Start()
    {
        base.Start();
        if (Target == null)
        {
            GameObject playerSearch = GameObject.FindGameObjectWithTag("Player");
            if (playerSearch != null)
            {
                Target = playerSearch.transform;
            }
        }
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();

        if (BehaviorProperties == null)
        {
            Debug.LogError("No AI Properties assigned to " + transform.name, gameObject);
            return;
        }

        if (IsStunned())
        {
            ChangeState(AIState.Patrol);
        }
        else if (Target != null)
        {
            StateUpdate();
        }
        else
        {
            ChangeState(AIState.Idle);
        }

        //Debug.Log(state);
    }

    /**
     * State logic meant to be called in Update
     */
    private void StateUpdate()
    {
        if (state == AIState.Patrol) // State when moving around while not following player
        {
            if (GetDistanceToTarget() <= BehaviorProperties.DetectRadius && TargetVisible())
            {
                ChangeState(AIState.Follow);
            }
        }
        else if (state == AIState.Follow) // State when following player to attack
        {
            if (BehaviorProperties.StopFollowingWhenOutOfRange && (GetDistanceToTarget() > BehaviorProperties.DetectRadius || !TargetVisible()))
            {
                ChangeState(AIState.Patrol);
            }
            else if (GetDistanceToTarget() <= BehaviorProperties.AttackRadius)
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
        state = newState;
        stateTime = 0.0f;
        StateChangeEvent.Invoke(state);
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
            return Mathf.Max(0.0f, BehaviorProperties.AvoidRadius - GetDistanceToTarget());
        }
        return 0.0f;
    }

    protected bool TargetVisible()
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
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }

    /**
     * Draw visual representations of properties
     */
    protected void OnDrawGizmos()
    {
        if (TargetVisible())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Target.position);
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
}

// Class for events where an AI state is passed as an argument
[System.Serializable]
public class AIStateEvent : UnityEvent<AIController.AIState> { }