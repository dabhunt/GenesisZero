using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
/**
 * Justin Couch
 * FakeRigidbody is designed to act like a rigidbody, but moves instead with translation rather than through the physics engine.
 */
public class FakeRigidbody : MonoBehaviour
{
    private Transform tr;
    private Rigidbody rb;

    private Vector3 velocity;
    public Vector3 GetVelocity() { return velocity; }

    private void Awake()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    public void Accelerate(Vector3 accel)
    {
        velocity += accel;
    }
}
