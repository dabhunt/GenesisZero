using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Kenny Doan
 */
[CreateAssetMenu(menuName = "Player/Create Skill")]
public class SkillObject : ScriptableObject
{
    public Sprite Icon;

    [TextArea]
    public string Description;

    public bool IsAbility;

    public float health;
    public float damage;
    public float speed;
    public float attackspeed;
    public float flatdamagereduction;
    public float damagereduction;
    public float dodgechance;
    public float critchance;
}
