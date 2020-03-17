﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UniqueEffects : MonoBehaviour
{
    //this script contains non bullet related unique effects of modifiers outside the data structure of stats
    //if this is 2, it checks twice per second
   public float checksPerSecond = 2f;
   //these are all done in multipliers, not in flat amounts
   // 1.1 = 10% increase, .9f = 10% reduction
    [Header("Better Coolant")]
    public float coolRatePerStack = 1.1f;
    [Header("Cooling Cell")]
    public float heatReductionPerStack = .9f;
    [Header("Spartan Laser")]
    public float SL_cooldown = 4f;
    public float SL_bonusPerKill = 10f;
    public float SL_baseDmg = 70f;
    public float SL_maxDmg = 100f;
    public float SL_decayTime = 10f;
    //how many seconds before bonus damage goes away
    private float SL_decay;
    private int SL_killCount;
    private AudioManager aManager;

    private Player player;
    private OverHeat overheat;
    void Start()
    {
        player = GetComponent<Player>();
        overheat = GetComponent<OverHeat>();
        InvokeRepeating("CheckUniques", 1/checksPerSecond, 1/checksPerSecond);
        aManager = FindObjectOfType<AudioManager>();
    }
    public void CheckUniques(){
        float multiplier = Mathf.Pow(heatReductionPerStack, player.GetSkillStack("Better Coolant"));
        overheat.ModifyHeatPerShot(multiplier);
        multiplier = Mathf.Pow(coolRatePerStack, player.GetSkillStack("Cooling Cell"));
        overheat.ModifyCoolRate(multiplier);
    }
    private void Update()
    {
        StackDecayTimer();
    }
    public void WeakPointHit()
    {
        float stacks = player.GetSkillManager().GetSkillStack("Mental Rush");
        if (stacks > 0)
        {
            float seconds = 0;
            if (stacks == 1) //1 second if 1 stack
                seconds = 1;
            else
                seconds = .5f + .5f * stacks; // increase by .5 seconds for additional stacks past 1
            player.GetComponent<AbilityCasting>().ReduceCooldowns(seconds);
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
                print("no kills in 10 seconds, decay resetting to 0..");
                SL_decay = 0;
                SL_killCount = 0;
            }

        }
    }
    public float CalculateDmg()
    {
        aManager.PlaySoundOneShot("SFX_AOE");
        return Mathf.Clamp(SL_baseDmg + player.GetDamage().GetValue() + SL_bonusPerKill * SL_killCount, SL_baseDmg, SL_maxDmg);
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
}
