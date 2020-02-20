using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EssenceFill : MonoBehaviour
{
	public Image[] capsule;
    public int TestEssence;
    public int EssencePerCapsule;
    public int TotalCapsules;
    private Player player;


    void Awake()
    {
        //will use this code once essence is stored on the player
        //GameObject temp = GameObject.FindWithTag("Player");
        //player = temp.GetComponent<Player>();
        //float fillvalue = player.GetHealth().GetValue();

       TestEssence = 0;
       //update UI every .3 seconds, starting in .3 seconds;
       //InvokeRepeating("test", .3f,.3f);
       InvokeRepeating("CalculateEssenceUI", .5f, .5f);
       
    }
    void test()
    {
    	TestEssence ++;
    	if (TestEssence > 25){
    		TestEssence = 0;
    	}
    }
    void CalculateEssenceUI()
    {
    	print("calculating essence...");
    	var essenceQuantity = TestEssence;
    	var capsuleCount = 0;
    	//check if the player has more essence than can be stored
    	if (essenceQuantity <= EssencePerCapsule*TotalCapsules){
    	//determine how many capsules can be filled 100% up
	    	for (int i = 1; i <= essenceQuantity; i++)
	        {
	            if ((i % EssencePerCapsule) == 0)
	            {
	                capsule[capsuleCount].fillAmount = 1.0f;
	                capsuleCount++;
	            }
	        }

	        if (capsuleCount <= TotalCapsules-1){
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