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
    private float valueLastFrame = 100;
    private float healthbarTime = .7f;
    private float overlayTime = 1.5f;
    private Color color;
    private GameObject canvas;
    private Image overlay; // the hurt effect overlay
    private Color zero; //lowHPcolor but with 0 alpha
    private bool delayedStart = true;
    void Start()
    {
    	//get slider component
        Invoke("DelayedStart", 4);
    }
    public void DelayedStart()
    {
        delayedStart = false;
        slider = GetComponent<Slider>();
        zero = new Color(lowHPcolor.r, lowHPcolor.g, lowHPcolor.b, 0);
        GameObject temp = GameObject.FindWithTag("Player");
        valueLastFrame = temp.GetComponent<Player>().GetHealth().GetValue();
        canvas = GameObject.FindWithTag("CanvasUI");
        player = temp.GetComponent<Player>();
        overlay = canvas.transform.Find("VignetteOverlay").gameObject.GetComponent<Image>();
    }
    void Update()
    {
        if (delayedStart)
            return;
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
        
        if (curValue < valueLastFrame && curValue > 0)
        {
            fillImage.color = Color.white;
            DOTween.To(() => slider.value, x => slider.value = x, curValue, healthbarTime);
            float pain = (valueLastFrame - curValue) / 15;
            pain = Mathf.Clamp(pain, 0, 1);
            HurtTween(pain);
            Invoke("TweenBackBar", healthbarTime);
            Invoke("TweenBackOverlay", overlayTime/2);
        }
        valueLastFrame = curValue;
        //print("fillvalue:"+ fillvalue);
        // if player is at less than 33% of max hp, display a different color red for now
    }

    public void HurtTween(float hurtAmount)
    {
        DOTween.To(() => fillImage.color, x => fillImage.color = x, lowHPcolor, healthbarTime);
        Color newcolor = new Color(lowHPcolor.r, lowHPcolor.g, lowHPcolor.b, hurtAmount);
        DOTween.To(() => overlay.color, x => overlay.color = x, newcolor, overlayTime/2);
    }
    public void TweenBackBar()
    {
        DOTween.To(() => fillImage.color, x => fillImage.color = x, fillColor, healthbarTime);
    }
    public void TweenBackOverlay()
    {
        DOTween.To(() => overlay.color, x => overlay.color = x, zero, overlayTime);
    }
    public void SetAlpha(float alpha)
    {
        color = overlay.GetComponent<Image>().color;
        overlay.GetComponent<Image>().color = new Color(color.r,color.g,color.b,alpha);
    }

}
