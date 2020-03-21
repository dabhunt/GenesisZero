﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EssenceFill : MonoBehaviour
{

    public float TestEssence;
	public float updatesPerSecond = 2;
	public Color EssenceFillColor;
	public Color OuterCapsuleColor;
	private Image[] capsule;
	private int EssencePerCapsule;
    private float TotalCapsules;
	private int capsuleHardCap = 6; //this gets set to the amount of capsules that exist in the canvas
    private Player player;

    void Start()
    {
       GameObject temp = GameObject.FindWithTag("Player");
       	player = temp.GetComponent<Player>();
		//update UI every .3 seconds, starting in .3 seconds;
		//set Essence Per capsule equal to Max essence player can have divided by totalcapsules determined here
       InvokeRepeating("CalculateEssenceUI", 1/updatesPerSecond, 1/updatesPerSecond);
		float capAmount = (int)player.GetMaxCapsuleAmount();
		TotalCapsules = capAmount;
		FindImages();
		SetCapsuleAmount(capAmount);
    }
	private void FindImages()
	{
		Canvas canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
		Transform parent = canvasRef.transform.Find("EssencePanel").Find("FillParent");
		int num = 0;
		capsuleHardCap = parent.childCount;
		capsule = new Image[capsuleHardCap];
		foreach (RectTransform child in parent)
		{
			capsule[num] = child.gameObject.GetComponent<Image>();
			capsule[num].color = EssenceFillColor;
			capsule[num].gameObject.transform.Find("bg").gameObject.GetComponent<Image>().color = OuterCapsuleColor;
			num++;
		}
	}
	public void SetCapsuleAmount(float amount)
	{
		for (int i = 0; i < capsuleHardCap; i++)
		{
			bool onOff = true;
			if (i >= amount)
				onOff = false;
			capsule[i].gameObject.SetActive(onOff);
		}
	}
    public void CalculateEssenceUI()
    {
		TotalCapsules = (int)player.GetMaxCapsuleAmount();
		SetCapsuleAmount(TotalCapsules);
		EssencePerCapsule = (int)(player.GetMaxEssenceAmount() / TotalCapsules);
		if (player != null){
	    	float essenceQuantity = player.GetEssenceAmount();
	    	//capsulecount tracks how many full essence containers the player has
	    	int capsuleCount = 0;
	    	//check if the player has more essence than can be stored
	    	if (essenceQuantity <= player.GetMaxEssenceAmount()){
				//determine how many capsules can be filled 100% up
				for (int i = 1; i <= essenceQuantity; i ++)
		        {
		            if ((i % EssencePerCapsule) == 0)
		            {
		                capsule[capsuleCount].fillAmount = 1.0f;
						capsule[capsuleCount].gameObject.transform.Find("bg").gameObject.GetComponent<Image>().color = Color.white; //or Essencefill
						capsuleCount++;
		            }
		        }
				if (capsuleCount < TotalCapsules){
			        //calculate how much essence can be put in the remaining capsule on UI
			        float remainderFill = essenceQuantity % EssencePerCapsule;
			        //print("remainderFill after just %" remainderFill);
			        remainderFill = remainderFill/EssencePerCapsule;
			        //print("remainderFill:" remainderFill);
			        capsule[capsuleCount].fillAmount = remainderFill;
					capsule[capsuleCount].gameObject.transform.Find("bg").gameObject.GetComponent<Image>().color = OuterCapsuleColor;
					capsuleCount++;
			        while (capsuleCount < TotalCapsules)
			        {
		        		capsule[capsuleCount].fillAmount = 0f;
						capsule[capsuleCount].gameObject.transform.Find("bg").gameObject.GetComponent<Image>().color = OuterCapsuleColor;
						capsuleCount++;
		        	}
		        }
		        //In case subtraction has happened, calculate if we need to remove fill from other capsules
		    } else
		    {
		    	print("error: too much essence for my delicate UI to handle");
		    }
    	}
    }
}