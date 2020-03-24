using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverHeat : MonoBehaviour
{
	//adjustable in script
    [Header("Base stats")]
    public float baseBloomMultiplier = 1f;
    public float baseHeatAddedPerShot = 3.3f;
    public float baseCoolRatePerSecond = 38f;
    public float baseDelayBeforeCooling = .5f;
    [Header("Broken Cooling Cell")]
    public float BC_minHeat = 20;
    private float ticksPerSecond = 20f;
	private float maxHeat = 100f;
    //private floats
    private List<Statistic> heatStatistics;
	private Statistic bloomMultiplier;
    private Statistic heatAddedPerShot;
    private float coolRatePerSecond;
    private Statistic delayBeforeCooling;
    private float coolDelay;
    private float heat;
    private bool isOverheated;
    private bool isCoolingDown;
    void Start()
    {
        InitializeOverheatStats();
        InvokeRepeating("Timer", 1/ticksPerSecond , 1/ticksPerSecond); 
	    coolRatePerSecond = baseCoolRatePerSecond;
    }
    private void LateUpdate()
    {
        float stacks = GetComponent<Player>().GetSkillStack("Broken Cooling Cell");
        float minHeat = stacks * BC_minHeat;
        if (heat < minHeat)
        {
            heat = minHeat;
        }
    }
    public void InitializeOverheatStats()
    {
        heatStatistics = new List<Statistic>();
        heatStatistics.Add(heatAddedPerShot = new Statistic(baseHeatAddedPerShot));
        heatAddedPerShot.SetValue(baseHeatAddedPerShot);
        heatStatistics.Add(delayBeforeCooling = new Statistic(baseDelayBeforeCooling));
        delayBeforeCooling.SetValue(baseDelayBeforeCooling);
        heatStatistics.Add(bloomMultiplier = new Statistic(baseBloomMultiplier));
        delayBeforeCooling.SetValue(baseBloomMultiplier);
    }
    public void Increment(float amount)
    {
    	heat += amount;
        heat = Mathf.Clamp(heat, 0, maxHeat);
        if (heat >= maxHeat) 
        {
            GetComponent<UniqueEffects>().OverHeatTrigger();
        }
    }
    public float ShootBloom()
    {
    	//also adds heat because this is called whenever the player successfully fires a shot
        Increment(heatAddedPerShot.GetValue());
        return CalculateBloom();
    }
    public float CalculateBloom()
    {
        coolDelay = delayBeforeCooling.GetValue();
        var bloom = .1f * bloomMultiplier.GetValue() * heat;
        return bloom;
    }
    public void Timer()
    {
        foreach (Statistic stat in heatStatistics)
        {
            stat.UpdateStatistics();
        }
        // if heat is greater than 0 the gun cools down
        coolDelay -= 1/ticksPerSecond;
    	if (heat > 0 && coolDelay <= 0)
        {
    		isCoolingDown = true;
    		heat -= (coolRatePerSecond/ticksPerSecond);
            heat = Mathf.Max(heat, 0);
    	}
	}
	//return if overheat is currently cooling down
	public bool IsCooling()
	{
		return isCoolingDown;
	}
	//if heat bar is maxed out, return true
    public bool IsOverheated()
    {
    	if (heat >= maxHeat)
        {
    		isOverheated = true;
    	} 
        else
        {
    		isOverheated = false;
    	}
    	return isOverheated;
    }
    public void SetHeat(float value)
    {
        heat = value;
    }
    public float GetHeat()
    {
    	return heat;
    }
    public float GetMaxHeat()
    {
    	return maxHeat;
    }
    public Statistic GetHeatAddedPerShot()
    {
        return heatAddedPerShot;
    }
    public Statistic GetDelayBeforeCooling()
    {
        return delayBeforeCooling;
    }
    public Statistic GetBloomMultiplier()
    {
        return bloomMultiplier;
    }
}
