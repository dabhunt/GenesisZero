using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FillHealthBar : MonoBehaviour
{
	public Image fillImage;
	public Slider slider;
    public Color fillColor;
    public Color lowHPcolor;
    private Player player;
    private float valueLastFrame;
    private float tweenTime = .7f;
    private GameObject overlay;
    private Color color;
    private GameObject canvas;

    void Start()
    {
    	//get slider component
        slider = GetComponent<Slider>();
        GameObject temp = GameObject.FindWithTag("Player");
        canvas = GameObject.FindWithTag("CanvasUI");
        player = temp.GetComponent<Player>();
        overlay = canvas.transform.Find("VignetteOverlay").gameObject;
        color = overlay.GetComponent<Image>().color;
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
        float curValue = player.GetHealth().GetValue();
        
        if (curValue < valueLastFrame)
        {
            
            fillImage.color = Color.white;
            DOTween.To(() => slider.value, x => slider.value = x, curValue, tweenTime);
            float pain = (valueLastFrame - curValue) / 15;
            pain = Mathf.Clamp(pain, 0, 1);
            HurtTween(pain);
            Invoke("TweenBack", tweenTime / 2);
        }
        overlay.GetComponent<Image>().color = color;
        valueLastFrame = curValue;
        //print("fillvalue:"+ fillvalue);
        // if player is at less than 33% of max hp, display a different color red for now
    }
    public void HurtTween(float hurtAmount)
    {
        DOTween.To(() => fillImage.color, x => fillImage.color = x, lowHPcolor, tweenTime/2);
        DOTween.To(() => color.a, x => color.a = x, hurtAmount, tweenTime/2);
    }
    public void TweenBack()
    {
        DOTween.To(() => fillImage.color, x => fillImage.color = x, fillColor, tweenTime/2);
        DOTween.To(() => color.a, x => color.a = x, 0, tweenTime/2);
    }

}
