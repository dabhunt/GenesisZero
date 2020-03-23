﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public class UniqueEffects : MonoBehaviour
{
    //this script contains non bullet related unique effects of modifiers outside the data structure of stats
    //if this is 2, it checks twice per second
    public float checksPerSecond = 4f;
    //these are all done in multipliers, not in flat amounts
    // 1.1 = 10% increase, .9f = 10% reduction
    [Header("Adrenaline Rush")]
    public float coolReductionPerStack = .5f;
    public float coolReductionSingleStack = 1;
    [Header("Better Coolant")]
    public float coolRatePerStack = 1.1f;
    [Header("Heat Expulsion")]
    public float HE_Damage = 20;
    public float HE_TotalBurnDMG = 21;
    public float HE_StackMulti = 1.2f;
    [Header("Chemical Accelerant")]
    public float CA_MaxAttackSpeedPerStack = .50f;
    public float CA_MaxCritChancePerStack = .25f;
    [Header("Amplified Essence")]
    public float AE_MaxAPMulti = 3f;
    [Header("Cooling Cell")]
    public float heatReductionPerStack = .9f;
    [Header("Spartan Laser")]
    public float SL_cooldown = 4f;
    public float SL_bonusPerKill = 10f;
    public float SL_maxDmg = 100f;
    public float SL_decayTime = 10f;
    //how many seconds before bonus damage goes away
    private float SL_decay;
    private int SL_killCount;
    private float currentAttackSpeed = 0;
    private float currentCritChance = 0;
    private AudioManager aManager;

    private Player player;
    private OverHeat overheat;
    void Start()
    {
        player = GetComponent<Player>();
        overheat = GetComponent<OverHeat>();
        InvokeRepeating("CheckUniques",0, 1 / checksPerSecond-(Time.deltaTime * 2));
        aManager = FindObjectOfType<AudioManager>();
    }
    public void CheckUniques() {
        float multiplier = Mathf.Pow(heatReductionPerStack, player.GetSkillStack("Better Coolant"));
        overheat.ModifyHeatPerShot(multiplier);
        multiplier = Mathf.Pow(coolRatePerStack, player.GetSkillStack("Cooling Cell"));
        overheat.ModifyCoolRate(multiplier);
        ChemicalAccelerant();
        AmplifiedEssence();
        ConcentratedEssence();
        UnstableEssence();
    }
    private void Update()
    {
        StackDecayTimer();
    }
    public void OverHeatTrigger()
    {
        int HE_stacks = player.GetSkillStack("Heat Expulsion");
        if (HE_stacks > 0 && overheat.GetHeat() >= overheat.GetMaxHeat())
        {
            //HE_stack multi is 1.2f, which represents a 20% increase in size for each modifier past 1
            float multi = player.GetSkillManager().GetSkillStackAsMultiplier("Heat Expulsion", HE_StackMulti);
            GameObject hitbox = SpawnGameObject("ExpulsionHitbox", transform.position, Quaternion.identity);
            Hitbox hit = hitbox.GetComponent<Hitbox>();
            //Sets damage to double player's * stack multiplier of 20% per stack, false means it can't crit
            hitbox.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue() * 2f * multi, player, false);
            hitbox.transform.parent = transform;
            hit.SetLifeTime(.25f);
            hit.Burn = new Vector2(3,HE_TotalBurnDMG/3);
            hit.SetStunTime(1f);
            hitbox.GetComponent<SphereCollider>().radius = 2*multi;
            VFXManager.instance.PlayEffect("VFX_HeatExpulsion", hitbox.transform.position, 0, 2* multi);
            aManager.PlaySoundOneShot("SFX_ExplosionEnemy");
            overheat.SetHeat(0);
        }
    }
    public void WeakPointHit()
    {
        float stacks = player.GetSkillManager().GetSkillStack("Adrenaline Rush");
        if (stacks > 0)
        {
            //float seconds = 0;
            //if (stacks == 1) //1 second if 1 stack
            //    seconds = coolReductionSingleStack;
            //else
            //    seconds = Math.Abs(coolReductionSingleStack-coolReductionPerStack) + coolReductionPerStack * (stacks-1); // increase by .5 seconds for additional stacks past 1
            player.GetComponent<AbilityCasting>().ReduceCooldowns(stacks);
        }
        //other Modifier effects can be put inside this function
    }
    void StackDecayTimer()
    {
        if (SL_killCount > 0)
        {
            SL_decay += Time.fixedDeltaTime;
            if (SL_decay > SL_decayTime)
            {
                SL_decay = 0;
                SL_killCount = 0;
            }

        }
    }
    //Player deals bonus damage proportionate to how much essence they have, up to a cap of their base damage
    // Player has less attack speed proportionate to how much essence they have, up to a cap of half as much base attack speed
    private void ConcentratedEssence()
    {
        int stacks = player.GetSkillStack("Concentrated Essence");
        if (stacks > 0)
        {
            float ratio = player.GetEssenceAmount() / player.GetMaxEssenceAmount();
            //Sets the players bonus AP proportionate to how much AP they have, up to a cap of x3 bonus
            float cur_ADbonus = (player.GetDamage().GetBaseValue() * 1f) * stacks * ratio;
            float cur_ASdebuff = (player.GetAttackSpeed().GetBaseValue() * -.5f) * stacks * ratio;
            player.GetDamage().AddRepeatingBonus(cur_ADbonus, cur_ADbonus, 1 / checksPerSecond, "ConcentratedEssence");
            player.GetAttackSpeed().AddRepeatingBonus(cur_ASdebuff, 0, 1 / checksPerSecond, "ConcentratedEssenceDebuff");
        }
    }
    //Player deals bonus damage proportionate to how much essence they have, up to a cap of their base damage x2 (x3 total including base AD)
    // Player receives extra damage, proporitionate to how much essence they have, up to a cap of double damage received
    private void UnstableEssence()
    {
        int stacks = player.GetSkillStack("Unstable Essence");
        if (stacks > 0)
        {
            float ratio = player.GetEssenceAmount() / player.GetMaxEssenceAmount();
            float cur_ADbonus = (player.GetDamage().GetBaseValue() * 2f) * stacks * ratio;
            float cur_DmgReductionDebuff = (-1f) * stacks * ratio;
            player.GetDamage().AddRepeatingBonus(cur_ADbonus, cur_ADbonus, 1 / checksPerSecond, "UnstableEssence");
            player.GetDamageReduction().AddRepeatingBonus(cur_DmgReductionDebuff, 0, 1 / checksPerSecond, "UnstableEssenceDebuff");
        }
    }
    //Player abilities deal bonus damage proportionate to how much essence they have, up to a cap of their base AP damage
    private void AmplifiedEssence()
    {
        int stacks = player.GetSkillStack("Amplified Essence");
        if (stacks > 0)
        {
            float ratio = player.GetEssenceAmount() / player.GetMaxEssenceAmount();
            //Sets the players bonus AP proportionate to how much AP they have, up to a cap of x3 bonus
            float currentAPbonus = (player.GetAbilityPower().GetBaseValue() * 1f) * stacks * ratio;
            player.GetAbilityPower().AddRepeatingBonus(currentAPbonus, currentAPbonus, 1/checksPerSecond, "AmplifiedEssence");
        }
    }
    private void ChemicalAccelerant()
    {
        int stacks = player.GetSkillStack("Chemical Accelerant");
        if (stacks > 0)
        {
            float heat = player.GetComponent<OverHeat>().GetHeat();
            //Sets the players bonus attack speed equal to the max value proportionate to how much heat you have, up to the max of 100 heat
            currentAttackSpeed = CA_MaxAttackSpeedPerStack * stacks * (heat / 100);
            currentCritChance = CA_MaxCritChancePerStack * stacks * (heat / 100);
            //gives the player an attackspeed boost that lasts until this function is called again (every .25 seconds)
            player.GetAttackSpeed().AddRepeatingBonus(currentAttackSpeed, currentAttackSpeed, 1 / checksPerSecond, "ChemicalAccelerant_AS");
            player.GetCritChance().AddRepeatingBonus(currentCritChance, currentCritChance, 1 / checksPerSecond, "ChemicalAccelerant_CC");
        }
    }
    public float SL_CalculateDmg()
    {
        aManager.PlaySoundOneShot("SFX_AOE");
        float AP = player.GetAbilityPower().GetValue();
        return Mathf.Clamp(AP + SL_bonusPerKill * SL_killCount, AP, SL_maxDmg);
    }
    public int GetKillCount()
    {
        return SL_killCount;
    }
    public void IncrementKillCount()
    {
        SL_killCount++;
        SL_decay = 0;
    }
    private GameObject SpawnGameObject(string name, Vector2 position, Quaternion quat)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>("Hitboxes/" + name), position, quat);
        return effect;
    }
}
