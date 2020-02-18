using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FakeRigidbody))]
/**
 * Justin Couch
 * DroneExploderAI is the class representing the flying drone enemy type that explodes when it gets close to the player.
 */
public class DroneExploderAI : AIController
{
    protected FakeRigidbody frb;

    private Vector3 lookDir = Vector3.up;
    public float RotationRate = 10f; // How fast to rotate
    public float MoveSpeed = 10f; // Maximum movement speed
    private float targetSpeed = 0.0f;
    public float Acceleration = 5.0f; // Rate of acceleration
    public float SideDecel = 1.0f; // Rate of deceleration for sideways velocity to create tighter movement

    public float PatrolSpeed = 5.0f; // Movement speed while patrolling
    public float PatrolRotateRate = 1.0f; // Rotation rate while patrolling
    private int patrolDir = 1;

    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;
    public GameObject Explosion;

    protected void Awake()
    {
        frb = GetComponent<FakeRigidbody>();
    }

    new protected void Start()
    {
        base.Start();
        patrolDir = Mathf.RoundToInt(Mathf.Sign(Random.value - 0.5f));
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target == null) { return; }

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
        {
            // Rotation assumes that local up direction is forward
            lookDir = Vector3.Slerp(lookDir, targetPosition - transform.position, RotationRate * Time.fixedDeltaTime); // Rotate to face target
            targetSpeed = MoveSpeed;
        }
        else if (state == AIState.Patrol)
        {
            //For now, patrolling just moves the drone in a circle
            lookDir = Quaternion.AngleAxis(PatrolRotateRate * patrolDir * Time.fixedDeltaTime, Vector3.forward) * lookDir;
            targetSpeed = PatrolSpeed;
        }
        else if (state == AIState.Idle)
        {
            targetSpeed = 0.0f;
        }

        targetSpeed *= GetSpeed().GetValue();
        frb.Accelerate(transform.up * (targetSpeed - frb.GetVelocity().magnitude * Mathf.Clamp01(Vector3.Dot(transform.up, frb.GetVelocity().normalized))) * Acceleration); // Accelerate toward the target
        frb.Accelerate(-transform.right * frb.GetVelocity().magnitude * Vector3.Dot(transform.right, frb.GetVelocity().normalized) * SideDecel); // Deceleration to prevent sideways movement
        transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir); // Actual rotation

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
     * This spawns the explosion hitbox
     */
    public void Explode()
    {
        if (Explosion != null)
        {
            GameObject spawnedExplosion = Instantiate(Explosion, transform.position, Quaternion.identity);
            Hitbox spawnedHitbox = spawnedExplosion.GetComponent<Hitbox>();
            spawnedHitbox.InitializeHitbox(GetDamage().GetValue(), this);
        }
        gameObject.SetActive(false);
    }
}
