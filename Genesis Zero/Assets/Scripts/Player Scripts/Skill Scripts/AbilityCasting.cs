using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player), typeof(PlayerController))]
public class AbilityCasting : MonoBehaviour
{
    Player player;
    PlayerController PC;
    SkillManager skillmanager;

    [Header("Time Dilation Ability")]
    public float timeScale = .5f;
    public float TS_effectDuration = 3.2f;
    [Header("Multi Shot Ability (Active)")]
    //how long the effect lasts
    public float MS_ActiveTime = 5f;
    public float MS_Cooldown = 8f;
    [Header("Manic Titan Ability (Active)")]
    //how long the effect lasts
    public float MT_ActiveTime = 4;
    public float MT_Cooldown = 13f;
    //1 = 100% more attack speed, based on base attack speed
    public float MT_AttackSpeedBoost = 1f;
    public float MT_CritBoost = .3f;
    [Header("Spartan Laser")]
    //each successful kill makes the laser 20% larger
    public float scaleMultiPerKill = 1.2f;
    [Header("FireDash")]
    public LayerMask ignoreEnemiesLayerMask;
    public float FD_duration = .5f;
    public float FD_gravityReplacement = .9f;
    private float AbilityCasttime1;
    private float AbilityCooldown1;
    private float TotalAbilityCasttime1;
    private float TotalAbilityCooldown1;
    private float ActiveTime1;

    
    private float AbilityCasttime2;
    private float AbilityCooldown2;
    private float TotalAbilityCasttime2;
    private float TotalAbilityCooldown2;
    private float ActiveTime2;
    public GameObject AbilityCooldownPanel;
    private AbilityCD ui;
    private Vector2 aimDir;
    private Vector2 WorldXhair;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        skillmanager = player.GetSkillManager();
        PC = GetComponent<PlayerController>();
        ui = AbilityCooldownPanel.GetComponent<AbilityCD>();
        aimDir = new Vector2(0, 0);
        
    }
    // Update is called once per frame
    void Update()
    {
        aimDir = PC.worldXhair.transform.position - transform.position;
        WorldXhair = PC.worldXhair.transform.position;
        UpdateAbilities();
    }
    public void CastAbility1()
    {
        if (CanCastAbility1())
        {
            CastAbility(skillmanager.GetAbility1().name, 1);
            ui.Cast(0);
        }
    }

    public void CastAbility2()
    {
        if (CanCastAbility2())
        {
            CastAbility(skillmanager.GetAbility2().name, 2);
            ui.Cast(1);
        }

    }

    public void CastUI(int num)
    {
        ui.Cast(num);
    }

    private void CastAbility(string name, int num)
    {
        GetComponent<UniqueEffects>().AbilityTrigger();
        switch (name)
        {
            case "Pulse Burst":
                InitializeAbility(3, 0, 0, num);
                CastPulseBurst();
                break;
            case "Burst Charge":
                InitializeAbility(3, 0,0, num);
                CastBurstCharge();
                break;
            case "Overdrive":
                InitializeAbility(20, 0,0, num);
                CastOverdrive(num);
                break;
            case "Time Dilation":
                InitializeAbility(10, 0,0, num);
                CastSlowDown();
                break;
            case "Culling Blast":
                InitializeAbility(7, 0,0, num);
                CastSpartanLaser();
                break;
            case "Wound Sealant":
                InitializeAbility(100, 0,0, num);
                CastWoundSealant();
                break;
            case "Atom Splitter":
                //(Cooldown, Casttime, ActiveTime, Abilitynum)
                //NOTE: Cooldown does not start going down until the Active wears off
                InitializeAbility(MS_Cooldown, 0, MS_ActiveTime, num);
                CastMultiShot();
                break;
            case "Heat Vent Shield":
                if (player.GetOverHeat().GetHeat() < 1)
                    return; //prevent player from using shield at 0 heat, since it does nothing
                InitializeAbility(8, 0, 4, num);
                CastHeatShield(num);
                break;
            case "Fire Dash":
                InitializeAbility(3, 0, 0, num);
                CastFireDash();
                break;
            case "Singularity":
                InitializeAbility(12, 0, 0, num);
                CastSingularity();
                break;
            case "Manic Titan":
                InitializeAbility(MT_Cooldown, 0, MT_ActiveTime, num);
                CastManicTitan();
                break;
        }
        GetComponent<UniqueEffects>().AfterAbilityTrigger();
    }

    private bool CanCastAbility1()
    {
        return (AbilityCasttime1 <= 0 && AbilityCooldown1 <= 0 && skillmanager.GetAbility1() != null);
    }

    private bool CanCastAbility2()
    {
        return (AbilityCasttime2 <= 0 && AbilityCooldown2 <= 0 && skillmanager.GetAbility2() != null);
    }

    private void InitializeAbility(float cooldown, float casttime, float activeTime, int num)
    {
        if (num == 1)
        {
            InitializeAbility1(cooldown, casttime, activeTime);
        }
        else
        {
            InitializeAbility2(cooldown, casttime, activeTime);
        }
    }

    private void InitializeAbility1(float cooldown, float casttime, float activeTime)
    {
        AbilityCooldown1 = cooldown;
        TotalAbilityCooldown1 = cooldown;
        AbilityCasttime1 = casttime;
        TotalAbilityCasttime1 = casttime;
        ActiveTime1 = activeTime;
    }

    private void InitializeAbility2(float cooldown, float casttime, float activeTime)
    {
        AbilityCooldown2 = cooldown;
        TotalAbilityCooldown2 = cooldown;
        AbilityCasttime2 = casttime;
        TotalAbilityCasttime2 = casttime;
        ActiveTime2 = activeTime;
    }

    private void UpdateAbilities()
    {
        if (ActiveTime1 <= 0)
        {
            AbilityCasttime1 = Mathf.Clamp(AbilityCasttime1 -= Time.deltaTime, 0, TotalAbilityCasttime1);
            AbilityCooldown1 = Mathf.Clamp(AbilityCooldown1 -= Time.deltaTime, 0, TotalAbilityCooldown1);
        }
        if (ActiveTime2 <= 0)
        {
            AbilityCasttime2 = Mathf.Clamp(AbilityCasttime2 -= Time.deltaTime, 0, TotalAbilityCasttime2);
            AbilityCooldown2 = Mathf.Clamp(AbilityCooldown2 -= Time.deltaTime, 0, TotalAbilityCooldown2);
        }

        ActiveTime1 = Mathf.Clamp(ActiveTime1 -= Time.deltaTime, 0, TotalAbilityCooldown1);
        ActiveTime2 = Mathf.Clamp(ActiveTime2 -= Time.deltaTime, 0, TotalAbilityCooldown2);
    }
    public float GetAbilityCooldownRatio(int num)
    {
        if (num == 0)
        {
            return AbilityCooldown1 / TotalAbilityCooldown1;
        }
        return AbilityCooldown2 / TotalAbilityCooldown2;
    }
    //check to see if an Ability is currently active by passing the name of the Ability
    public bool IsAbilityActive(string name)
    {
        if (skillmanager.GetAbility1() !=null && skillmanager.GetAbility1().name == name)
        {
            if (ActiveTime1 > 0)
                return true;
        }
        if (skillmanager.GetAbility2() != null && skillmanager.GetAbility2().name == name)
        {
            if (ActiveTime2 > 0)
                return true;
        }
        return false;
    }
    public void ResetAbilityCooldown1()
    {
        AbilityCooldown1 = 0;
    }

    public void ResetAbilityCooldown2()
    {
        AbilityCooldown2 = 0;
    }

    public void ReduceCooldowns(float seconds)
    {
        AbilityCooldown1 -= seconds;
        AbilityCooldown2 -= seconds;
    }


    public void SwapAbilityCooldowns()
    {
        float TCastTime = AbilityCasttime1;
        float TCooldown = AbilityCooldown1;
        float TTotalCastTime = TotalAbilityCasttime1;
        float TTotalCooldown = TotalAbilityCooldown1;
        float TActiveTime = ActiveTime1;

        AbilityCasttime1 = AbilityCasttime2;
        AbilityCooldown1 = AbilityCooldown2;
        TotalAbilityCasttime1 = TotalAbilityCasttime2;
        TotalAbilityCooldown1 = TotalAbilityCooldown2;
        ActiveTime1 = ActiveTime2;

        AbilityCasttime2 = TCastTime;
        AbilityCooldown2 = TCooldown;
        TotalAbilityCasttime2 = TTotalCastTime;
        TotalAbilityCooldown2 = TTotalCooldown;
        ActiveTime2 = TActiveTime;
    }

    private void CastPulseBurst()
    {
        player.GetComponent<PlayerController>().SetVertVel(0);
        player.KnockBackForced(-aimDir + Vector2.up, 25);
        GameObject hitbox = SpawnGameObject("PulseBurstHitbox", CastAtAngle(transform.position, aimDir, 1), Quaternion.identity);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(GetComponent<Player>().GetAbilityPower().GetValue()/2, GetComponent<Player>());
        hitbox.GetComponent<Hitbox>().SetStunTime(1.2f);
        player.SetInvunerable(.5f);
        hitbox.GetComponent<Hitbox>().SetLifeTime(.1f);
    }

    private void CastBurstCharge()
    {
        player.GetComponent<PlayerController>().SetVertVel(0);
        player.KnockBackForced(aimDir + Vector2.up, 25);
        GameObject hitbox = SpawnGameObject("BurstChargeHitbox", transform.position, Quaternion.identity);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(GetComponent<Player>().GetAbilityPower().GetValue(), GetComponent<Player>());
        hitbox.transform.parent = transform;
        player.SetInvunerable(.6f);
        player.GetComponent<PlayerController>().Dash(.6f,FD_gravityReplacement);
        hitbox.GetComponent<Hitbox>().SetLifeTime(.6f);
    }
    private void CastFireDash()
    {
        player.GetComponent<PlayerController>().SetVertVel(0);
        player.KnockBackForced(aimDir + Vector2.up, 25);
        player.GetComponent<PlayerController>().NewLayerMask(ignoreEnemiesLayerMask, FD_duration);
        GameObject hitbox = SpawnGameObject("FireDashHitbox", CastAtAngle(transform.position, aimDir, .5f), GetComponent<Gun>().firePoint.rotation);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(GetComponent<Player>().GetAbilityPower().GetValue(), GetComponent<Player>());
        hitbox.transform.parent = transform;
        player.SetInvunerable(FD_duration);
        player.GetComponent<PlayerController>().Dash(FD_duration+.1f,FD_gravityReplacement);
        hitbox.GetComponent<Hitbox>().SetLifeTime(FD_duration);
    }
    private void CastOverdrive(int num)
    {
        if (num == 1) // Reset Ability2
        {
            ResetAbilityCooldown2();
        }
        else // Reset Ability1
        {
            ResetAbilityCooldown1();
        }
        player.GetSpeed().AddBonus(player.GetSpeed().GetBaseValue() / 2, 2); // 50% MS for 2 seconds
    }

    private void CastSlowDown()
    {
        StateManager.instance.ChangeTimeScale(timeScale, TS_effectDuration);
        VFXManager.instance.TimeEffect(TS_effectDuration, 1);
    }

    private void CastSpartanLaser()
    {
        GameObject hitbox = SpawnGameObject("SpartanLaser", CastAtAngle(transform.position, aimDir, .5f), GetComponent<Gun>().firePoint.rotation);
        SpartanLaser laser = hitbox.GetComponent<SpartanLaser>();
        UniqueEffects U = GetComponent<UniqueEffects>();
        float scale = Mathf.Pow(scaleMultiPerKill, U.GetKillCount());
        hitbox.transform.localScale = new Vector3(scale, scale, scale);
        float damage = U.SL_CalculateDmg();
        hitbox.GetComponent<Hitbox>().InitializeHitbox(damage, player);
    }
    //this won't work after the changes to getmodfromstring 
    private void CastWoundSealant()
    {
        SkillObject skill = player.GetSkillManager().GetSkillFromString("Wound Sealant");
        player.GetSkillManager().RemoveSkill(skill);
        VFXManager.instance.PlayEffect("VFX_Health",transform.position);
        player.Heal(55);
    }
    private void CastManicTitan()
    {
        float AS_boost = player.GetAttackSpeed().GetValue() * MT_AttackSpeedBoost;
        player.GetAttackSpeed().AddBonus(AS_boost, MT_ActiveTime);
        player.GetCritChance().AddBonus(MT_CritBoost, MT_ActiveTime);
    }
    private void CastMultiShot()
    {
        //Multishot is dealt with in Gun.cs based on isabilityactive
    }
    private void CastHeatShield(int num)
    {
        if (GetComponent<OverHeat>().GetHeat() > 0)
        {
            GameObject shield = SpawnGameObject("HeatVentShield", transform.position, Quaternion.identity);
            shield.transform.parent = transform;
            shield.GetComponent<Pawn>().Initialize();
            shield.GetComponent<Pawn>().UpdateStats();
            shield.GetComponent<Pawn>().GetHealth().SetMaxValue(GetComponent<OverHeat>().GetHeat());
            GetComponent<OverHeat>().Increment(-GetComponent<OverHeat>().GetHeat());
            Destroy(shield, num == 1 ? ActiveTime1 : ActiveTime2);
        }
    }

    private void CastSingularity()
    {
        GameObject hitbox = SpawnGameObject("Sing_Projectile", CastAtAngle(transform.position, aimDir, .5f), GetComponent<Gun>().firePoint.rotation);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(1, player);
        hitbox.GetComponent<Projectile>().lifeTime = ((WorldXhair - (Vector2)transform.position).magnitude / hitbox.GetComponent<Projectile>().speed);
        if (hitbox.GetComponent<EmitOnDestroy>().Emits[0] != null)
        {
            GameObject pull = hitbox.GetComponent<EmitOnDestroy>().Emits[0];
        }
        if (hitbox.GetComponent<EmitOnDestroy>().Emits[0].GetComponent<EmitOnDestroy>().Emits[0] != null)
        {
            GameObject explosion = hitbox.GetComponent<EmitOnDestroy>().Emits[0].GetComponent<EmitOnDestroy>().Emits[0];
            explosion.GetComponent<Hitbox>().InitializeHitbox(player.GetAbilityPowerAmount()*.8f, player);
        }
    }

    private GameObject SpawnGameObject(string name, Vector2 position, Quaternion quat)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>("Hitboxes/" + name), position, quat);
        return effect;
    }
    private Vector2 CastAtAngle(Vector2 position, Vector2 direction, float distance)
    {
        Vector2 result = position + direction.normalized * distance;
        return result;
    }
}