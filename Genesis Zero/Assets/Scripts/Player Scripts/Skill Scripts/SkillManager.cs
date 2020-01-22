﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    Dictionary<string, int> Skills;

    private List<SkillObject> skillobjects;
    private Player player;
    public SkillManager(Player p)
    {
        player = p;
    }

    /**
     * Adds stats to the player using the data in the SkillObject.
     * 
     */
    public void AddSkill(SkillObject skill)
    {
        if (Skills.ContainsKey(skill.name)) // Adds to the stack of skills
        {
            Skills[skill.name] += 1;
        }
        else
        {
            skillobjects.Add(skill);
            Skills.Add(skill.name, 1);
        }

        AddSkillStats(skill, true);
    }

    /**
     * Removes a stack of skill
     */
    public void RemoveSkill(SkillObject skill)
    {
        if (Skills.ContainsKey(skill.name)) // Removes the skill from the dictionary
        {
            Skills[skill.name] -= 1;
            if (Skills[skill.name] <= 0)
            {
                Skills.Remove(skill.name);
            }
            AddSkillStats(skill, false);
        }
    }

    /**
     * Adds the stats to the player. If Adding is true
     * Then it is a positive addition, otherwise it is negative
     */
    private void AddSkillStats(SkillObject skill, bool IsAdding)
    {
        int multi = IsAdding ? 1 : -1;
        player.GetHealth().AddMaxValue(skill.health * multi);
        player.GetDamage().AddMaxValue(skill.damage * multi);
        player.GetSpeed().AddMaxValue(skill.speed * multi);
        player.GetAttackSpeed().AddMaxValue(skill.attackspeed * multi);
        player.GetFlatDamageReduction().AddMaxValue(skill.flatdamagereduction * multi);
        player.GetDamageReduction().AddMaxValue(skill.damagereduction * multi);
        player.GetDodgeChance().AddMaxValue(skill.dodgechance * multi);
        player.GetCritChance().AddMaxValue(skill.critchance * multi);
    }

    /**
     * Returns whether or not the SkillManager contains the skill
     */
    public bool HasSkill(string name)
    {
        return Skills.ContainsKey(name);
    }

    public SkillObject GetRandomSkill()
    {
        Object[] skills = Resources.LoadAll("Skills");
        SkillObject skill = (SkillObject)skills[Random.Range(0, skills.Length)];
        //Debug.Log(skill.name);
        return skill;
    }

    public SkillObject GetRandomAbility()
    {
        Object[] skills = Resources.LoadAll("Skills/Abilities");
        SkillObject skill = (SkillObject)skills[Random.Range(0, skills.Length)];
        //Debug.Log(skill.name);
        return skill;
    }


}
