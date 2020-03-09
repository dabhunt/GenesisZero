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
    [Header("Movement")]
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

    [Header("Effects")]
    public string vfxName = "VFX_Explosion";
    public float blastRadius = 5f;
    public float lerpMultiplier = 2.3f;
    public float startScale=.01f;

    [Header("Attack")]
    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;
    public GameObject explosionPrefab;

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
            frb.Accelerate((transform.position - targetPosition).normalized * Mathf.Min(GetAvoidCloseness(), AvoidAccelLimit) * Acceleration * AvoidAmount);
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
            patrolDir = Mathf.RoundToInt(Mathf.Sign(Random.value - 0.5f));
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
        if (explosionPrefab != null)
        {
        	
            GameObject spawnedExplosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Hitbox spawnedHitbox = spawnedExplosion.GetComponent<Hitbox>();
            spawnedHitbox.InitializeHitbox(GetDamage().GetValue(), this);
            GameObject emit = VFXManager.instance.PlayEffect(vfxName, new Vector3(transform.position.x, transform.position.y, transform.position.z), 0f, blastRadius);
            spawnedExplosion.GetComponent<ProjectileTest>().DestroyEvent.AddListener(DestroySelf);
            // Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y,0);
            // //this is for collision, not VFX
            // GameObject spawnedExplosion = Instantiate(explosionPrefab,spawnPoint, Quaternion.identity);
            // AOE AOEscript = spawnedExplosion.GetComponent<AOE>();
            // AOEscript.setScaleTarget(startScale, blastRadius, lerpMultiplier);
            // //print("blastRadius = " + blastRadius + "during the on destroy call last section");
            // 
        }
        gameObject.SetActive(false);
    }

    /*
     * Wrapper function to destroy self with no parameters.
     */
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
