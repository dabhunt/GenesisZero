using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverHeat : MonoBehaviour
{
	//adjustable in script
    [Header("Base stats")]
    public float baseBloomMultiplier = 1f;
    public float baseHeatAddedPerShot = 7f;
    public float baseCoolRatePerSecond = 30f;
    public float baseDelayBeforeCooling = 1f;

    [Header("probably don't change these")]
    public float ticksPerSecond = 20f;
	public float maxHeat = 100f;
	//private floats
	private float bloomMultiplier;
    private float heatAddedPerShot;
    private float coolRatePerSecond;
    private float delayBeforeCooling;
    private float coolDelay;
    private float heat;
    private bool isOverheated;
    private bool isCoolingDown;
    void Start()
    {
        InvokeRepeating("Timer", 1/ticksPerSecond , 1/ticksPerSecond); 
       	bloomMultiplier = baseBloomMultiplier;
	    heatAddedPerShot = baseHeatAddedPerShot;
	    coolRatePerSecond = baseCoolRatePerSecond;
	    delayBeforeCooling = baseDelayBeforeCooling;
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
        Increment(heatAddedPerShot);
        return CalculateBloom();
    }
    public float CalculateBloom()
    {
        coolDelay = delayBeforeCooling;
        var bloom = .1f * bloomMultiplier * heat;
        return bloom;
    }
    public void Timer()
    {
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
	//if heat is more than 66% full, return true
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
    //All adjustments should be multiplication of original stat
    // these functions should be called by Modifiers
    // it is calculated this way so that the base rates remain in tact, so they may be removed easily
    public void ModifyHeatPerShot(float adjustment)
    {
    	heatAddedPerShot = baseHeatAddedPerShot;
    	heatAddedPerShot *= adjustment;
    }
    public void ModifyCoolRate(float adjustment)
    {
    	coolRatePerSecond = baseCoolRatePerSecond;
    	coolRatePerSecond *= adjustment;
    }
    public void ModifyBloomMultiplier(float adjustment)
    {
    	bloomMultiplier = baseBloomMultiplier;
    	bloomMultiplier *= adjustment;
    }
    public void ModifyCoolDelay(float adjustment)
    {
    	delayBeforeCooling = baseDelayBeforeCooling;
    	delayBeforeCooling *= adjustment;
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
    public float GetHeatAddedPerShot()
    {
    	return heatAddedPerShot;
    }
}
