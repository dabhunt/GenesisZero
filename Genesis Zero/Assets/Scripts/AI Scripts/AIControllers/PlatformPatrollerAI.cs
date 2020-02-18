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
    public float MaxFollowHeight = 5.0f; // Maximum height above the enemy for which the target will be tracked after going out of sight

    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;
    public GameObject AttackHitbox;
    private Transform spawnedHitboxObj; // Refernce to currently spawned hitbox object

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

    protected override void SetTarget(Transform tr)
    {
        base.SetTarget(tr);
        if (Target != null && tracker != null)
        {
            tracker.GiveUpCondition = () => { return tracker.PeekFirstPoint().y > transform.position.y + MaxFollowHeight; };
        }
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target == null) { return; }

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Cooldown)
        {
            faceDir = Mathf.RoundToInt(Mathf.Sign(targetPosition.x - transform.position.x));
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

    /**
     * This spawns the hitbox to damage the player while lunging
     */
    public void SpawnAttackHitbox()
    {
        if (AttackHitbox != null)
        {
            spawnedHitboxObj = Instantiate(AttackHitbox, Vector3.zero, Quaternion.identity, transform).transform;
            spawnedHitboxObj.localPosition = Vector3.zero;
            spawnedHitboxObj.localRotation = Quaternion.identity;
            Hitbox spawnedHitbox = spawnedHitboxObj.GetComponent<Hitbox>();
            spawnedHitbox.InitializeHitbox(GetDamage().GetValue(), this);
        }
    }
    
    /**
     * Cleans up currently spawned attack hitbox
     */
    public void DestroyAttackHitbox()
    {
        if (spawnedHitboxObj != null)
        {
            Destroy(spawnedHitboxObj.gameObject);
        }
    }
}
