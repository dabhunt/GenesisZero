﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPickup : MonoBehaviour
{
    [Header("Mod or Ability")]
    public SkillObject skill;
    private bool added;
    // Start is called before the first frame update
    void Start()
    {
        if (skill == null)
        {
            // Add default skill
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>() || other.GetComponent<Player>())
        {
            Player p = other.GetComponent<Player>();
            if (other.GetComponentInParent<Player>())
            {
                p = other.GetComponentInParent<Player>();
            }

            if (skill != null && added == false)
            {
                //Check if it's an ability, cant have more than one
                if (skill.IsAbility && p.GetSkillManager().HasSkill(skill.name))
                {
                    // Pop-up prompt UI stuff for when there are dupicates
                }
                else
                {
                    // There are more than two abilities and a new ability is here
                    if (skill.IsAbility && !p.GetSkillManager().HasSkill(skill.name) && p.GetSkillManager().GetAbilityAmount() >= 2)
                    {
                        // Prompt choice to swap out with current abilities
                    }
                    else
                    {
                        p.GetSkillManager().AddSkill(skill);
                        added = true;
                    }
                }
            }

            if (added == true)
            {
                Destroy(gameObject);
            }

        }
    }
}
