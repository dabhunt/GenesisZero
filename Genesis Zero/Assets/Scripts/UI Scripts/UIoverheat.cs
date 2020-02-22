using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIoverheat : MonoBehaviour
{
	public Image fillImage;
    public Color lowHeatColor;
    public Color overHeatColor;
    public float lerpTime =.3f;
    private float lerp;
    private OverHeat overheat;
    private float curPercent;
    private float prevPercent;

    void Start()
    {
    	//get slider component
        GameObject temp = GameObject.FindWithTag("Player");
        overheat = temp.GetComponent<OverHeat>();
        InvokeRepeating("slowUpdate", .1f, .1f);
        prevPercent = 0;
    }
    void slowUpdate()
    {
    	//set fill to % of heat
    	curPercent = overheat.GetHeat()/100;
        lerp += 0.2f * Time.deltaTime;
        fillImage.fillAmount = Mathf.Lerp(prevPercent,curPercent,lerp);
        
    	// fillvalue represents how filled in the healthbar is, from 0 to 100
       	float max = overheat.GetMaxHeat()/100;
       	//if player is at 33% or lower change color
         // if (curPercent <= (max / 3) )
         // {
         // 	fillImage.color = lowHeatColor;
         // }
         // //if player is at 66% to overheating change color
         // else if (curPercent > (max / 3) )
         // {
         // 	fillImage.color = overHeatColor;
         // }
         prevPercent = curPercent;
    }
}
