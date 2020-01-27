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
    private float targetSpeed = 0.0f;
    public float Acceleration = 5.0f; // Rate of acceleration
    public float SideDecel = 1.0f; // Rate of deceleration for sideways velocity to create tighter movement
    private Vector3 velocity = Vector3.zero;
    public float AvoidAmount = 1.0f; // How much to accelerate away from the target
    public float AvoidAccelLimit = 1.0f; // Limit on avoidance acceleration

    public float PatrolSpeed = 5.0f; // Movement speed while patrolling
    public float PatrolRotateRate = 1.0f; // Rotation rate while patrolling
    private int patrolDir = 1;

    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;

    protected override void Awake()
    {
        base.Awake();
        patrolDir = Mathf.RoundToInt(Mathf.Sign(Random.value - 0.5f));
    }

    public void Update()
    {
        base.Update();

        if (Target == null) { return; }

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
        {
            // Rotation assumes that local up direction is forward
            lookDir = Vector3.Slerp(lookDir, Target.position - tr.position, RotationRate * Time.deltaTime); // Rotate to face target
            targetSpeed = MoveSpeed;
            Accelerate((tr.position - Target.position).normalized * Mathf.Min(GetAvoidCloseness(), AvoidAccelLimit) * Acceleration * AvoidAmount); // Acceleration to keep away from the target
        }
        else if (state == AIState.Patrol)
        {
            //For now, patrolling just moves the drone in a circle
            lookDir = Quaternion.AngleAxis(PatrolRotateRate * patrolDir * Time.deltaTime, Vector3.forward) * lookDir;
            targetSpeed = PatrolSpeed;
        }
        else if (state == AIState.Idle)
        {
            targetSpeed = 0.0f;
        }

        Accelerate(tr.up * (targetSpeed - velocity.magnitude * Mathf.Clamp01(Vector3.Dot(tr.up, velocity.normalized))) * Acceleration); // Accelerate toward the target
        Accelerate(-tr.right * velocity.magnitude * Vector3.Dot(tr.right, velocity.normalized) * SideDecel); // Deceleration to prevent sideways movement
        tr.rotation = Quaternion.LookRotation(Vector3.forward, lookDir); // Actual rotation
        tr.Translate(velocity * Time.deltaTime, Space.World); // Actual translation based on velocity

        // Particles to show charge and attack states (for testing)
        if (chargeParticles != null)
        {
            if (state == AIState.Charge)
            {
                if (!chargeParticles.isPlaying)
                {
                    chargeParticles.Play();
                }
            }
            else if (chargeParticles.isPlaying)
            {
                chargeParticles.Stop();
            }
        }

        if (attackParticles != null)
        {
            if (state == AIState.Attack)
            {
                if (!attackParticles.isPlaying)
                {
                    attackParticles.Play();
                }
            }
            else if (attackParticles.isPlaying)
            {
                attackParticles.Stop();
            }
        }
    }

    /**
     * Accelerates the drone in the given direction
     */
    public void Accelerate(Vector3 accel)
    {
        velocity += accel * Time.deltaTime;
    }
}
