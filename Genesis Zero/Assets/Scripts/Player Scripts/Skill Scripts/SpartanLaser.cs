using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class SpartanLaser : MonoBehaviour
{
    // Update is called once per frame
    void CheckHit()
    {
        if (true)
        {
            Debug.Log("Kill");
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
}
