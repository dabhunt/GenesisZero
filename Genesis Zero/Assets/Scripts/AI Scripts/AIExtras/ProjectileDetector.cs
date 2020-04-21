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
    [System.Serializable]
    public class PawnEvent : UnityEvent<Pawn> { } // Class for UnityEvents with a Pawn argument
    public PawnEvent DetectionEvent; // Invoked with a hitbox source passed as the argument

    private void OnTriggerEnter(Collider other)
    {
        Hitbox hb = other.GetComponentInChildren<Hitbox>();
        if (hb != null)
        {
            DetectionEvent.Invoke(hb.Source);
        }
    }
}
