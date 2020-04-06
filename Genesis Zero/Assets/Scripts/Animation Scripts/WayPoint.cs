using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BUG-E Way Point")]
public class WayPoint : ScriptableObject
{
    public Vector2 Location;
    public float DurationAtWayPoint = 1f;
    public Vector2 LookLocation;
    public bool Finished = false;

    public void SetValues(Vector2 location, float duration, Vector2 look)
    {
        Location = location;
        DurationAtWayPoint = duration;
        LookLocation = look;
    }
    public void SetValues(Vector2 location, float duration)
    {
        Location = location;
        DurationAtWayPoint = duration;
    }
}
