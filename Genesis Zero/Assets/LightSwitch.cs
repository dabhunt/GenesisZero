using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public GameObject[] lightObjects;
    public Material[] lightOnMaterial;

    private bool lightsTriggered = false;

    private float lerp = 0f;
    private float startScale = 0f;
    private float endScale = 200f;
    private float lerpScale = 0f;
    private float lerpMultiplier = 0.005f;


    void OnTriggerEnter(Collider other)
    {
    	if (other.GetComponent<Player>())
    	{
    		lightsTriggered = true;
    	}
    }

    void FixedUpdate()
    {
    	if (lightsTriggered)
    	{
    		foreach (GameObject light in lightObjects)
    		{
    			lerp = Mathf.Lerp(startScale, endScale, lerpScale);
    			light.GetComponent<Renderer>().material.SetFloat("EmissionStrength", lerp);
    			lerpScale += Time.deltaTime*lerpMultiplier;
    		}
    	}
    }
}
