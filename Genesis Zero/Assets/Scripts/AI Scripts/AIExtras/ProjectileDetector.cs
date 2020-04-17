using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * ProjectileDetector is used for detecting collisions with projectiles without destroying them
 */
public class ProjectileDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}
