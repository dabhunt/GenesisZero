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

    private Vector3 velocity = Vector3.zero;
    public Vector3 GetVelocity() { return velocity; }

    public float GravityFactor = 1.0f;
    public float friction = 1.0f;
    public float drag = 0.0f;

    private void Awake()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    private void FixedUpdate()
    {
        Accelerate(Physics.gravity * GravityFactor);
        Accelerate(-velocity * drag);

        //rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        velocity.Set(velocity.x, velocity.y, 0.0f);
        tr.Translate(velocity * Time.fixedDeltaTime, Space.World);
    }

    /**
    * Accelerates the rigidbody in the given direction
    */
    public void Accelerate(Vector3 accel)
    {
        velocity += accel * Time.fixedDeltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            velocity = Vector3.ProjectOnPlane(velocity, collision.contacts[i].normal);
            velocity -= velocity * (1.0f - Mathf.Abs(Vector3.Dot(velocity, collision.contacts[i].normal))) * friction * Time.fixedDeltaTime;
        }
    }
}
