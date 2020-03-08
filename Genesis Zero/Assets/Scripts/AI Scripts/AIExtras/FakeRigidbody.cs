using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
/**
 * Justin Couch
 * FakeRigidbody is designed to act like a rigidbody, but moves through translation rather than with the physics engine.
 */
public class FakeRigidbody : MonoBehaviour
{
    private Transform tr;
    private Rigidbody rb;

    private Vector3 velocity = Vector3.zero;

    /**
     * Returns the current velocity
     */
    public Vector3 GetVelocity() { return velocity; }

    public float GravityFactor = 1.0f; // Multiplier for gravity in the physics project settings
    public float friction = 1.0f; // How much to slow down when touching a surface
    public float drag = 0.0f; // How much to slow down at all times

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

    /**
    * Instantly increases the velocity by the given vector
    */
    public void AddVelocity(Vector3 vel)
    {
        if (Vector3.Dot(velocity, vel) < 0)
        {
            velocity = Vector3.ProjectOnPlane(velocity, vel.normalized);
        }
        velocity += vel;
    }

    /**
     * Called whenever touching a surface
     */
    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector3.Dot(velocity, collision.contacts[i].normal) < 0)
            {
                velocity = Vector3.ProjectOnPlane(velocity, collision.contacts[i].normal);
                velocity -= velocity * (1.0f - Mathf.Abs(Vector3.Dot(velocity, collision.contacts[i].normal))) * friction * Time.fixedDeltaTime;
            }
        }
    }
}
