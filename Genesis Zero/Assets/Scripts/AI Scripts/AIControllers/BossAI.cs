﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * BossAI is the class representing the boss of the game
 */
public class BossAI : AIController
{
    public enum State { Headbutt, Firebreath, Pulse, Wild, MovingAway, MovingCloser, Centering }
    protected State bossstate = State.Firebreath; // Current behavior state
    private bool secondphase;

    private Vector3 lookDir = Vector3.up;
    private Vector3 rotDir = Vector3.forward;
    private Vector3 lookposition;

    [Tooltip("Points the boss uses to determine how to do it's attacks")]
    public List<Transform> Waypoints;
    public Transform Center;

    private float lookAngle = 0;
    private float zdepth;

    public float TriggerRadius;
    public float TimeBeforeFight;
    private bool initiated;
    protected void Awake()
    {
        zdepth = transform.position.z;
        lookposition = transform.position;
    }

    new protected void Start()
    {
        base.Start();
    }

    new protected void Update()
    {
        base.Update();
        if (GetDistanceToTarget() < TriggerRadius) { initiated = true; }
        if (initiated && TimeBeforeFight > 0) { TimeBeforeFight -= Time.deltaTime; }
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target == null) { return; }
        if (initiated == false) { return; }

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
        {
            lookDir = Vector3.Slerp(lookDir, targetPosition - transform.position, 5 * Time.fixedDeltaTime); // Rotate to face target
        }
        else if (state == AIState.Patrol)
        {
            //For now, patrolling just moves the drone in a circle
            lookDir = Quaternion.AngleAxis(5 * Time.fixedDeltaTime, Vector3.forward) * lookDir;
        }
        else if (state == AIState.Idle) // Stunned
        {

        }

        // Set where the boss looks at, default player
        lookposition = Vector3.Lerp(lookposition, Target.position, 3 * Time.fixedDeltaTime);

        Vector3 lookoffset = new Vector3(0, 0, lookDir.x > 0 ? -1f : -1f);
        transform.LookAt(lookposition + lookoffset);
        //transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up); // USe this for center state


        // Don't move until fight starts
        if (TimeBeforeFight > 0) { return; }

        // Move toward target, may move somewhere depending on state
        float speed = GetSpeed().GetValue();
        if (GetDistanceToTarget() - BehaviorProperties.AvoidRadius != 0)
        {
            float diff = GetDistanceToTarget() - BehaviorProperties.AvoidRadius;
            transform.position = Vector2.MoveTowards(transform.position, Target.position, (5 * diff / GetDistanceToTarget()) * Time.fixedDeltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, zdepth);
        }


    }

    /**
     * Checks for closeness to walls in order to avoid them
     */
    protected void CheckWalls()
    {
        if (!wallCheckCycleInProgress)
        {
            StartCoroutine(WallCheckCycle());
        }
    }

    [Header("Wall Checking")]
    public int WallCheckCasts = 6;
    public float WallCheckDistance = 1.0f;
    public LayerMask WallMask;
    private bool wallCheckCycleInProgress = false;
    protected bool isCloseToWall = false;
    protected Vector3 wallPoint = Vector3.zero;
    public float WallAvoidForce = 10f;

    IEnumerator WallCheckCycle()
    {
        wallCheckCycleInProgress = true;
        float castAngle = 2.0f / WallCheckCasts * Mathf.PI;
        float curAngle = 0.0f;
        Vector3 castDir = Vector3.zero;
        for (int i = 0; i < WallCheckCasts; i++)
        {
            castDir = new Vector3(Mathf.Sin(curAngle), Mathf.Cos(curAngle), 0.0f);
            RaycastHit hit = new RaycastHit();
            isCloseToWall = Physics.Raycast(transform.position, castDir, out hit, WallCheckDistance, WallMask, QueryTriggerInteraction.Ignore);
            wallPoint = isCloseToWall ? hit.point : Vector3.zero;
            //Debug.DrawRay(transform.position, castDir * WallCheckDistance);
            curAngle += castAngle;
            yield return new WaitForFixedUpdate();
        }
        wallCheckCycleInProgress = false;
    }

    // Returns waypoint furthest away from target
    public Transform GetFurthestWaypoint()
    {
        if (Waypoints != null)
        {
            Debug.LogWarning("Waypoints not initialized for boss");
            return transform;
        }

        Transform max = Waypoints[0];
        float maxdist = Vector2.Distance(max.position, Target.position);
        foreach (Transform t in Waypoints)
        {
            float currdist = Vector2.Distance(t.position, Target.position);
            if (currdist > maxdist)
            {
                max = t;
                maxdist = currdist;
            }
        }
        return max;
    }

    // Returns waypoint closest to the target
    public Transform GetClosestWaypoint()
    {
        if (Waypoints != null)
        {
            Debug.LogWarning("Waypoints not initialized for boss");
            return transform;
        }

        Transform min = Waypoints[0];
        float mindist = Vector2.Distance(min.position, Target.position);
        foreach (Transform t in Waypoints)
        {
            float currdist = Vector2.Distance(t.position, Target.position);
            if (currdist < mindist)
            {
                min = t;
                mindist = currdist;
            }
        }
        return min;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, TriggerRadius);
    }
}
