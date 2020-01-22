using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    private Statistic health, damage, speed, attackspeed, flatdamagereduction, damagereduction, dodgechance, critchance;
    private List<Statistic> statistics;
    public StatObject stats;

    private Status invunerable, stunned, burning, slowed;
    private List<Status> statuses;

    private float burntime, burndamage; //burndamage is damage per second
    private float slowtime;

    protected void Start()
    {
        if (stats != null)
        {
            AddStats();
        }
        else
        {
            Debug.LogError("No statistics have been assigned to " + transform.parent.name);
        }

        InitializeStatuses();
    }

    /**
     * The function that handles damage taken. Does not handle things like burning and knockback
     */
    public void TakeDamage(float amount)
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

            // Damage
            GetHealth().AddValue(-finaldamage);
            //Debug.Log("Health Left:"+GetHealth().GetValue());
        }
        else
        {
            Debug.Log(transform.root.name + " Dodged!");
            //Dodge Effect
        }
    }

    /**
     * Restores health to the pawn
     */
    public void Heal(float amount)
    {
        //Heal effect
        GetHealth().AddValue(amount);
    }

    protected void Update()
    {
        foreach (Statistic stat in statistics)
        {
            stat.UpdateStatistics();
        }
        foreach (Status status in statuses)
        {
            status.UpdateStatus();
        }



        if (IsBurning())
        {
            GetHealth().AddValue(-burndamage * Time.deltaTime);
            burntime -= Time.deltaTime;
        }
        if (IsSlowed())
        {
            speed.AddBonus(-GetSpeed().GetMaxValue() * GetSlowedStatus().GetFactor(), Time.deltaTime);
            slowtime -= Time.deltaTime;
        }

        if (GetHealth().GetValue() <= 0)
        {
            Destroy(this.gameObject);
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

    public bool IsBurning()
    {
        return burning.IsTrue();
    }

    public void Burn(float time, float damage)
    {
        burntime = time;
        burndamage = damage;
        burning.SetTime(time);
    }

    public bool IsSlowed()
    {
        return slowed.IsTrue();
    }

    public Status GetSlowedStatus()
    {
        return slowed;
    }

    public void Slow(float factor, float time){
        slowed.SetFactor(factor);
        slowtime = time;
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
        health = new Statistic(stats.health); statistics.Add(health);
        damage = new Statistic(stats.damage); statistics.Add(damage);
        speed = new Statistic(stats.speed); statistics.Add(speed);
        attackspeed = new Statistic(stats.attackspeed); statistics.Add(attackspeed);
        flatdamagereduction = new Statistic(stats.flatdamagereduction); statistics.Add(flatdamagereduction);
        damagereduction = new Statistic(stats.damagereduction); statistics.Add(damagereduction);
        dodgechance = new Statistic(stats.dodgechance); statistics.Add(dodgechance);
        critchance = new Statistic(stats.critchance); statistics.Add(critchance);
    }

    private void InitializeStatuses()
    {
        statuses = new List<Status>();
        invunerable = new Status(0); statuses.Add(invunerable);
        stunned = new Status(0); statuses.Add(stunned);
        burning = new Status(0); statuses.Add(burning);
        slowed = new Status(0); statuses.Add(slowed);
    }

}
