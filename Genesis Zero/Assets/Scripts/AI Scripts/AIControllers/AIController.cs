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
    protected AIState state = AIState.Idle; // Current behavior state
    protected bool initialized = false;

    public float IdlePatrolIntervalMin = 1.0f; // Minimum time interval for switching between idling and patrolling
    public float IdlePatrolIntervalMax = 2.0f; // Maximum time interval for switching between idling and patrolling
    private float idlePatrolIntervalCurrent = 1.0f; // Randomly chosen interval in range
    protected bool isGrounded = false;
    protected Vector3 groundNormal = Vector3.zero;
    public Vector3 Origin;
    protected Vector3 trueOrigin = Vector3.zero;

    public Transform Target; // Target or player object to follow and attack
    protected Vector3 targetPosition = Vector3.zero; // Position to move to
    protected Vector3 alertPoint = Vector3.zero; // Position to move to when alerted
    protected bool alertTracking = false; // Whether the target following type is alert-based (moving to alert point)
    private float alertTrackTime = 0.0f;
    protected bool targetVisible = false;

    protected float stateTime = 0.0f; // Duration of current state
    private bool stunnedLastFrame = false;
    public AIStateEvent StateChangeEvent; // Invoked whenever the state is changed and passes in the new state to called methods

    protected ObjectTracker tracker;

    public DifficultyMultiplier ChargeTimeDifficultyMultiplier;

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

        StartCoroutine(DelayedStart());
    }

    /**
     * This is for initialization tasks that need to happen after other scripts have called their Start() functions
     */
    protected IEnumerator DelayedStart()
    {
        yield return new WaitForFixedUpdate();
        EnemyManager.AllEnemies.Add(this);
        initialized = true;
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
        if (!initialized) { return; }

        UpdateOrigin();
        GroundCheck();

        if (Target != null)
        {
            CheckTargetVisibility();
        }
        targetPosition = GetTargetFollowPoint();
        //if AI was stunned last frame, but not this frame, change state to AIState.Follow
        if (stunnedLastFrame == true && !IsStunned())
        {
            ChangeState(AIState.Follow);
            stunnedLastFrame = false;
        }
        if (IsStunned() || IsDying())
        {
            ChangeState(AIState.Idle);
            stunnedLastFrame = true;
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
                if ((tracker.HasReachedEnd() || tracker.GiveUpCondition()) && state != AIState.Charge && state != AIState.Attack && state != AIState.Cooldown)
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

        //Debug.Log(GetNearbyEnemies().Length);
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
            if ((GetDistanceToTarget() <= BehaviorProperties.DetectRadius && targetVisible) || alertTracking)
            {
                ChangeState(AIState.Follow);
                if (!alertTracking)
                {
                    AlertNearbyEnemies(Target);
                }
            }

            if (stateTime > idlePatrolIntervalCurrent)
            {
                idlePatrolIntervalCurrent = Random.Range(IdlePatrolIntervalMin, IdlePatrolIntervalMax);
                ChangeState(AIState.Patrol);
            }
        }
        else if (state == AIState.Patrol) // State when moving around while not following player
        {
            if ((GetDistanceToTarget() <= BehaviorProperties.DetectRadius && targetVisible) || alertTracking)
            {
                idlePatrolIntervalCurrent = Random.Range(IdlePatrolIntervalMin, IdlePatrolIntervalMax);
                ChangeState(AIState.Follow);
                if (!alertTracking)
                {
                    AlertNearbyEnemies(Target);
                }
            }

            if (stateTime > idlePatrolIntervalCurrent)
            {
                ChangeState(AIState.Idle);
            }
        }
        else if (state == AIState.Follow) // State when following player to attack
        {
            if (BehaviorProperties.StopFollowingWhenOutOfRange && (GetDistanceToTarget() > BehaviorProperties.DetectRadius || !targetVisible))
            {
                ChangeState(AIState.Patrol);
            }
            else if (GetDistanceToTarget() <= BehaviorProperties.AttackRadius && targetVisible && (!BehaviorProperties.AttackOnlyWhenGrounded || (BehaviorProperties.AttackOnlyWhenGrounded && isGrounded)))
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

        stateTime += Time.fixedDeltaTime * (state == AIState.Charge ? ChargeTimeDifficultyMultiplier.GetFactor() : 1.0f);
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
     * Returns whether the enemy is on the ground
     */
    public bool IsGrounded()
    {
        return isGrounded;
    }

    /**
     * Inheriting enemies can override this to implement their own ground check methods.
     */
    protected virtual void GroundCheck()
    {
        isGrounded = false;
        groundNormal = Vector3.zero;
    }

    /**
     * Returns the origin of the object, potentially transformed by facing direction.
     */
    public Vector3 GetOrigin()
    {
        return trueOrigin;
    }

    /**
     * Updates the true origin.
     */
    protected virtual void UpdateOrigin()
    {
        trueOrigin = transform.TransformPoint(Origin);
    }

    /**
     * Returns the distance to the target/player
     */
    public float GetDistanceToTarget()
    {
        if (Target != null)
        {
            return Vector3.Distance(trueOrigin, Target.position);
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
        targetVisible = CheckVisibility(Target);
        /*if (Target != null && BehaviorProperties != null)
        {
            if (BehaviorProperties.UseLineOfSight)
            {
                Vector3 toTarget = Target.position - trueOrigin;
                RaycastHit[] sightHits = new RaycastHit[BehaviorProperties.MaxSightCastHits];
                //if (Physics.RaycastNonAlloc(trueOrigin, toTarget.normalized, sightHits, toTarget.magnitude, BehaviorProperties.SightMask, QueryTriggerInteraction.Ignore) > 0)
                if (Physics.SphereCastNonAlloc(trueOrigin, 0.1f, toTarget.normalized, sightHits, toTarget.magnitude, BehaviorProperties.SightMask, QueryTriggerInteraction.Ignore) > 0)
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
        targetVisible = false;*/
    }

    /**
     * Returns whether the given transform is visible
     */
    protected bool CheckVisibility(Transform other)
    {
        if (other != null && BehaviorProperties != null)
        {
            if (BehaviorProperties.UseLineOfSight)
            {
                Vector3 toTarget = other.position - trueOrigin;
                RaycastHit[] sightHits = new RaycastHit[BehaviorProperties.MaxSightCastHits];
                //if (Physics.RaycastNonAlloc(trueOrigin, toTarget.normalized, sightHits, toTarget.magnitude, BehaviorProperties.SightMask, QueryTriggerInteraction.Ignore) > 0)
                if (Physics.SphereCastNonAlloc(trueOrigin, 0.1f, toTarget.normalized, sightHits, toTarget.magnitude, BehaviorProperties.SightMask, QueryTriggerInteraction.Ignore) > 0)
                {
                    for (int i = 0; i < sightHits.Length; i++)
                    {
                        RaycastHit curHit = sightHits[i];
                        if (curHit.collider != null)
                        {
                            //Debug.Log(curHit.transform.name);
                            if (!curHit.transform.IsChildOf(transform) && !curHit.transform.IsChildOf(other))
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
     * Returns the time of the current state
     */
    public float GetStateTime()
    {
        return stateTime;
    }

    /**
     * Returns the normalized progress of charging before an attack
     */
    public float GetNormalizedChargeTime()
    {
        if (BehaviorProperties != null && state == AIState.Charge)
        {
            return stateTime / Mathf.Max(0.001f, BehaviorProperties.AttackChargeTime);
        }
        return 0.0f;
    }

    /**
     * Returns an array of all enemies within the alert radius
     */
    public AIController[] GetNearbyEnemies()
    {
        List<AIController> nearEnemies = new List<AIController>();
        if (BehaviorProperties != null)
        {
            for (int i = 0; i < EnemyManager.AllEnemies.Count; i++)
            {
                AIController curEnemy = EnemyManager.AllEnemies[i];
                if (curEnemy != null && curEnemy != this && curEnemy.gameObject.activeSelf)
                {
                    if ((curEnemy.transform.position - transform.position).sqrMagnitude <= BehaviorProperties.AlertEnemiesRadius * BehaviorProperties.AlertEnemiesRadius)
                    {
                        nearEnemies.Add(curEnemy);
                    }
                }
            }
        }
        return nearEnemies.ToArray();
    }

    /**
     * Draw visual representations of properties
     */
    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying) { UpdateOrigin(); }

        if (targetVisible && Target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(trueOrigin, Target.position);
        }

        if (alertTracking)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(trueOrigin, alertPoint);
        }

        if (BehaviorProperties != null)
        {
            Gizmos.color = Color.cyan;
            GizmosExtra.DrawWireCircle(trueOrigin, Vector3.forward, BehaviorProperties.DetectRadius);
            Gizmos.color = Color.yellow;
            GizmosExtra.DrawWireCircle(trueOrigin, Vector3.forward, BehaviorProperties.AvoidRadius);
            Gizmos.color = Color.red;
            GizmosExtra.DrawWireCircle(trueOrigin, Vector3.forward, BehaviorProperties.AttackRadius);
            Gizmos.color = Color.green;
            GizmosExtra.DrawWireCircle(trueOrigin, Vector3.forward, BehaviorProperties.AlertEnemiesRadius);
        }
    }

    /**
     * Overrides the base TakeDamage function to also alert the enemy
     */
    public override float TakeDamage(float amount, Pawn source)
    {
        //Debug.Log("Enemy Damaged");
        if (source != null)
        {
            AlertAndFollow(source.transform);
        }
        return base.TakeDamage(amount, source);
    }

    /**
     * Alert the enemy and follow the target pawn without alerting nearby enemies
     */
    public void AlertAndFollow(Pawn target)
    {
        if (target == null)
            return;
        AlertAndFollow(target.transform, false);
    }

    /**
     * Alert the enemy and follow the target while alerting nearby enemies
     */
    public void AlertAndFollow(Transform target)
    {
        AlertAndFollow(target, true);
    }

    /**
     * Alert the enemy and follow the target with the option to alert nearby enemies
     */
    public void AlertAndFollow(Transform target, bool alertOthers)
    {
        if (target != null && (state == AIState.Patrol || state == AIState.Idle || alertTracking))
        {
            alertPoint = target.position;
            alertTracking = true;
            if (alertOthers)
            {
                AlertNearbyEnemies(target);
            }
        }
    }

    /**
     * Alert nearby enemies and make them follow the target
     */
    public void AlertNearbyEnemies(Transform target)
    {
        bool onlyNear = false;
        if (BehaviorProperties != null)
        {
            onlyNear = BehaviorProperties.OnlyAlertVisibleEnemies;
        }

        AIController[] nearEnemies = GetNearbyEnemies();
        for (int i = 0; i < nearEnemies.Length; i++)
        {
            if (!onlyNear || (onlyNear && CheckVisibility(nearEnemies[i].transform)))
            {
                nearEnemies[i].AlertAndFollow(target, false);
            }
        }
    }

    private void OnDestroy()
    {
        if (EnemyManager.AllEnemies.Contains(this))
        {
            EnemyManager.AllEnemies.Remove(this);
        }
    }
}

// Class for events where an AI state is passed as an argument
[System.Serializable]
public class AIStateEvent : UnityEvent<AIController.AIState> { }