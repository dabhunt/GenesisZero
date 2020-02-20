using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverHeat : MonoBehaviour
{
    public float bloomMultiplier=5f;
    public float maxHeat=100f;
    public float heatAddedPerShot=3;
    public float coolRatePerSecond = 1f;
    public float delayBeforeCooling = 1f;
    public float ticksPerSecond=20;

    private float coolDelay;
    private float heat;
    private bool isOverheated;


    void Awake()
    {
       InvokeRepeating("Timer", 1/ticksPerSecond , 1/ticksPerSecond); 
    }
    public float CalculateBloom()
    {
    	//also adds heat because this is called whenever the player successfully fires a shot
    	coolDelay = delayBeforeCooling;
    	heat +=heatAddedPerShot;
    	return bloomMultiplier * heat*.1f; 
    }
    public void Timer(){
    	// if heat is greater than 0 the gun cools down
    	coolDelay -= 1/ticksPerSecond;
    	if (heat > 0 && coolDelay <= 0){
    		heat -= (coolRatePerSecond/ticksPerSecond);
    	} else{
    		heat = 0;
    		if (isOverheated)
    			isOverheated = false;
    	}
    	print("heat: "+heat);
    	print("coolDelay: "+coolDelay);
	}
    public bool IsOverheated()
    {
    	if (heat >= maxHeat){
    		heat = maxHeat;
    		isOverheated = true;
    	}
    	return isOverheated;
    }
}
