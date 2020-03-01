using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UniqueModEffects : MonoBehaviour
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
    [Header("Peripheral Bullet")]
    //this is a multiplier, so 20%
    public float extraBloomPerStack = 1.2f;

    private Player player;
    private OverHeat overheat;
    void Start()
    {
        player = GetComponent<Player>();
        overheat = GetComponent<OverHeat>();
        InvokeRepeating("CheckUniques", 1/checksPerSecond, 1/checksPerSecond);
    }
    public void CheckUniques(){
        float multiplier = Mathf.Pow(heatReductionPerStack, player.GetSkillStack("Better Coolant"));
        overheat.ModifyHeatPerShot(multiplier);
        multiplier = Mathf.Pow(coolRatePerStack, player.GetSkillStack("Cooling Cell"));
        overheat.ModifyCoolRate(multiplier);
        multiplier = Mathf.Pow(extraBloomPerStack, player.GetSkillStack("Peripheral Bullet"));
        overheat.ModifyBloomMultiplier(multiplier);
    }
}
