using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * TestPhysicsObject is a basic class for handling collision detection for prototyping.
 */
public class TestPhysicsObjectBox : MonoBehaviour
{
    private Collider myCollider;
    public int MaxCollisions = 8;
    private Collider[] hitColliders;
    

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        hitColliders = new Collider[MaxCollisions];
    }

    private void FixedUpdate()
    {
        if (myCollider == null) { return; }

    }
}
