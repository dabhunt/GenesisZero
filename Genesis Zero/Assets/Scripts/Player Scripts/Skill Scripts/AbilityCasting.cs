using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class AbilityCasting : MonoBehaviour
{
    Player player;
    SkillManager skillmanager;

    private float AbitityCasttime1;
    private float AbilityCooldown1;
    private float TotalAbitityCasttime1;
    private float TotalAbilityCooldown1;

    private float AbitityCasttime2;
    private float AbilityCooldown2;
    private float TotalAbitityCasttime2;
    private float TotalAbilityCooldown2;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        skillmanager = player.GetSkillManager();
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void CastAbility1()
    {
        if (CanCastAbility1())
        {
            CastAbility(skillmanager.GetAbility1().name, 1);
        }
    }

    public void CastAbility2()
    {
        if (CanCastAbility2())
        {
            CastAbility(skillmanager.GetAbility2().name, 2);
        }
    }

    private void CastAbility(string name, int num)
    {
        switch (name)
        {
            case "Ability1":
                InitializeAbility(10, 1, num);
                //Implement functionality
                break;
            case "Ability2":
                //Implement functionality
                break;
            case "Ability3":
                //Implement functionality
                break;
            case "Ability4":
                //Implement functionality
                break;
        }
    }

    private bool CanCastAbility1()
    {
        return (AbitityCasttime1 <= 0 && AbilityCooldown1 <= 0);
    }

    private bool CanCastAbility2()
    {
        return (AbitityCasttime2 <= 0 && AbilityCooldown2 <= 0);
    }

    private void InitializeAbility(float cooldown, float casttime, int num)
    {
        if (num == 1)
        {
            InitializeAbility1(cooldown, casttime);
        }
        else
        {
            InitializeAbility2(cooldown, casttime);
        }
    }

    private void InitializeAbility1(float cooldown, float casttime)
    {
        AbilityCooldown1 = cooldown;
        TotalAbilityCooldown1 = cooldown;
        AbitityCasttime1 = casttime;
        TotalAbitityCasttime1 = casttime;
    }

    private void InitializeAbility2(float cooldown, float casttime)
    {
        AbilityCooldown2 = cooldown;
        TotalAbilityCooldown2 = cooldown;
        AbitityCasttime2 = casttime;
        TotalAbitityCasttime2 = casttime;
    }

    private void UpdateAbilities()
    {
        AbitityCasttime1 -= Time.deltaTime;
        AbilityCooldown1 -= Time.deltaTime;

        AbitityCasttime2 -= Time.deltaTime;
        AbilityCooldown2 -= Time.deltaTime;
    }

    public float GetCooldownRatio1()
    {
        return AbilityCooldown1 / TotalAbilityCooldown1;
    }

    public float GetCooldownRatio2()
    {
        return AbilityCooldown2 / TotalAbilityCooldown2;
    }

}
