using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FillHealthBar : MonoBehaviour
{
	public Image fillImage;
	public Slider slider;
    public Color fillColor = new Color(0,1,.9f);
    public Color gainHPColor = new Color(.25f, .75f, .27f);
    public Color lowHPcolor;

    private Player player;
    private float valueLastFrame = 100;
    private float tweenTime = .7f;
    private Color color;
    private GameObject canvas;
    private SpriteFade fade;
    private Color zero; //lowHPcolor but with 0 alpha
    private bool delayedStart = true;
    void Start()
    {
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
        fade = canvas.transform.Find("VignetteOverlay").gameObject.GetComponent<SpriteFade>();
    }
    void Update()
    {
        if (delayedStart)
            return;
        if (player == null)
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
            DOTween.To(() => slider.value, x => slider.value = x, curValue, tweenTime);
            float pain = (valueLastFrame - curValue) / 15;
            pain = Mathf.Clamp(pain, 0, 1);
            fade.HurtTween(pain);
            HurtTween();
            Invoke("TweenBackBar", tweenTime);
        }
        if (curValue > valueLastFrame) 
        {
            fillImage.color = Color.white;
            DOTween.To(() => slider.value, x => slider.value = x, curValue, tweenTime);
            HealTween();
            Invoke("TweenBackBar", tweenTime);
        }
        valueLastFrame = curValue;
    }
    public void HurtTween()
    {
        DOTween.To(() => fillImage.color, x => fillImage.color = x, lowHPcolor, tweenTime);
    }
    public void HealTween()
    {
        DOTween.To(() => fillImage.color, x => fillImage.color = x, gainHPColor, tweenTime);
    }
    public void TweenBackBar()
    {
        DOTween.To(() => fillImage.color, x => fillImage.color = x, fillColor, tweenTime);
    }
}
