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

       TestEssence = Random.Range(1,25);
       InvokeRepeating("CalculateEssenceUI", .2f, .3f);
    }
    void CalculateEssenceUI()
    {
    	print("calculating essence...");
    	var essenceQuantity = TestEssence;
    	var capsuleCount = 0;
    	//check if the player has more essence than can be stored
    	if (essenceQuantity <= EssencePerCapsule*TotalCapsules){
    	//determine how many capsules can be filled 100% up
	    	for (int i = 0; i < essenceQuantity; i++)
	        {
	            if ((i % TotalCapsules) == 0)
	            {
	                capsule[capsuleCount].fillAmount = 1.0f;
	                capsuleCount++;
	            }
	        }
	        //calculate how much essence can be put in the remaining capsule on UI
	        var remainderFill = essenceQuantity % EssencePerCapsule;
	        remainderFill = remainderFill/EssencePerCapsule;
	        capsule[capsuleCount].fillAmount = remainderFill;
	    } else
	    {
	    	print("error: too much essence for my delicate UI to handle");
	    }

    }
}