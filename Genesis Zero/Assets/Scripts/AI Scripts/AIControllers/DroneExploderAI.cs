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
    public float blastRadius = 3.5f;
    public float scaleCorrector = 3.5f;
    public float lerpMultiplier = 2.3f;
    public float startScale = .01f;

    [Header("Attack")]
    public ParticleSystem chargeParticles;
    public ParticleSystem attackParticles;
    public GameObject explosionPrefab;

    [Header("Difficulty")]
    public DifficultyMultiplier SpeedDifficultyMultiplier;
    public DifficultyMultiplier RotationDifficultyMultiplier;

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

        targetSpeed *= GetSpeed().GetValue() * SpeedDifficultyMultiplier.GetFactor();
        frb.Accelerate(transform.up * (targetSpeed - frb.GetVelocity().magnitude * Mathf.Clamp01(Vector3.Dot(transform.up, frb.GetVelocity().normalized))) * Acceleration); // Accelerate toward the target
        frb.Accelerate(-transform.right * frb.GetVelocity().magnitude * Vector3.Dot(transform.right, frb.GetVelocity().normalized) * SideDecel); // Deceleration to prevent sideways movement
        transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir); // Actual rotation

        CheckWalls();
        if (isCloseToWall)
        {
            frb.Accelerate((transform.position - wallPoint).normalized * (1.0f - (transform.position - wallPoint).magnitude / WallCheckDistance) * WallAvoidForce);
        }

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
           
            GameObject emit = VFXManager.instance.PlayEffect(vfxName, transform.position, 0f, ScaleFloat(blastRadius / scaleCorrector));
            spawnedExplosion.GetComponent<ProjectileTest>().DestroyEvent.AddListener(DestroySelf);
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayRandomSFXType("EnemyHit");
                AudioManager.instance.PlaySound("SFX_ExplosionEnemy", 1.7f, 1f);
            }
            // Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y,0);
            // //this is for collision, not VFX
            // GameObject spawnedExplosion = Instantiate(explosionPrefab,spawnPoint, Quaternion.identity);
            // AOE AOEscript = spawnedExplosion.GetComponent<AOE>();
            // AOEscript.setScaleTarget(startScale, blastRadius, lerpMultiplier);
            // //print("blastRadius = " + blastRadius + "during the on destroy call last section");
            // 
        }
        Destroy(this.GetComponent<SpawnOnDestroy>());
        Destroy(this.gameObject);
    }

    /*
     * Wrapper function to destroy self with no parameters.
     */
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    /**
     * Checks for closeness to walls in order to avoid them
     */
    protected void CheckWalls()
    {
        if (!wallCheckCycleInProgress && gameObject.activeInHierarchy)
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
