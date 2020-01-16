using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pawn/Create Statistic")]
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

}
