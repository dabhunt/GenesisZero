using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * ObjectTracker tracks the path of an object over time.
 */
public class ObjectTracker : MonoBehaviour
{
    public Transform Target;
    private Queue<Vector3> points = new Queue<Vector3>();
    public int MaxPoints = 100;
    public float TrackInterval = 0.1f;
    private float trackTime = 0.0f;
    private bool tracking = false;

    private void FixedUpdate()
    {
        if (Target == null) { return; }

        if (tracking)
        {
            trackTime -= Time.fixedDeltaTime;
            if (trackTime <= 0)
            {
                if (points.Count >= MaxPoints)
                {
                    points.Dequeue();
                }
                points.Enqueue(Target.position);
                trackTime = TrackInterval;
            }
        }
        else
        {
            trackTime = 0.0f;
        }
    }

    /**
     * Sets the tracker to begin tracking the position of the target.
     */
    public void StartTracking()
    {
        tracking = true;
    }

    /**
     * Sets the tracker to stop tracking the position of the target.
     */
    public void StopTracking()
    {
        tracking = false;
    }

    /**
     * Clears the list of tracked points.
     */
    public void ClearTrackedPoints()
    {
        points.Clear();
    }

    /**
     * Returns the number of tracked points.
     */
    public int GetPointCount()
    {
        return points.Count;
    }

    /**
     * Gets the oldest tracked point and dequeues it.
     */
    public Vector3 DequeueFirstPoint()
    {
        if (points.Count > 0)
        {
            return points.Dequeue();
        }
        else
        {
            return Vector3.zero;
        }
    }
}
