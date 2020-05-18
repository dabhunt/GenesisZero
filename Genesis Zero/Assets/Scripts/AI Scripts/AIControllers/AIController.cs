﻿using System.Collections;
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
    protected AIAnimationStateController anim;

    public enum AIState { Idle, Patrol, Follow, Charge, Attack, Cooldown }
    protected AIState state = AIState.Idle; // Current behavior state
    protected bool initialized = false;

    protected FakeRigidbody frb;
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
    public DifficultyMultiplier CooldownTimeDifficultyMultiplier;

    public float EnemySpaceRadius = 2.0f; // Radius within which enemies will push each other away to avoid overlapping
    public float EnemySpaceForce = 20f; // Force with which to push other enemies away when they're too close

    protected virtual void Awake()
    {
        frb = GetComponent<FakeRigidbody>();
    }

    new protected void Start()
    {
        if (!Mathf.Approximately(transform.localScale.x, transform.localScale.y)
            || !Mathf.Approximately(transform.localScale.y, transform.localScale.z)
            || !Mathf.Approximately(transform.localScale.x, transform.localScale.z))
        {
            transform.localScale = Vector3.one * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        base.Start();
        anim = GetComponent<AIAnimationStateController>();

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

        if (frb != null)
        {
            AIController[] nearEnemies = GetNearbyEnemies(EnemySpaceRadius);
            List<FakeRigidbody> nearBodies = new List<FakeRigidbody>();
            for (int i = 0; i < nearEnemies.Length; i++)
            {
                FakeRigidbody curBody = nearEnemies[i].GetComponent<FakeRigidbody>();
                if (curBody != null)
                {
                    nearBodies.Add(curBody);
                }
            }

            for (int i = 0; i < nearBodies.Count; i++)
            {
                Vector3 bodyDir = transform.position - nearBodies[i].transform.position;
                frb.Accelerate(bodyDir.normalized * Mathf.Min(10f, Mathf.Pow(1.0f - bodyDir.magnitude / EnemySpaceRadius, 2.0f) * EnemySpaceForce));
            }
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
        float stateTimeFactor = 1.0f;
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
            stateTimeFactor = ChargeTimeDifficultyMultiplier.GetFactor();
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
            stateTimeFactor = CooldownTimeDifficultyMultiplier.GetFactor();
            if (stateTime >= BehaviorProperties.AttackCooldownTime)
            {
                ChangeState(AIState.Follow);
            }
        }

        stateTime += Time.fixedDeltaTime * stateTimeFactor;
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
        if (BehaviorProperties != null)
        {
            return GetNearbyEnemies(BehaviorProperties.AlertEnemiesRadius);
        }
        return new AIController[0];
    }

    /**
     * Returns an array of all enemies within the given radius
     */
    public AIController[] GetNearbyEnemies(float radius)
    {
        List<AIController> nearEnemies = new List<AIController>();
        for (int i = 0; i < EnemyManager.AllEnemies.Count; i++)
        {
            AIController curEnemy = EnemyManager.AllEnemies[i];
            if (curEnemy != null && curEnemy != this && curEnemy.gameObject.activeSelf)
            {
                if ((curEnemy.transform.position - transform.position).sqrMagnitude <= radius * radius)
                {
                    nearEnemies.Add(curEnemy);
                }
            }
        }
        return nearEnemies.ToArray();
    }

    /**
     * Draw visual representations of properties
     */
    protected virtual void OnDrawGizmosSelected()
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
        if (target == null) { return; }
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
            if (CheckVisibility(target.transform))
            {
                alertPoint = target.position;
                alertTracking = true;
                //AudioManager.instance.PlayRandomSFXType("Enemy", this.gameObject);
                print("enemy SFX");
                AudioManager.instance.PlayRandomSFXType("EnemyNearby", this.gameObject, .2f);
                if (alertOthers)
                {
                    AlertNearbyEnemies(target);
                }
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

    /**
     * Returns the aim direction to the target for enemies that shoot
     */
    public virtual Vector3 GetAimDirection()
    {
        if (Target != null)
        {
            return (Target.position - transform.position).normalized;
        }
        return Vector3.right;
    }

    /**
     * Returns the launching point for projectiles
     */
    public virtual Vector3 GetProjectilePoint()
    {
        return GetOrigin();
    }

    /**
     * Scales a vector3 by the local scale
     */
    protected Vector3 ScaleVector3(Vector3 v)
    {
        return Vector3.Scale(v, transform.localScale);
    }

    /**
     * Scales a float by the local scale magnitude
     */
    protected float ScaleFloat(float f)
    {
        return f * (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3.0f;
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