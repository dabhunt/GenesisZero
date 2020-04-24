using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FakeRigidbody))]
/**
 * Justin Couch
 * ShieldEnemyAI is the class representing the shield enemy type.
 */
public class ShieldEnemyAI : AIController
{
    protected FakeRigidbody frb;

    [Header("Movement")]
    public float MoveSpeed = 10f; // Maximum movement speed
    public float LungeSpeed = 20f; // Lunging attack speed
    public float LungeVerticality = 0.5f; // Amount to jump vertically for horizontal lunges
    private float targetSpeed = 0.0f;
    public float Acceleration = 5.0f; // Rate of acceleration

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
    //public Vector3 BackEdgeRay;
    private bool edgeInFront = true;
    public LayerMask groundCheckMask;

    [Header("Attack")]
    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;
    public GameObject AttackHitbox;
    public Vector3 AttackHitboxStart = Vector3.zero;
    private Transform spawnedHitboxObj; // Refernce to currently spawned hitbox object

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
            tracker.GiveUpCondition = () =>
            {
                return tracker.PeekFirstPoint().y > transform.position.y + MaxFollowHeight;
            };
        }
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target == null) { return; }

        float slopeForceFactor = Vector3.Dot(groundNormal, Vector3.left * faceDir) + 1.0f; // Adjust movement force based on slope steepness

        if (state == AIState.Follow || state == AIState.Charge)
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
        }
        else if (state == AIState.Attack || state == AIState.Cooldown)
        {
            targetSpeed = 0.0f;
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
        if ((isGrounded && state != AIState.Attack) || state == AIState.Cooldown)
        {
            frb.Accelerate(Vector3.right * (targetSpeed * faceDir - frb.GetVelocity().x) * Acceleration * slopeForceFactor); // Accelerate toward the target
        }

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
    }

    /**
     * Overrides origin setting for this enemy
     */
    protected override void UpdateOrigin()
    {
        trueOrigin = transform.position + new Vector3(Origin.x * faceDir, Origin.y, Origin.z);
    }

    /**
     * Checks if the enemy is on the ground
     */
    protected override void GroundCheck()
    {
        Ray groundRay = new Ray(transform.position + Vector3.up * groundCheckStartHeight, Vector3.down);
        RaycastHit hit = new RaycastHit();
        isGrounded = Physics.SphereCast(groundRay, groundCheckRadius, out hit, groundCheckDistance, groundCheckMask, QueryTriggerInteraction.Ignore);
        groundNormal = isGrounded ? hit.normal : Vector3.up;
        Ray forwardRay = new Ray(trueOrigin, new Vector3(ForwardEdgeRay.x * faceDir, ForwardEdgeRay.y, ForwardEdgeRay.z));
        //Ray backRay = new Ray(trueOrigin, new Vector3(BackEdgeRay.x * faceDir, BackEdgeRay.y, BackEdgeRay.z));
        edgeInFront = ForwardEdgeRay.sqrMagnitude > 0 ? Physics.Raycast(forwardRay, ForwardEdgeRay.magnitude, groundCheckMask, QueryTriggerInteraction.Ignore) : true;
    }

    /**
     * This initiates the lunge toward the target while attacking
     */
    public void JumpTowardTarget(AIState state)
    {
        if (state == AIState.Attack)
        {
            Vector3 normalizedTargetDir = (Target.position - trueOrigin).normalized;
            //Vector3 lungeDir = (Vector3.right * faceDir + Vector3.up * Mathf.Abs(Vector3.Dot(normalizedTargetDir, Vector3.right)) * LungeVerticality).normalized;

            //if (Vector3.Dot(normalizedTargetDir, Vector3.up) > 0)
            //{
            Vector3 lungeDir = (normalizedTargetDir + Vector3.up * Mathf.Abs(Vector3.Dot(normalizedTargetDir, Vector3.right)) * LungeVerticality).normalized;
            //}
            frb.AddVelocity(lungeDir * LungeSpeed);
            SpawnAttackHitbox();
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
            spawnedHitboxObj.localPosition = AttackHitboxStart;
            spawnedHitboxObj.localRotation = Quaternion.identity;
            Hitbox spawnedHitbox = spawnedHitboxObj.GetComponent<Hitbox>();
            spawnedHitbox.DirectionalKnockback = true;
            spawnedHitbox.Knockbackforce = 20f;
            spawnedHitbox.InitializeHitbox(GetDamage().GetValue(), this);
            spawnedHitbox.SetLifeTime(1);
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

    protected void OnDrawGizmosSelected()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * groundCheckStartHeight, groundCheckRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * groundCheckStartHeight + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(trueOrigin, new Vector3(ForwardEdgeRay.x * faceDir, ForwardEdgeRay.y, ForwardEdgeRay.z));
        //Gizmos.DrawRay(trueOrigin, new Vector3(BackEdgeRay.x * faceDir, BackEdgeRay.y, BackEdgeRay.z));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(AttackHitboxStart.x * faceDir, AttackHitboxStart.y, AttackHitboxStart.z), 0.1f);
    }
}
