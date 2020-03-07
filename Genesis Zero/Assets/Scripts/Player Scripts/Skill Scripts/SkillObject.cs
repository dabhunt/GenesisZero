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
    [Tooltip("0 - Ability, 1 - White, 2 - Green, 3 - Gold")]
    public int Rarity = 1;

    [Space]
    public float health;
    public float damage;
    public float speed;
    public float attackspeed;
    public float flatdamagereduction;
    public float damagereduction;
    public float dodgechance;
    public float critchance;
    public float critdamage;
    public float range;
    public float shield;
    public float weight;

    public Color GetColor()
    {
        Color color;
        switch (Rarity)
        {
            case 2:
                color = new Color(0.618f, 0f, 1f);
                break;
            case 3:
                color = new Color(1, 0.83f, 0);
                break;
            default:
                color = Color.white;
                break;
        }
        return color;
    }
}
