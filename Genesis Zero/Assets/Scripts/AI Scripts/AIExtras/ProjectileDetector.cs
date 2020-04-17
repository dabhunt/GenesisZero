using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Justin Couch
 * ProjectileDetector is used for detecting collisions with projectiles without destroying them
 */
public class ProjectileDetector : MonoBehaviour
{
    public UnityEvent DetectionEvent;

    private void OnTriggerEnter(Collider other)
    {
        Projectile p = other.GetComponentInChildren<Projectile>();
        if (p != null)
        {
            DetectionEvent.Invoke();
        }
    }
}
