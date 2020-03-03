using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class SpartanLaser : MonoBehaviour
{
    public void Start()
    {
        Hitbox.killDelegate += CheckHit;
    }
    // Update is called once per frame
    void CheckHit()
    {
        Player p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (p.GetSkillManager().GetAbility1().name == "Culling Blast")
        {
            p.GetComponent<AbilityCasting>().ResetAbilityCooldown1();
        }
        else if (p.GetSkillManager().GetAbility2().name == "Culling Blast")
        {
            p.GetComponent<AbilityCasting>().ResetAbilityCooldown2();
        }

    }
}
