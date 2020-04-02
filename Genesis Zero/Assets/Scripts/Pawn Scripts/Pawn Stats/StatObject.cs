using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pawn/Create Statistic")]
/**
 * Kenny Doan
 * ScriptableObject that contains data on what the pawn's initial stats are.
 * Add a Statobject to the script that extends the Pawn Script
 */
public class StatObject : ScriptableObject
{
    public float health;
    public float damage;
    public float speed;
    public float attackspeed;
    public float flatdamagereduction;
    public float damagereduction;
    public float dodgechance;
    public float critchance;
    public float critdamage = 2;
    public float range = 1;
    public float shield;
    public float weight = 10;
    public float deathDuration = 3;
}
