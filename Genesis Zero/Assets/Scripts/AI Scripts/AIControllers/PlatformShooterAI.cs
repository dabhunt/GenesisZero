﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FakeRigidbody))]
/**
 * Justin Couch
 * PlatformShooterAI is the class representing the platform shooter enemy type.
 */
public class PlatformShooterAI : AIController
{
    protected FakeRigidbody frb;

    public float MoveSpeed = 10f; // Maximum movement speed
    private float targetSpeed = 0.0f;
    public float Acceleration = 5.0f; // Rate of acceleration
    public float AvoidAmount = 1.0f; // How much to accelerate away from the target
    public float AvoidAccelLimit = 1.0f; // Limit on avoidance acceleration

    public float PatrolSpeed = 5.0f; // Movement speed while patrolling
    public float PatrolSwitchRate = 1.0f; // Rate at which the enemy switches directions while patrolling
    private float patrolCycleOffset = 0.0f; // Randomly set offset for the patrol cycle
    private int faceDir = 1; // Direction the enemy is facing: 1 = right, -1 = left
    private int faceDirPrev = 1; // Previous face direction
    private float faceDirChangeTime = 0.0f; // Time since the enemy last changed direction
    private float lookAngle = 0.0f; // Angle for rotating the model laterally
    public float rotateRate = 1.0f;
    public float MaxFollowHeight = 5.0f; // Maximum height above the enemy for which the target will be tracked after going out of sight

    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;

    public float groundCheckDistance = 1.0f;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundCheckMask;

    public GameObject AttackProjectile;
    public float AttackLaunchInterval = 1.0f;
    private float attackLaunchTime = 0.0f;

    protected void Awake()
    {
        frb = GetComponent<FakeRigidbody>();
    }

    new protected void Start()
    {
        base.Start();
        faceDir = Mathf.RoundToInt(Mathf.Sign(Random.value - 0.5f));
        patrolCycleOffset = Random.value * Mathf.PI;
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

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
        {
            faceDirPrev = faceDir;
            if (faceDirChangeTime > 0.2f)
            {
                faceDir = Mathf.RoundToInt(Mathf.Sign(targetPosition.x - transform.position.x));
            }

            if (faceDir != faceDirPrev)
            {
                faceDirChangeTime = 0.0f;
            }

            targetSpeed = MoveSpeed;
            frb.Accelerate(Mathf.Sign(transform.position.x - targetPosition.x) * Vector3.right * Mathf.Min(GetAvoidCloseness(), AvoidAccelLimit) * Acceleration * AvoidAmount); // Acceleration to keep away from the target
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
        faceDirChangeTime += Time.fixedDeltaTime;

        targetSpeed *= GetSpeed().GetValue();
        frb.Accelerate(Vector3.right * (targetSpeed * faceDir - frb.GetVelocity().x) * Acceleration); // Accelerate toward the target

        // Smoothly rotate to face target
        lookAngle = Mathf.Lerp(lookAngle, -faceDir * Mathf.PI * 0.5f + Mathf.PI * 0.5f, rotateRate * Time.fixedDeltaTime);
        Vector3 lookDir = new Vector3(Mathf.Sin(lookAngle), 0.0f, Mathf.Cos(lookAngle));
        transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

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

        // Projectile shooting logic
        if (state == AIState.Attack)
        {
            if (attackLaunchTime <= 0)
            {
                attackLaunchTime = AttackLaunchInterval;
                if (AttackProjectile != null)
                {
                    GameObject spawnedProjectile = Instantiate(AttackProjectile, transform.position, Quaternion.LookRotation(Vector3.forward, (Target.position - transform.position).normalized));
                    Hitbox spawnedHitbox = spawnedProjectile.GetComponent<Hitbox>();
                    if (spawnedHitbox != null)
                    {
                        spawnedHitbox.InitializeHitbox(GetDamage().GetValue(), this);
                    }
                }
            }
        }
        else
        {
            attackLaunchTime = 0.0f;
        }

        attackLaunchTime = Mathf.Max(0.0f, attackLaunchTime - Time.fixedDeltaTime);
    }

    /**
      * Checks if the enemy is on the ground
      */
    protected override void GroundCheck()
    {
        Ray groundRay = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.SphereCast(groundRay, groundCheckRadius, groundCheckDistance, groundCheckMask, QueryTriggerInteraction.Ignore);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, groundCheckRadius);
    }
}