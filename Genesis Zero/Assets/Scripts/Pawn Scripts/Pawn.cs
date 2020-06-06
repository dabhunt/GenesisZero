using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * Pawn is a base class for entities that contain statistics, status and can take damage
 * It can handle taking damage and maintaining the statuses/statistics it contains.
 * To use the pawn class, one should extend the class and call the base function Update()
 */
public class Pawn : MonoBehaviour
{
    private Statistic health, damage, speed, attackspeed, flatdamagereduction, damagereduction, dodgechance, critchance, critdamage, range, shield, weight;
    private List<Statistic> statistics;
    public StatObject Stats;

    private Status invunerable, stunned, burning, slowed, stunimmune;
    private List<Status> statuses;

    private float burntime, burndamage, burntick, burnimmunity; //burndamage is damage per second
    private float slowtime, knockbackforce;
    private float stunImmuneAfterStun = 1.5f;
    private Vector3 knockbackvector, lastposition;
    private bool Initialized, ForcedKnockBack, Dying, StunnedLastFrame, HasAnimationEventController;
    private float dashDecay = .01f;

    protected void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (Initialized == false)
        {
            if (Stats != null)
            {
                AddStats();
            }
            else
            {
                Debug.LogError("No statistics have been assigned to " + transform.parent.name);
            }
            InitializeStatuses();

            Initialized = true;

            PawnAnimationEvents animEvents = GetComponentInChildren<PawnAnimationEvents>(true);
            if (animEvents != null)
            {
                HasAnimationEventController = true;
                animEvents.SetParentPawn(this);
            }
        }
    }

    /**
     * The function that handles damage taken. Does not handle things like burning and knockback
     */
    public virtual float TakeDamage(float amount, Pawn source)
    {
        int chance = Random.Range(0, 100);
        if (chance > GetDodgeChance().GetValue() * 100) // Dodging will ignore damage
        {
            float finaldamage = amount;
            if (invunerable.IsActive() == true)
            {
                finaldamage = 0;
            }
            finaldamage -= GetFlatDamageReduction().GetValue();
            finaldamage = finaldamage - finaldamage * GetDamageReduction().GetValue();
            finaldamage = Mathf.Clamp(finaldamage, 0, 999);

            // Shield Damage
            if (GetShield().GetValue() > 0)
            {
                print("shield thing is running" + GetShield().GetValue());
                float diff = GetShield().GetValue() - finaldamage;
                if (diff > 0)
                {
                    GetShield().AddValue(-finaldamage);
                    finaldamage = 0;
                }
                else
                {
                    GetShield().SetValue(0);
                    finaldamage = -diff;
                }
            }

            // Damage
            GetHealth().AddValue(-finaldamage);
            return finaldamage;
            //Debug.Log("Health Left:"+GetHealth().GetValue());
        }
        else
        {
            //Debug.Log(transform.root.name + " Dodged!");
            return 0;
            //Dodge Effect
        }
    }

    /**
     * Restores health to the pawn
     */
    public void Heal(float amount)
    {
        //Heal effect
        if (amount >= 1)
        {
            GameObject emit = VFXManager.instance.PlayEffect("DamageNumber", new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - .5f));
            emit.GetComponent<DamageNumber>().SetNumber(amount);
            emit.GetComponent<DamageNumber>().SetColor(new Color(0, .9f, 0));
            GetHealth().AddValue(amount);
        }
    }

    protected void Update()
    {
        UpdateStats();

        if (IsStunImmune())
        {
            GetStunnedStatus().RemoveTime();
        }

        if (IsBurning() && IsInvunerable() == false)
        {
            if (burntick >= .5f)
            {
                float damage = -burndamage * .5f;
                GetHealth().AddValue(damage);
                GameObject emit = VFXManager.instance.PlayEffect("DamageNumber", new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - .5f));
                emit.GetComponent<DamageNumber>().SetNumber(-damage);
                emit.GetComponent<DamageNumber>().SetColor(new Color(1, .35f, 0));
                burntick -= .5f;
            }
            burntime -= Time.deltaTime;
            burntick += Time.deltaTime;
			burnimmunity -= Time.deltaTime;
        }
        if (IsSlowed())
        {
            
            //slowtime -= Time.deltaTime;
        }
        //gives the pawn stun immunity after being stunned. default value is 1.5 seconds immunity
        if (StunnedLastFrame == true && !IsStunned())
        {
            StunnedLastFrame = false;
            stunimmune.SetTime(stunImmuneAfterStun);
        }
        if (IsStunned() || IsDying())
            StunnedLastFrame = true;
        if (GetHealth().GetValue() <= 0)
        {
            Die();
        }
    }

    protected void FixedUpdate()
    {
        lastposition = transform.position;

        if (knockbackforce > 0)
        {
            if (knockbackforce > 6)
            {
                GetStunnedStatus().AddTime(Time.fixedDeltaTime);
            }
            if (!ForcedKnockBack)
                knockbackforce *= Mathf.Clamp(9.5f / GetWeight().GetValue(), 0, .99f); //only decrease velocity for non players
            else
            {
                knockbackforce *= Mathf.Clamp(1-dashDecay, 0, 1-dashDecay);
                dashDecay *= 1.2f; //decay rate increases by 20% each frame
                Vector3 translation = knockbackvector.normalized * knockbackforce * Time.fixedDeltaTime;
                bool colliding = false;
                if (GetComponentInChildren<CapsuleCollider>() && GetComponent<Hurtbox>())
                {
                    RaycastHit hit;
                    CapsuleCollider cc = GetComponentInChildren<CapsuleCollider>();
                    //Debug.Log(cc);
                    Vector3 p1 = cc.center + Vector3.up * -cc.height / 2;
                    Vector3 p2 = p1 + Vector3.up * cc.height;
                    //colliding = Physics.CapsuleCast(p1, p2, cc.radius, knockbackvector, out hit, translation.magnitude);
                    colliding = Physics.Raycast(Player.instance.CenterPoint(), knockbackvector, out hit, translation.magnitude, GetComponentInParent<PlayerController>().rollingLayerMask);
                    if (colliding && hit.collider.isTrigger == false)
                    {
                        //Debug.Log(colliding);
                        //print(hit.collider);
                        colliding = !hit.collider.isTrigger;

                        StopCollision(hit.normal);
                    }
                    translation = colliding ? -translation : translation;
                }
                if (GetComponentInParent<PlayerController>() && colliding == false)
                {
                    transform.position += translation;
                }
                else if (GetComponentInParent<PlayerController>() && colliding == true)
                {
                    transform.position -= translation * 2;
                }
            }
            if (knockbackforce < 1 || dashDecay >= 1)
            {
                ForcedKnockBack = false;
                knockbackforce = 0;
            }

        }
    }

    public void StopCollision(Vector3 normal)
    {
        GetComponentInParent<Pawn>().KnockBackForced(-GetComponentInParent<Pawn>().GetKnockBackVector(), GetComponentInParent<Pawn>().GetKnockBackForce() / 2);
        normal = (normal * 1);
        GetComponentInParent<Pawn>().transform.position += normal / 2;

        //collisions.Add(collision.gameObject);
        //resettime = Time.fixedDeltaTime * 1;
    }

    public void UpdateStats()
    {
        foreach (Statistic stat in statistics)
        {
            stat.UpdateStatistics();
        }
        foreach (Status status in statuses)
        {
            status.UpdateStatus();
        }

    }

    // ------------------------- ACCESS FUNCTIONS ------------------------------//
    public Statistic GetHealth()
    {
        return health;
    }

    public Statistic GetDamage()
    {
        return damage;
    }

    public Statistic GetSpeed()
    {
        return speed;
    }

    public Statistic GetAttackSpeed()
    {
        return attackspeed;
    }

    public Statistic GetFlatDamageReduction()
    {
        return flatdamagereduction;
    }

    public Statistic GetDamageReduction()
    {
        return damagereduction;
    }

    public Statistic GetDodgeChance()
    {
        return dodgechance;
    }

    public Statistic GetCritChance()
    {
        return critchance;
    }

    public Statistic GetCritDamage()
    {
        return critdamage;
    }

    public Statistic GetRange()
    {
        return range;
    }

    public Statistic GetShield()
    {
        return shield;
    }

    public Statistic GetWeight()
    {
        return weight;
    }
    public bool IsStunned()
    {
        return stunned.IsTrue();
    }

    public Status GetStunnedStatus()
    {
        return stunned;
    }

    public bool IsInvunerable()
    {
        return invunerable.IsTrue();
    }

    public void SetInvunerable(float time)
    {
        invunerable.SetTime(time);
    }

    public Status GetInvunerableStatus()
    {
        return invunerable;
    }
    public Status GetBurningStatus()
    {
        return burning;
    }
    public bool IsBurning()
    {
        return burning.IsTrue();
    }

    public void Burn(float time, float damage)
    {
		if (IsInvunerable()) //Does not inflict burn if the pawn is invunerable
		{
			return;
		}
		bool replace = damage >= burndamage && time >= burntime;

		if (replace)
		{
			burntime = time;		
			burndamage = damage;
		}
        burntick = Time.deltaTime;
        burning.SetTime(time);
        GameObject burnemit = VFXManager.instance.PlayEffectForDuration("VFX_BurnEffect", transform.position, burntime);
        burnemit.transform.parent = transform;
        burnemit.transform.localScale = new Vector3(1, 1, 1);
		burnimmunity = Time.deltaTime;

    }

	public Vector2 GetBurnStats()
	{
		return new Vector2(burndamage, burntime);
	}

	public float GetBurnImmunity()
	{
		return burnimmunity;
	}

    public void KnockBack(Vector3 direction, float force)
    {
        //Debug.Log(direction+" "+force);
        knockbackvector = direction;
        knockbackforce = force;
        knockbackforce *= Mathf.Clamp(9.5f / GetWeight().GetValue(), 0, .99f);
        float knockback = Mathf.Clamp(knockbackforce, 0, 100f);
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().AddForce(knockbackvector.normalized * knockback, ForceMode.Impulse);
        }
        if (GetComponent<Player>())
        {
            ForcedKnockBack = true;
        }
    }
    //can be used to change the default amount of seconds a pawn is immune to stuns after being stunned. the default is 1.5 seconds
    public void SetDefaultStunImmune(float val)
    {
        stunImmuneAfterStun = val;
    }
    public void KnockBackForced(Vector3 direction, float force)
    {
        //Invoke("EndDash", force / 65); //ex: if force is 25, cancels in 1 second
        dashDecay = .01f;
        KnockBack(direction, force);
    }

    public void SetKnockBackForce(float force)
    {
        knockbackforce = force;
    }

    public Vector3 GetKnockBackVector()
    {
        return knockbackvector;
    }

    public float GetKnockBackForce()
    {
        return knockbackforce;
    }

    public void SetForcedKnockBack(bool boolean)
    {
        ForcedKnockBack = boolean;
    }

    public bool IsForcedKnockBack()
    {
        return ForcedKnockBack;
    }

    public bool IsSlowed()
    {
        return slowed.IsTrue();
    }

    public Status GetSlowedStatus()
    {
        return slowed;
    }

    public void Slow(float factor, float time)
    {
        speed.AddRepeatingBonus(-GetSpeed().GetMaxValue() * factor, -GetSpeed().GetMaxValue() * factor, 1.5f, "FT_slow");
        GameObject burnemit = VFXManager.instance.PlayEffectForDuration("VFX_Health", transform.position, time);
        burnemit.transform.parent = transform;
        burnemit.transform.localScale = new Vector3(1, 1, 1);
    }

    public bool IsStunImmune()
    {
        return stunimmune.IsTrue();
    }
    public bool IsDying()
    {
        return Dying;
    }
    public Status GetStunImmuneStatus()
    {
        return stunimmune;
    }

    public Vector3 GetLastPosition()
    {
        return lastposition;
    }

    // ------------------------- ---------------- ------------------------------//

    // ------------------------ General Functions ------------------------------//
    public Vector3 Position()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Translate(Vector3 translation)
    {
        transform.position += translation;
    }

    // ------------------------- ---------------- ------------------------------//

    private void AddStats()
    {
        statistics = new List<Statistic>();
        health = new Statistic(Stats.health); statistics.Add(health);
        damage = new Statistic(Stats.damage); statistics.Add(damage);
        speed = new Statistic(Stats.speed); statistics.Add(speed);
        attackspeed = new Statistic(Stats.attackspeed); statistics.Add(attackspeed);
        flatdamagereduction = new Statistic(Stats.flatdamagereduction); statistics.Add(flatdamagereduction);
        damagereduction = new Statistic(Stats.damagereduction); statistics.Add(damagereduction);
        dodgechance = new Statistic(Stats.dodgechance); statistics.Add(dodgechance);
        critchance = new Statistic(Stats.critchance); statistics.Add(critchance);
        critdamage = new Statistic(Stats.critdamage); statistics.Add(critdamage);
        range = new Statistic(Stats.range); statistics.Add(range);
        shield = new Statistic(Stats.shield); statistics.Add(shield);
        weight = new Statistic(Stats.weight); statistics.Add(weight);
    }

    private void InitializeStatuses()
    {
        statuses = new List<Status>();
        invunerable = new Status(0); statuses.Add(invunerable);
        stunned = new Status(0); statuses.Add(stunned);
        burning = new Status(0); statuses.Add(burning);
        slowed = new Status(0); statuses.Add(slowed);
        stunimmune = new Status(0); statuses.Add(stunimmune);
    }

    private void Die()
    {
        if (!Dying)
        {
            if (!GetComponent<Player>() && gameObject.tag == "Enemy")
            {
                GameObject playerSearch = Player.instance.gameObject;
                if (playerSearch != null)
                {
                    Player playerComponent = playerSearch.GetComponent<Player>();
                    if (playerComponent != null)
                    {
                        playerComponent.TriggerEffectOnKill();
                    }
                }
            }
            //Allow the animator to control when the object is destroyed
            if (!HasAnimationEventController)
            {
                StartCoroutine(DeathSequence());
            }

            if (this is AIController)
            {
                StateManager.instance.RecursiveLayerChange(this.transform, "Dead");
                GetComponent<AIController>().DeathEvent.Invoke();
                //Collider[] cols = this.GetComponentsInChildren<Collider>();
                //for (int i = 0; i < cols.Length; i++)
                //{
                //    //cols[i].isTrigger = true;
                //}
            }
        }
    }

    private IEnumerator DeathSequence()
    {
        Dying = true;
        stunned.SetTime(999f);
        if (GetComponent<Hurtbox>() != null)
            Destroy(GetComponent<Hurtbox>());
        if (GetComponentInChildren<Hurtbox>() != null)
            Destroy(GetComponentInChildren<Hurtbox>());
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1);
        yield return new WaitForSeconds(Stats.deathDuration);
        Destroy(this.gameObject);
    }

}
