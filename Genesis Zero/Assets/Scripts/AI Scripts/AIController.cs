using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * AIControl is the base class controlling the behavior states of enemies and how they interact with the player.
 */
public class AIController : Pawn
{
    public AIPropertyObject BehaviorProperties;

    public enum AIState { Idle, Patrol, Follow, Charge, Attack, Cooldown }
    protected AIState state = AIState.Patrol; // Current behavior state

    protected Transform tr; // Reference to own transform to avoid internal GetComponent call
    public Transform Target; // Target or player object to follow and attack

    protected float stateTime = 0.0f; // Duration of current state

    protected virtual void Awake()
    {
        tr = transform;
    }

    public void Update()
    {
        base.Update();

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
            if (GetDistanceToTarget() <= BehaviorProperties.DetectRadius)
            {
                ChangeState(AIState.Follow);
            }
        }
        else if (state == AIState.Follow) // State when following player to attack
        {
            if (BehaviorProperties.StopFollowingWhenOutOfRange && GetDistanceToTarget() > BehaviorProperties.DetectRadius)
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

        stateTime += Time.deltaTime;
    }

    /**
     * This is used to change the current state of the AI
     */
    public void ChangeState(AIState newState)
    {
        state = newState;
        stateTime = 0.0f;
    }

    /**
     * Returns the distance to the target/player
     */
    public float GetDistanceToTarget()
    {
        if (tr != null && Target != null)
        {
            return Vector3.Distance(tr.position, Target.position);
        }
        return 0.0f;
    }

    /**
     * Returns the distance that the target has entered into the avoid radius
     */
    public float GetAvoidCloseness()
    {
        if (tr != null && Target != null && BehaviorProperties != null)
        {
            return Mathf.Max(0.0f, BehaviorProperties.AvoidRadius - GetDistanceToTarget());
        }
        return 0.0f;
    }

    /**
     * Draw visual representations of properties
     */
    public void OnDrawGizmos()
    {
        if (BehaviorProperties != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, BehaviorProperties.DetectRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, BehaviorProperties.AvoidRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, BehaviorProperties.AttackRadius);
        }
    }
}
