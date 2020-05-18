using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FakeRigidbody))]
/**
 * Justin Couch
 * PlatformShooterAI is the class representing the platform shooter enemy type.
 */
public class PlatformShooterAI : AIController
{
    [Header("Movement")]
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

    [Header("Ground Checking")]
    public float groundCheckDistance = 1.0f;
    public float groundCheckStartHeight = 0.0f;
    public float groundCheckRadius = 0.5f;
    public Vector3 ForwardEdgeRay;
    public Vector3 BackEdgeRay;
    private bool edgeInFront = true;
    private bool edgeBehind = true;
    public LayerMask groundCheckMask;

    [Header("Attack")]
    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;
    public GameObject AttackProjectile;
    public float AttackLaunchInterval = 1.0f;
    private float attackLaunchTime = 0.0f;
    public float AimSpeed = 1.0f;
    public Vector3 ProjectileStart = Vector3.zero;
    private Vector3 projectileAim = Vector3.right;
    public Transform ProjectileSource;
    private Vector3 nextAim = Vector3.right;

    [Header("Difficulty")]
    public DifficultyMultiplier SpeedDifficultyMultiplier;
    public DifficultyMultiplier AimDifficultyMultiplier;
    public DifficultyMultiplier ShootRateDifficultyMultiplier;

    new protected void Start()
    {
        base.Start();
        faceDir = Mathf.RoundToInt(Mathf.Sign(Random.value - 0.5f));
        projectileAim = Vector3.right * faceDir;
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
            tracker.GiveUpCondition = () =>
            {
                return tracker.PeekFirstPoint().y > transform.position.y + ScaleFloat(MaxFollowHeight)
                || tracker.PeekFirstPoint().y < transform.position.y - ScaleFloat(MaxFollowHeight);
            };
        }
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target == null) { return; }

        float slopeForceFactor = Vector3.Dot(groundNormal, Vector3.left * faceDir) + 1.0f; // Adjust movement force based on slope steepness

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
        {
            faceDirPrev = faceDir;
            if (faceDirChangeTime > 0.2f && state != AIState.Attack && state != AIState.Cooldown)
            {
                faceDir = Mathf.RoundToInt(Mathf.Sign(targetPosition.x - transform.position.x));
            }

            if (faceDir != faceDirPrev)
            {
                faceDirChangeTime = 0.0f;
            }

            targetSpeed = MoveSpeed;
            if (state == AIState.Attack)
            {
                targetSpeed *= 0.5f;
            }
            if (state == AIState.Cooldown)
            {
                targetSpeed *= 0.2f;
            }

            if (isGrounded && faceDir == Mathf.RoundToInt(Mathf.Sign(targetPosition.x - transform.position.x)))
            {
                if (!edgeInFront)
                {
                    targetSpeed = 0.0f;
                }

                if (edgeBehind && state != AIState.Attack && state != AIState.Cooldown)
                {
                    frb.Accelerate(Mathf.Sign(transform.position.x - targetPosition.x) * Vector3.right * Mathf.Min(GetAvoidCloseness(), AvoidAccelLimit) * Acceleration * AvoidAmount * slopeForceFactor * SpeedDifficultyMultiplier.GetFactor()); // Acceleration to keep away from the target
                }
            }
        }
        else if (state == AIState.Patrol)
        {
            targetSpeed = isGrounded ? PatrolSpeed : 0.0f;
            if (isGrounded && !edgeInFront)
            {
                patrolCycleOffset += Mathf.PI;
            }
            faceDir = Mathf.RoundToInt(Mathf.Sign(Mathf.Sin(Time.time * PatrolSwitchRate + patrolCycleOffset)));
        }
        else if (state == AIState.Idle)
        {
            targetSpeed = 0.0f;
            if (isGrounded)
            {
                frb.Accelerate(-frb.GetVelocity() * 50f * slopeForceFactor);
            }
        }
        faceDirChangeTime += Time.fixedDeltaTime;

        targetSpeed *= GetSpeed().GetValue() * SpeedDifficultyMultiplier.GetFactor();
        if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.5f)
        {
            targetSpeed = 0.0f;
        }
        frb.Accelerate(Vector3.right * (targetSpeed * faceDir - frb.GetVelocity().x) * Acceleration * slopeForceFactor); // Accelerate toward the target

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

        if (state != AIState.Attack)
        {
            nextAim = (Target.position - GetProjectilePoint()).normalized;
        }

        projectileAim = Vector3.Slerp(projectileAim, nextAim, AimSpeed * AimDifficultyMultiplier.GetFactor() * Time.fixedDeltaTime);

        if (anim != null)
        {
            anim.SetFacing(Mathf.RoundToInt(Mathf.Sign(frb.GetVelocity().x)) == faceDir);
            anim.SetAimDirection(projectileAim.y);
            anim.SetAttacking(state == AIState.Attack || state == AIState.Charge || state == AIState.Follow);
        }

        // Projectile shooting logic
        if (state == AIState.Attack)
        {
            if (attackLaunchTime <= 0)
            {
                attackLaunchTime = AttackLaunchInterval * ShootRateDifficultyMultiplier.GetFactor();
                if (AttackProjectile != null)
                {

                    GameObject spawnedProjectile = Instantiate(AttackProjectile, GetProjectilePoint(),
                        Quaternion.LookRotation(Vector3.forward, projectileAim));
                    spawnedProjectile.transform.parent = transform;
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
      * Overrides origin setting for this enemy
      */
    protected override void UpdateOrigin()
    {
        trueOrigin = transform.position + ScaleVector3(new Vector3(Origin.x * faceDir, Origin.y, Origin.z));
    }

    /**
      * Checks if the enemy is on the ground
      */
    protected override void GroundCheck()
    {
        Ray groundRay = new Ray(transform.position + ScaleVector3(Vector3.up * groundCheckStartHeight), Vector3.down);
        RaycastHit hit = new RaycastHit();
        isGrounded = Physics.SphereCast(groundRay, ScaleFloat(groundCheckRadius), out hit, ScaleFloat(groundCheckDistance), groundCheckMask, QueryTriggerInteraction.Ignore);
        groundNormal = isGrounded ? hit.normal : Vector3.up;
        Ray forwardRay = new Ray(trueOrigin, ScaleVector3(new Vector3(ForwardEdgeRay.x * faceDir, ForwardEdgeRay.y, ForwardEdgeRay.z)));
        Ray backRay = new Ray(trueOrigin, ScaleVector3(new Vector3(BackEdgeRay.x * faceDir, BackEdgeRay.y, BackEdgeRay.z)));
        edgeInFront = ForwardEdgeRay.sqrMagnitude > 0 ? Physics.Raycast(forwardRay, ScaleFloat(ForwardEdgeRay.magnitude), groundCheckMask, QueryTriggerInteraction.Ignore) : true;
        edgeBehind = BackEdgeRay.sqrMagnitude > 0 ? Physics.Raycast(backRay, ScaleFloat(BackEdgeRay.magnitude), groundCheckMask, QueryTriggerInteraction.Ignore) : true;
    }

    public override Vector3 GetAimDirection()
    {
        return projectileAim;
    }

    public override Vector3 GetProjectilePoint()
    {
        return ProjectileSource != null ? ProjectileSource.position : transform.position + ScaleVector3(new Vector3(ProjectileStart.x * faceDir, ProjectileStart.y, ProjectileStart.z));
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + ScaleVector3(Vector3.up * groundCheckStartHeight), ScaleFloat(groundCheckRadius));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + ScaleVector3(Vector3.up * groundCheckStartHeight) + ScaleVector3(Vector3.down * groundCheckDistance), ScaleFloat(groundCheckRadius));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(trueOrigin, ScaleVector3(new Vector3(ForwardEdgeRay.x * faceDir, ForwardEdgeRay.y, ForwardEdgeRay.z)));
        Gizmos.DrawRay(trueOrigin, ScaleVector3(new Vector3(BackEdgeRay.x * faceDir, BackEdgeRay.y, BackEdgeRay.z)));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetProjectilePoint(), ScaleFloat(0.1f));
    }
}
