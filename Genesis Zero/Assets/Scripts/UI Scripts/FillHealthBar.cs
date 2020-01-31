using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FillHealthBar : MonoBehaviour
{
	public Image fillImage;
	public Slider slider;
	public Pawn player;
    public Color fillColor;
    public Color backFillColor;
    public Color lowHPcolor;

    void Awake()
    {
    	//get slider component
        slider = GetComponent<Slider>();
    }
    void Update()
    {
    	if (slider.value <= slider.minValue)
    	{
    		fillImage.enabled = false;
    	}
    	if (slider.value > slider.minValue && fillImage.enabled)
    	{
    		fillImage.enabled = true;
    	}
    	// uses the player object to get info about player health
    	// fillvalue represents how filled in the healthbar is, from 0 to 100
        float fillvalue = player.GetHealth().GetValue();
        //print("fillvalue:"+ fillvalue);
        // if player is at less than 33% of max hp, display a different color red for now
         if (fillvalue <= slider.maxValue / 3)
         {
         	fillImage.color = Color.red;
         }
         // if player is at more than 33% of max hp, display a different color  green for now
         else if (fillvalue > slider.maxValue / 3 )
         {
         	fillImage.color = fillColor;
         }
        slider.value = fillvalue;
    }
    // test function to see if damage the player takes is being shown
    // public void HurtPlayerTest(){
    // 	player.TakeDamage(5);
    // }
}
