using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Justin Couch
 * ObjectTracker tracks the path of an object over time.
 */
public class ObjectTracker : MonoBehaviour
{
    public Transform Target;
    private Queue<Vector3> points = new Queue<Vector3>();
    private int pointsAdded = 0;
    public int MaxPoints = 100;
    public float TrackInterval = 0.1f;
    private float trackTime = 0.0f;
    private bool tracking = false;
    public float AutoDequeueDistance = -1.0f; // If the tracker is closer than this distance to the oldest point, it will be dequeued
    private bool reachedEnd = false;

    private void FixedUpdate()
    {
        if (Target == null) { return; }

        if (tracking)
        {
            trackTime -= Time.fixedDeltaTime;
            if (trackTime <= 0 && points.Count < MaxPoints && pointsAdded < MaxPoints)
            {
                points.Enqueue(Target.position);
                trackTime = TrackInterval;
                pointsAdded++;
            }
        }
        else
        {
            trackTime = 0.0f;
        }

        if (AutoDequeueDistance >= 0 && points.Count > 0)
        {
            if (Vector3.Distance(transform.position, points.Peek()) <= AutoDequeueDistance)
            {
                DequeueFirstPoint();
            }
        }
    }

    /**
     * Sets the tracker to begin tracking the position of the target
     */
    public void StartTracking()
    {
        tracking = true;
    }

    /**
     * Sets the tracker to stop tracking the position of the target
     */
    public void StopTracking()
    {
        tracking = false;
    }

    /**
     * Clears the list of tracked points
     */
    public void ClearTrackedPoints()
    {
        points.Clear();
    }

    /**
     * Resets the state of the tracker
     */
    public void Reset()
    {
        points.Clear();
        reachedEnd = false;
        pointsAdded = 0;
    }

    /**
     * Returns whether the tracker has reached the end of the points
     */
    public bool HasReachedEnd()
    {
        return reachedEnd;
    }

    /**
     * Returns the number of tracked points
     */
    public int GetPointCount()
    {
        return points.Count;
    }

    /**
     * Gets the oldest tracked point
     */
    public Vector3 PeekFirstPoint()
    {
        if (points.Count > 0)
        {
            return points.Peek();
        }
        else
        {
            return Vector3.zero;
        }
    }

    /**
     * Gets the oldest tracked point and dequeues it
     */
    public Vector3 DequeueFirstPoint()
    {
        if (points.Count > 0)
        {
            if (points.Count == 1)
            {
                reachedEnd = true;
            }
            return points.Dequeue();
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawWireSphere(points.ElementAt(i), 0.1f);
        }
    }
}
