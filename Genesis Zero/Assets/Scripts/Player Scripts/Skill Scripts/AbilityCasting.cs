using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(Player), typeof(PlayerController))]
public class AbilityCasting : MonoBehaviour
{
    Player player;
    PlayerController pc;
    SkillManager skillmanager;
    public bool PhaseTrigger = false;
    [Header("Time Dilation Ability")]
    public Color TS_screenTint = Color.blue;
    public float TS_alpha = .03f;
    public float TS_tweenTime = .4f;
    public float timeScale = .4f;
    public float TS_effectDuration = 3.2f;
    public float TS_offset = 1;


    [Header("Multi Shot Ability (Active)")]
    //how long the effect lasts
    public float MS_ActiveTime = 5f;
    public float MS_Cooldown = 8f;
    [Header("Manic Titan Ability (Active)")]
    //how long the effect lasts
    public float MT_ActiveTime = 4;
    public float MT_Cooldown = 9f;
    //1 = 100% more attack speed, based on base attack speed
    public float MT_AttackSpeedBoost = 1f;
    public float MT_CritBoost = .3f;
    [Header("Spartan Laser")]
    public float SL_Cooldown = 5f;
    public float SL_ActiveTime = 3f;
    public float SL_chargeUp = 1f;
    //each successful kill makes the laser 20% larger
    public float scaleMultiPerKill = 1.2f;
    private bool SL_pressed = false;
    private int SL_ActiveLastFrame = -1;
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
    public bool cooldownCheatOn = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        skillmanager = player.GetSkillManager();
        pc = GetComponent<PlayerController>();
        ui = AbilityCooldownPanel.GetComponent<AbilityCD>();
        aimDir = new Vector2(0, 0);

    }
    // Update is called once per frame
    void Update()
    {
        aimDir = pc.worldXhair.transform.position - (Vector3)pc.CenterPoint();
        WorldXhair = pc.worldXhair.transform.position;
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
        float cdr = 1-Player.instance.GetCDR().GetValue();
        switch (name)
        {
            case "Pulse Burst":
                InitializeAbility(4 * cdr, 0, 0, num);
                CastPulseBurst();
                break;
            case "Burst Charge":
                InitializeAbility(5 * cdr, 0, 0, num);
                CastBurstCharge();
                break;
            case "Overdrive":
                InitializeAbility(17 * cdr, 0, 0, num);
                CastOverdrive(num);
                break;
            case "Time Dilation":
                InitializeAbility(7 * cdr, 0, TS_effectDuration-TS_offset, num);
                CastSlowDown();
                break;
            case "Culling Blast":
                InitializeAbility(0, 0, 0, num);
                CastSpartanLaser(num);
                break;
            case "Wound Sealant":
                InitializeAbility(100 * cdr, 0, 0, num);
                CastWoundSealant();
                break;
            case "Atom Splitter":
                //(Cooldown, Casttime, ActiveTime, Abilitynum)
                //NOTE: Cooldown does not start going down until the Active wears off
                InitializeAbility(MS_Cooldown * cdr, 0, MS_ActiveTime, num);
                CastMultiShot();
                break;
            case "Heat Vent Shield":
                if (player.GetOverHeat().GetHeat() < 1)
                    return; //prevent player from using shield at 0 heat, since it does nothing
                InitializeAbility(5 * cdr, 0, 4, num);
                CastHeatShield(num);
                break;
            case "Fire Dash":
                InitializeAbility(5 * cdr, 0, 0, num);
                CastFireDash();
                break;
            case "Singularity":
                InitializeAbility(5 * cdr, 0, 0, num);
                CastSingularity();
                break;
            case "Manic Titan":
                InitializeAbility(MT_Cooldown * cdr, 0, MT_ActiveTime, num);
                CastManicTitan();
                break;
        }
        GetComponent<UniqueEffects>().AfterAbilityTrigger();
    }

    private bool CanCastAbility1()
    {
        return (AbilityCasttime1 <= 0 && AbilityCooldown1 <= 0 && skillmanager.GetAbility1() != null && GameInputManager.instance.isEnabled());
    }

    private bool CanCastAbility2()
    {
        return (AbilityCasttime2 <= 0 && AbilityCooldown2 <= 0 && skillmanager.GetAbility2() != null && GameInputManager.instance.isEnabled());
    }
    //sets the players cool down reduction
    public void SetCoolDownReduction(float percent)
    {
        Player.instance.GetCDR().SetValue(percent);
    }
    //adds to the cooldown reduction statistic
    //ex passing .2 would result in 20% cool down reduction
    public void AddCoolDownReduction(float percent)
    {
        Player.instance.GetCDR().AddValue(percent);
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
        if (AbilityCooldown1 <= 0 && !cooldownCheatOn)
        {
            AbilityCooldown1 = cooldown;
            TotalAbilityCooldown1 = cooldown;
        }
        if (AbilityCasttime1 <= 0)
        {
            AbilityCasttime1 = casttime;
            TotalAbilityCasttime1 = casttime;
        } 
        if (ActiveTime1 <= 0)
            ActiveTime1 = activeTime;
    }

    private void InitializeAbility2(float cooldown, float casttime, float activeTime)
    {
        if (AbilityCooldown2 <= 0 && !cooldownCheatOn) {
            AbilityCooldown2 = cooldown;
            TotalAbilityCooldown2 = cooldown;
        }
        if (AbilityCasttime2 <= 0)
        {
            AbilityCasttime2 = casttime;
            TotalAbilityCasttime2 = casttime;
        }
        if (ActiveTime2 <= 0)
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
        //to deal with what happens when spartan laser charged up but wasn't used within the active time frame
        int ActiveSlot = IsAbilityActive("Culling Blast");
        if (SL_ActiveLastFrame > 0 && ActiveSlot == -1)
        {
            //OPTION 1
            if (SL_ActiveLastFrame == 1)
               CastAbility1();
            else
                CastAbility2();
            //OPTION 2
            //SL_pressed = false;
            //AudioManager.instance.PlayAttachedSound("SFX_WindingDown");
        }
        SL_ActiveLastFrame = ActiveSlot;
        ActiveTime1 = Mathf.Clamp(ActiveTime1 -= Time.deltaTime, 0, 30);
        ActiveTime2 = Mathf.Clamp(ActiveTime2 -= Time.deltaTime, 0, 30);
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
    //returns -1 if not active, returns the slot number of the ability if the ability is active (1 or 2)
    public int IsAbilityActive(string name)
    {
        if (skillmanager.GetAbility1() != null && skillmanager.GetAbility1().name == name)
        {
            if (ActiveTime1 > 0)
                return 1;
        }
        if (skillmanager.GetAbility2() != null && skillmanager.GetAbility2().name == name)
        {
            if (ActiveTime2 > 0)
                return 2;
        }
        return -1;
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
        if (!IsAbilitySlotActive(1))
            AbilityCooldown1 -= seconds;
        if (!IsAbilitySlotActive(2))
            AbilityCooldown2 -= seconds;
    }
    public void ReduceSlotCooldown(float seconds, int slot)
    {
        if (IsAbilitySlotActive(slot))
            return;
        if (slot == 1)
        {

            AbilityCooldown1 -= seconds;

        }

        else
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
        pc.SetVertVel(0);
        player.KnockBackForced(-aimDir + Vector2.up, 30);
        GameObject hitbox = SpawnGameObject("PulseBurstHitbox", CastAtAngle(pc.CenterPoint(), aimDir, 1), Quaternion.identity);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(GetComponent<Player>().GetAbilityPower().GetValue(), GetComponent<Player>(), false);
        hitbox.GetComponent<Hitbox>().SetStunTime(1.2f);
        player.SetInvunerable(.5f);
        hitbox.GetComponent<Hitbox>().SetLifeTime(.15f);
        EndBonus();
    }

    private void CastBurstCharge()
    {
        player.GetComponent<PlayerController>().SetVertVel(0);
        player.KnockBackForced(aimDir + Vector2.up, 30);
        GameObject hitbox = SpawnGameObject("BurstChargeHitbox", pc.CenterPoint(), Quaternion.identity);
        hitbox.transform.LookAt(WorldXhair);
        //print("ability power in burst charge:"+ GetComponent<Player>().GetAbilityPower().GetValue());
        hitbox.GetComponent<Hitbox>().InitializeHitbox(GetComponent<Player>().GetAbilityPower().GetValue(), GetComponent<Player>(), false);
        hitbox.transform.parent = transform;
        player.SetInvunerable(.6f);
        player.GetComponent<PlayerController>().Dash(FD_duration - .1f, FD_gravityReplacement);
        hitbox.GetComponent<Hitbox>().SetLifeTime(.6f);
        EndBonus();
    }
    private void CastFireDash()
    {
        player.GetComponent<PlayerController>().SetVertVel(0);
        player.KnockBackForced(aimDir + Vector2.up, 25);
        player.GetComponent<PlayerController>().NewLayerMask(ignoreEnemiesLayerMask, FD_duration);
        GameObject hitbox = SpawnGameObject("FireDashHitbox", CastAtAngle(pc.CenterPoint(), aimDir, .5f), GetComponent<Gun>().firePoint.rotation);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(GetComponent<Player>().GetAbilityPower().GetValue() / 2, GetComponent<Player>(), false);
        hitbox.transform.parent = transform;
        player.SetInvunerable(FD_duration);
        player.GetComponent<PlayerController>().Dash(FD_duration - .1f, FD_gravityReplacement);
        hitbox.GetComponent<Hitbox>().SetLifeTime(FD_duration);
        EndBonus();
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
        VFXManager.instance.TimeEffect(TS_effectDuration-TS_offset, 1);
        StateManager.instance.TintScreenForDuration(TS_screenTint,TS_effectDuration-TS_offset, TS_tweenTime, TS_alpha);
    }

    private void CastSpartanLaser(int num)
    {
        if (SL_pressed == false)
        {   //check if the user has pressed the ability once to start the charge up process
            SL_pressed = true;
            //set the cast time manually for this ability so that we know when it's charged up 
            if (num == 1)
            {
                ui.Cast(0);
                ActiveTime1 = SL_ActiveTime;
            }
            else
            {
                ui.Cast(1);
                ActiveTime2 = SL_ActiveTime;
            }
            GameObject chargeUp = VFXManager.instance.PlayEffectReturn("VFX_CullingBuildUp", GetComponent<Gun>().firePoint.position, 0, "");
            chargeUp.transform.SetParent(GetComponent<Gun>().firePoint.transform);
            chargeUp.transform.LookAt(WorldXhair);
            AudioManager.instance.PlayAttachedSound("SFX_ChargeCulling", this.gameObject, .7f, 1, false, 0);
        }
        //if the ability has been pressed before and the casttime has reached 0
        else
        { //if the ability has charged up
            if (SL_pressed == true && ((num == 1 && ActiveTime1 <= (SL_ActiveTime - SL_chargeUp)) || (num == 2 && ActiveTime2 <= (SL_ActiveTime - SL_chargeUp))))
            {
                SL_pressed = false;
                Camera.main.transform.DOShakePosition(duration: .6f, strength: .5f, vibrato: 4, randomness: 50, snapping: false, fadeOut: true);
                AudioManager.instance.StopSound("SFX_ChargeCulling");
                AudioManager.instance.PlayAttachedSound("SFX_LaserBlast");
                GameObject hitbox = SpawnGameObject("SpartanLaser", GetComponent<Gun>().firePoint.position, GetComponent<Gun>().firePoint.rotation);
                SpartanLaser laser = hitbox.GetComponent<SpartanLaser>();
                hitbox.transform.SetParent(GetComponent<Gun>().firePoint);
                GameObject vfx_blast = VFXManager.instance.PlayEffectReturn("VFX_CullingBlast", GetComponent<Gun>().firePoint.position, 0, "");
                vfx_blast.transform.SetParent(GetComponent<Gun>().firePoint.transform);
                vfx_blast.transform.LookAt(WorldXhair);
               // vfx_blast.transform.rotation = GetComponent<Gun>().firePoint.rotation;
                UniqueEffects U = GetComponent<UniqueEffects>();
                float scale = Mathf.Pow(scaleMultiPerKill, U.GetKillCount());
                hitbox.transform.localScale = new Vector3(scale, scale, scale);
                vfx_blast.transform.localScale = new Vector3(scale, scale, scale);
                float damage = U.SL_CalculateDmg();
                hitbox.GetComponent<Hitbox>().InitializeHitbox(damage, player, false);
                //manually put spartan laser ability on cooldown since the initial cast can't have a cooldown
                if (num == 1)
                {
                    ActiveTime1 = 0;
                    AbilityCooldown1 = SL_Cooldown;
                    TotalAbilityCooldown1 = SL_Cooldown;
                    ui.Cast(0);
                }
                else
                {
                    ActiveTime2 = 0;
                    AbilityCooldown2 = SL_Cooldown;
                    TotalAbilityCooldown2 = SL_Cooldown;
                    ui.Cast(1);
                }
                EndBonus();
            }
        }
    }
    private void CastWoundSealant()
    {
        SkillObject skill = player.GetSkillManager().GetSkillFromString("Wound Sealant");
        player.GetSkillManager().RemoveSkill(skill);
        VFXManager.instance.PlayEffect("VFX_Health", pc.CenterPoint());
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
            GameObject shield = SpawnGameObject("HeatVentShield", pc.CenterPoint(), Quaternion.identity);
            shield.transform.parent = transform;
            shield.GetComponent<Pawn>().Initialize();
            shield.GetComponent<Pawn>().UpdateStats();
            shield.GetComponent<Pawn>().GetHealth().SetMaxValue(GetComponent<OverHeat>().GetHeat());
            GetComponent<OverHeat>().Increment(-GetComponent<OverHeat>().GetHeat());
            Destroy(shield, num == 1 ? ActiveTime1 : ActiveTime2);
        }
    }
    public bool IsAbilitySlotActive(int num)
    {
        if (1+num == 1)
            return ActiveTime1 > 0;
        else
            return ActiveTime2 > 0;
    }
    public void EndBonus()
    {
        player.GetAbilityPower().EndRepeatingBonus("PS_TempAbilityMultiplier");
        GetComponent<UniqueEffects>().ResetPhaseTrigger();
    }
    private void CastSingularity()
    {
        AudioManager.instance.PlayRandomSFXType("blackholeInitial");
        GameObject hitbox = SpawnGameObject("Sing_Projectile", GetComponent<Gun>().firePoint.position, GetComponent<Gun>().firePoint.rotation);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(1, player);
        hitbox.GetComponent<Projectile>().lifeTime = ((WorldXhair - (Vector2)GetComponent<Gun>().firePoint.position).magnitude / hitbox.GetComponent<Projectile>().speed);
        if (hitbox.GetComponent<EmitOnDestroy>().Emits[0] != null)
        {
            GameObject pull = hitbox.GetComponent<EmitOnDestroy>().Emits[0];
        }
        if (hitbox.GetComponent<EmitOnDestroy>().Emits[0].GetComponent<EmitOnDestroy>().Emits[0] != null)
        {
            GameObject explosion = hitbox.GetComponent<EmitOnDestroy>().Emits[0].GetComponent<EmitOnDestroy>().Emits[0];
            explosion.GetComponent<Hitbox>().InitializeHitbox(player.GetAbilityPowerAmount()*1, player);
        }
        EndBonus();
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