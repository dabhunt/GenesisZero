using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FakeRigidbody))]
/**
 * Justin Couch
 * DroneAI is the class representing the flying drone enemy type.
 */
public class DroneAI : AIController
{
    protected FakeRigidbody frb;

    private Vector3 lookDir = Vector3.up;
    public float RotationRate = 10f; // How fast to rotate
    public float MoveSpeed = 10f; // Maximum movement speed
    private float targetSpeed = 0.0f;
    public float Acceleration = 5.0f; // Rate of acceleration
    public float SideDecel = 1.0f; // Rate of deceleration for sideways velocity to create tighter movement
    public float AvoidAmount = 1.0f; // How much to accelerate away from the target
    public float AvoidAccelLimit = 1.0f; // Limit on avoidance acceleration

    public float PatrolSpeed = 5.0f; // Movement speed while patrolling
    public float PatrolRotateRate = 1.0f; // Rotation rate while patrolling
    private int patrolDir = 1;

    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;

    protected void Awake()
    {
        frb = GetComponent<FakeRigidbody>();
    }

    new protected void Start()
    {
        base.Start();
        patrolDir = Mathf.RoundToInt(Mathf.Sign(Random.value - 0.5f));
        if (Target == null)
        {
            Target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    new protected void Update()
    {
        base.Update();

        if (Target == null) { return; }

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
        {
            // Rotation assumes that local up direction is forward
            lookDir = Vector3.Slerp(lookDir, Target.position - transform.position, RotationRate * Time.deltaTime); // Rotate to face target
            targetSpeed = MoveSpeed;
            frb.Accelerate((transform.position - Target.position).normalized * Mathf.Min(GetAvoidCloseness(), AvoidAccelLimit) * Acceleration * AvoidAmount); // Acceleration to keep away from the target
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

        frb.Accelerate(transform.up * (targetSpeed - frb.GetVelocity().magnitude * Mathf.Clamp01(Vector3.Dot(transform.up, frb.GetVelocity().normalized))) * Acceleration); // Accelerate toward the target
        frb.Accelerate(-transform.right * frb.GetVelocity().magnitude * Vector3.Dot(transform.right, frb.GetVelocity().normalized) * SideDecel); // Deceleration to prevent sideways movement
        transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir); // Actual rotation
        //transform.Translate(velocity * Time.deltaTime, Space.World); // Actual translation based on velocity

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
}
