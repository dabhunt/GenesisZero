using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class SpartanLaser : MonoBehaviour
{
    private Player p;
    public void Start()
    {
        GetComponent<Hitbox>().killDelegate += CheckHit;
        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    // Update is called once per frame
    void CheckHit()
    {
        
        if (p.GetSkillManager().GetAbility1().name == "Culling Blast")
        {
            p.GetComponent<AbilityCasting>().ResetAbilityCooldown1();
            p.GetComponent<UniqueEffects>().IncrementKillCount();
        }
        else if (p.GetSkillManager().GetAbility2().name == "Culling Blast")
        {
            p.GetComponent<AbilityCasting>().ResetAbilityCooldown2();
            p.GetComponent<UniqueEffects>().IncrementKillCount();
        }

    }
}
