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
    public float LungeSpeed = 20f; // Maximum movement speed
    private float targetSpeed = 0.0f;
    public float Acceleration = 5.0f; // Rate of acceleration
    private Vector3 velocity = Vector3.zero;
    public float AvoidAmount = 1.0f; // How much to accelerate away from the target
    public float AvoidAccelLimit = 1.0f; // Limit on avoidance acceleration

    private BoxCollider mainCollider; // Physical collider for movement
    public LayerMask GroundMask; // Layermask for ground objects

    public float PatrolSpeed = 5.0f; // Movement speed while patrolling
    private int faceDir = 1; // Direction the enemy is facing: 1 = right, -1 = left

    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;

    protected void Awake()
    {
        frb = GetComponent<FakeRigidbody>();
        mainCollider = GetComponent<BoxCollider>();
    }

    new protected void Start()
    {
        base.Start();
        faceDir = Mathf.RoundToInt(Mathf.Sign(Random.value - 0.5f));
        if (Target == null)
        {
            Target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    new protected void Update()
    {
        base.Update();
    }

    protected void FixedUpdate()
    {
        //base.Update();

        if (Target == null || mainCollider == null) { return; }

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
        {
            faceDir = Mathf.RoundToInt(Mathf.Sign(Target.position.x - transform.position.x));
            targetSpeed = MoveSpeed;
            //Accelerate((transform.position - Target.position).normalized * Mathf.Min(GetAvoidCloseness(), AvoidAccelLimit) * Acceleration * AvoidAmount); // Acceleration to keep away from the target
        }
        else if (state == AIState.Patrol)
        {
            targetSpeed = PatrolSpeed;
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
        else if (state == AIState.Attack)
        {
            targetSpeed = LungeSpeed;
        }
        else if (state == AIState.Cooldown)
        {
            targetSpeed = 0.0f;
        }

        frb.Accelerate(Vector3.right * (targetSpeed * faceDir - velocity.x) * Acceleration); // Accelerate toward the target
        //transform.Translate(velocity * Time.fixedDeltaTime, Space.World); // Actual translation based on velocity

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
}
