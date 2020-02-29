using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Material[] lightOnMaterial;

    private bool lightsTriggered = false;

    private float lerp = 0f;
    private float startScale = 0f;
    private float endScale = 0.5f;
    private float lerpScale = 0f;
    private float lerpMultiplier = 0.05f;


    void OnTriggerEnter(Collider other)
    {
    	if (other.GetComponent<Player>())
    	{
    		lightsTriggered = true;
    	}
    }
    //now gets all children of the gameobject you put the script on automatically, no inspector required
    void FixedUpdate()
    {
    	if (lightsTriggered)
    	{
    		foreach (Transform child in this.transform)
    		{
    			lerp = Mathf.Lerp(startScale, endScale, lerpScale);
    			child.gameObject.GetComponent<Renderer>().material.SetFloat("EmissionStrength", lerp);
    			lerpScale += Time.deltaTime*lerpMultiplier;
    		}
    	}
    }
}
