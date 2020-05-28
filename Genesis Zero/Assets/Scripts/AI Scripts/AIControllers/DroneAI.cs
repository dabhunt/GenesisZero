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
    private Vector3 lookDir = Vector3.up;
    [Header("Movement")]
    public float RotationRate = 10f; // How fast to rotate
    public float MoveSpeed = 10f; // Maximum movement speed
    private float targetSpeed = 0.0f;
    public float Acceleration = 5.0f; // Rate of acceleration
    public float SideDecel = 1.0f; // Rate of deceleration for sideways velocity to create tighter movement
    public float AvoidAmount = 1.0f; // How much to accelerate away from the target
    public float AvoidAccelLimit = 1.0f; // Limit on avoidance acceleration
    public GameObject spawnProjectileSpot;

    public float PatrolSpeed = 5.0f; // Movement speed while patrolling
    public float PatrolRotateRate = 1.0f; // Rotation rate while patrolling
    private int patrolDir = 1;

    [Header("Attack")]
    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;

    public GameObject AttackProjectile;
    public float AttackLaunchInterval = 1.0f;
    private float attackLaunchTime = 0.0f;

    [Header("Difficulty")]
    public DifficultyMultiplier SpeedDifficultyMultiplier;
    public DifficultyMultiplier RotationDifficultyMultiplier;
    public DifficultyMultiplier ShootRateDifficultyMultiplier;

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
            lookDir = Vector3.Slerp(lookDir, targetPosition - transform.position, RotationRate * RotationDifficultyMultiplier.GetFactor() * Time.fixedDeltaTime); // Rotate to face target
            targetSpeed = MoveSpeed;
            frb.Accelerate((transform.position - targetPosition).normalized * Mathf.Min(GetAvoidCloseness(), AvoidAccelLimit) * Acceleration * AvoidAmount * SpeedDifficultyMultiplier.GetFactor()); // Acceleration to keep away from the target
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

        targetSpeed *= GetSpeed().GetValue() * SpeedDifficultyMultiplier.GetFactor();
        frb.Accelerate(transform.up * (targetSpeed - frb.GetVelocity().magnitude * Mathf.Clamp01(Vector3.Dot(transform.up, frb.GetVelocity().normalized))) * Acceleration); // Accelerate toward the target
        frb.Accelerate(-transform.right * frb.GetVelocity().magnitude * Vector3.Dot(transform.right, frb.GetVelocity().normalized) * SideDecel); // Deceleration to prevent sideways movement
        transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir); // Actual rotation

        CheckWalls();
        if (isCloseToWall)
        {
            frb.Accelerate((transform.position - wallPoint).normalized * (1.0f - (transform.position - wallPoint).magnitude / WallCheckDistance) * WallAvoidForce);
        }
    }

    new protected void Update()
    {
        base.Update();
        if (Target == null) { return; }

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

        if (state == AIState.Attack)
        {
            if (attackLaunchTime <= 0)
            {
                attackLaunchTime = AttackLaunchInterval * ShootRateDifficultyMultiplier.GetFactor();
                if (AttackProjectile != null)
                {
                    GameObject spawnedProjectile = Instantiate(AttackProjectile, spawnProjectileSpot.transform.position, transform.rotation);
                    Hitbox spawnedHitbox = spawnedProjectile.GetComponent<Hitbox>();
                    if (spawnedHitbox != null)
                    {
                        spawnedHitbox.InitializeHitbox(GetDamage().GetValue() * DamageDifficultyMultiplier.GetFactor(), this);
                    }
                }
            }
        }
        else
        {
            attackLaunchTime = 0.0f;
        }

        attackLaunchTime = Mathf.Max(0.0f, attackLaunchTime - Time.deltaTime);
    }

    /**
     * Checks for closeness to walls in order to avoid them
     */
    protected void CheckWalls()
    {
        if (!wallCheckCycleInProgress)
        {
            StartCoroutine(WallCheckCycle());
        }
    }

    [Header("Wall Checking")]
    public int WallCheckCasts = 6;
    public float WallCheckDistance = 1.0f;
    public LayerMask WallMask;
    private bool wallCheckCycleInProgress = false;
    protected bool isCloseToWall = false;
    protected Vector3 wallPoint = Vector3.zero;
    public float WallAvoidForce = 10f;

    IEnumerator WallCheckCycle()
    {
        wallCheckCycleInProgress = true;
        float castAngle = 2.0f / WallCheckCasts * Mathf.PI;
        float curAngle = 0.0f;
        Vector3 castDir = Vector3.zero;
        for (int i = 0; i < WallCheckCasts; i++)
        {
            castDir = new Vector3(Mathf.Sin(curAngle), Mathf.Cos(curAngle), 0.0f);
            RaycastHit hit = new RaycastHit();
            isCloseToWall = Physics.Raycast(transform.position, castDir, out hit, ScaleFloat(WallCheckDistance), WallMask, QueryTriggerInteraction.Ignore);
            wallPoint = isCloseToWall ? hit.point : Vector3.zero;
            //Debug.DrawRay(transform.position, castDir * WallCheckDistance);
            curAngle += castAngle;
            yield return new WaitForFixedUpdate();
        }
        wallCheckCycleInProgress = false;
    }
}
