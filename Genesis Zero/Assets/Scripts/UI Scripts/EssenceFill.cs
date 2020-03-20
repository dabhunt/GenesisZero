using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EssenceFill : MonoBehaviour
{
	public Image[] capsule;
    public float TestEssence;
	public float updatesPerSecond = 2;
	private int EssencePerCapsule;
    private int TotalCapsules;
    private Player player;

    void Start()
    {
       GameObject temp = GameObject.FindWithTag("Player");
       	player = temp.GetComponent<Player>();
		//update UI every .3 seconds, starting in .3 seconds;
		//set Essence Per capsule equal to Max essence player can have divided by totalcapsules determined here

       TotalCapsules = capsule.Length;
       InvokeRepeating("CalculateEssenceUI", 1/updatesPerSecond, 1/updatesPerSecond);
		setExtraCapsule(false);
    }
	public void setExtraCapsule(bool boo)
	{
		Canvas canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
		canvasRef.transform.Find("EssencePanel").Find("capsule (6)").gameObject.SetActive(boo);
		canvasRef.transform.Find("EssencePanel").Find("bg capsule (6)").gameObject.SetActive(boo);
	}
    void CalculateEssenceUI()
    {
		TotalCapsules = (int)player.GetMaxCapsuleAmount();
		EssencePerCapsule = (int)(player.GetMaxEssenceAmount() / TotalCapsules);
		if (player != null){
	    	float essenceQuantity = player.GetEssenceAmount();
	    	//print("getEssence Quantity:" + essenceQuantity);
	    	int capsuleCount = 0;
	    	//check if the player has more essence than can be stored
	    	if (essenceQuantity <= player.GetMaxEssenceAmount()){
	    		//determine how many capsules can be filled 100% up
		    	for (int i = 1; i <= essenceQuantity; i++)
		        {
		            if ((i % EssencePerCapsule) == 0)
		            {
		                capsule[capsuleCount].fillAmount = 1.0f;
		                capsuleCount++;
		            }
		        }

		        if (capsuleCount < TotalCapsules-1){
			        //calculate how much essence can be put in the remaining capsule on UI
			        float remainderFill = essenceQuantity % EssencePerCapsule;
			        //print("remainderFill after just %" remainderFill);
			        remainderFill = remainderFill/EssencePerCapsule;
			        //print("remainderFill:" remainderFill);
			        capsule[capsuleCount].fillAmount = remainderFill;
			        capsuleCount++;
			        while (capsuleCount < TotalCapsules)
			        {
		        		capsule[capsuleCount].fillAmount = 0f;
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