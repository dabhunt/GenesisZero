using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn: MonoBehaviour
{
    [SerializeField]
    Statistic health, damage, speed, flatdamagereduction, damagereduction, dodgechance;

    public StatObject Stats;

    protected void Start()
    {
        if (Stats != null)
        {
            health = new Statistic(Stats.health);
            damage = new Statistic(Stats.damage);
            speed = new Statistic(Stats.speed);
        }
        else
        {
            Debug.LogError("No statistics have been assigned to " + transform.parent.name);
        }
    }

    public void TakeDamage(float amount, GameObject source)
    {

    }

    protected void Update()
    {
        Debug.Log("Pawn");
    }
}
