using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BUG-E Way Point")]
public class WayPoint : ScriptableObject
{
    public Vector3 Destination;
    public float DurationAtWayPoint = 1f;
    public Vector3 LookLocation;
    public bool AtLocation = false;

    public void SetValues(Vector3 location, float duration, Vector3 look)
    {
        Destination = location;
        DurationAtWayPoint = duration;
        LookLocation = look;
    }
    public void SetValues(Vector3 location, float duration)
    {
        Destination = location;
        DurationAtWayPoint = duration;
    }
}
