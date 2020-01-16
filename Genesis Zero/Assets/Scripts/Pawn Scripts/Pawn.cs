using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    private Statistic health, damage, speed, attackspeed, flatdamagereduction, damagereduction, dodgechance, critchance;
    private List<Statistic> statistics;
    public StatObject Stats;

    private Status invunerable, stunned, burning, slowed;
    private List<Status> statuses;

    protected void Start()
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
    }

    /**
     * The function that handles damage taken. Does not handle things like burning and knockback
     */
    public void TakeDamage(float amount, GameObject source)
    {
        if (Random.Range(0, 100) > GetDodgeChance().GetValue() * 100) // Not invunerable, will take the damage
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
        }
        else
        {
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
    }

    private void InitializeStatuses()
    {
        statuses = new List<Status>();
        statuses.Add(invunerable);
        statuses.Add(stunned);
        statuses.Add(burning);
        statuses.Add(slowed);
    }

}
