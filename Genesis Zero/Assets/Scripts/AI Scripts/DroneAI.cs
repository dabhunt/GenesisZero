using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * Drone is the class representing the flying drone enemy type.
 */
public class DroneAI : AIController
{
    private Vector3 lookDir = Vector3.up;
    public float RotationRate = 10f; // How fast to rotate
    public float MoveSpeed = 10f; // Maximum movement speed
    public float Acceleration = 5.0f; // Rate of acceleration
    public float SideDecel = 1.0f; // Rate of deceleration for sideways velocity to create tighter movement
    private Vector3 velocity = Vector3.zero;
    public float AvoidAmount = 1.0f; // How much to accelerate away from the target
    public float AvoidAccelLimit = 1.0f; // Limit on avoidance acceleration

    public void Update()
    {
        base.Update();

        if (Target == null) { return; }

        // Rotation assumes that local up direction is forward
        lookDir = Vector3.Slerp(lookDir, Target.position - tr.position, RotationRate * Time.deltaTime); // Rotate to face target
        tr.rotation = Quaternion.LookRotation(Vector3.forward, lookDir); // Actual rotation

        Accelerate(tr.up * (MoveSpeed - velocity.magnitude * Mathf.Clamp01(Vector3.Dot(tr.up, velocity.normalized))) * Acceleration); // Accelerate toward the target
        Accelerate(-tr.right * velocity.magnitude * Vector3.Dot(tr.right, velocity.normalized) * SideDecel); // Deceleration to prevent sideways movement
        Accelerate((tr.position - Target.position).normalized * Mathf.Min(GetAvoidCloseness(), AvoidAccelLimit) * Acceleration * AvoidAmount); // Acceleration to keep away from the target
        tr.Translate(velocity * Time.deltaTime, Space.World); // Actual translation based on velocity
    }

    /**
     * Accelerates the drone in the given direction
     */
    public void Accelerate(Vector3 accel)
    {
        velocity += accel * Time.deltaTime;
    }
}
