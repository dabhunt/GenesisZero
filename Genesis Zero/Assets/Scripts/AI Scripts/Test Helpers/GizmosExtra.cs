using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * Class for extra gizmo drawing methods.
 */
public static class GizmosExtra
{
    public static void DrawWireCircle(Vector3 position, Vector3 normalAxis, float radius)
    {
        int segments = 30;
        float segmentAngle = 360f / segments;
        Quaternion rot = Quaternion.AngleAxis(0.0f, normalAxis);
        Quaternion rotPrev = rot;
        for (int i = 0; i <= segments; i++)
        {
            rotPrev = rot;
            rot = Quaternion.AngleAxis(i * segmentAngle, normalAxis);
            Gizmos.DrawLine(position + rotPrev * Vector3.up * radius, position + rot * Vector3.up * radius);
        }
    }
}
