using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FakeRigidbody))]
/**
 * Justin Couch
 * PlatformPatrollerAI is the class representing the platform patroller enemy type.
 */
public class PlatformPatrollerAI : AIController
{
    protected FakeRigidbody frb;

    public float MoveSpeed = 10f; // Maximum movement speed
    public float LungeSpeed = 20f; // Lunging attack speed
    public float JumpSpeed = 10f; // Vertical jump speed for lunge attack
    private float targetSpeed = 0.0f;
    public float Acceleration = 5.0f; // Rate of acceleration

    public float PatrolSpeed = 5.0f; // Movement speed while patrolling
    public float PatrolSwitchRate = 1.0f; // Rate at which the enemy switches directions while patrolling
    private int faceDir = 1; // Direction the enemy is facing: 1 = right, -1 = left

    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;

    protected void Awake()
    {
        frb = GetComponent<FakeRigidbody>();
    }

    new protected void Start()
    {
        base.Start();
        faceDir = Mathf.RoundToInt(Mathf.Sign(Random.value - 0.5f));
    }

    new protected void Update()
    {
        base.Update();
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target == null) { return; }

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Cooldown)
        {
            faceDir = Mathf.RoundToInt(Mathf.Sign(Target.position.x - transform.position.x));
            targetSpeed = MoveSpeed;
        }
        else if (state == AIState.Attack)
        {
            targetSpeed = LungeSpeed;
        }
        else if (state == AIState.Patrol)
        {
            targetSpeed = PatrolSpeed;
            faceDir = Mathf.RoundToInt(Mathf.Sign(Mathf.Sin(Time.time * PatrolSwitchRate)));
        }
        else if (state == AIState.Idle)
        {
            targetSpeed = 0.0f;
        }

        // Separate checks for logic that only applies to individual states
        if (state == AIState.Follow)
        {
            targetSpeed = MoveSpeed;
        }
        else if (state == AIState.Charge)
        {
            targetSpeed = MoveSpeed * 0.1f;
        }
        else if (state == AIState.Cooldown)
        {
            targetSpeed = 0.0f;
        }

        targetSpeed *= GetSpeed().GetValue();
        frb.Accelerate(Vector3.right * (targetSpeed * faceDir - frb.GetVelocity().x) * Acceleration); // Accelerate toward the target
        transform.rotation = Quaternion.LookRotation(Vector3.forward * faceDir, Vector3.up);

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
     * This initiates the lunge toward the target while attacking
     */
    public void JumpTowardTarget(AIState state)
    {
        if (state == AIState.Attack)
        {
            frb.AddVelocity(Vector3.up * JumpSpeed + Vector3.right * faceDir * LungeSpeed);
        }
    }
}
