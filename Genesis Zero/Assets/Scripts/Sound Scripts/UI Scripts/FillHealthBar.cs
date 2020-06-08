using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FillHealthBar : MonoBehaviour
{
	public Image fillImage;
	public Image shieldImage;
	public Slider slider;
	public Slider shieldSlider;
    public Color fillColor = new Color(0,1,.9f);
    public Color gainHPColor = new Color(.25f, .75f, .27f);
    public Color lowHPcolor;
    [Header("Camera Shake")]
    public float randomness = 60;
    public float strength = 1f;
    public float duration = .75f;
    public int vibration = 5;

    private Player player;
    private float valueLastFrame = 100;
    private float shieldLastFrame = 30;
    private float tweenTime = .7f;
    private Color color;
    private GameObject canvas;
    private SpriteFade fade;
    private Color zero; //lowHPcolor but with 0 alpha
    private bool delayedStart = true;
    private Camera camRef;
    private Tween t = null;
    void Start()
    {
        camRef = Camera.main;
        Invoke("DelayedStart", 0);
        shieldSlider.value = 0;
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
    public void UpdateHealthBar()
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
        if (shieldImage != null)
            ShieldUpdate();
        slider.maxValue = player.GetHealth().GetMaxValue();
        // uses the player object to get info about player health
        // fillvalue represents how filled in the healthbar is, from 0 to 100
        float curValue = player.GetHealth().GetValue();
        if (!StateManager.instance.InTutorial && GetComponent<SimpleTooltip>())
        {
            GetComponent<SimpleTooltip>().infoLeft = curValue + " / " + player.GetHealth().GetMaxValue() + " Health";
        }
        if (curValue < valueLastFrame && curValue > 0)
        {
            player.GetComponent<Gun>().PlayerHurtTrigger = true;
            fillImage.color = Color.white;
            DOTween.To(() => slider.value, x => slider.value = x, curValue, tweenTime);
            float pain = (valueLastFrame - curValue) / 15;
            if (pain > .5f)
            {
                camRef.transform.DOShakePosition(duration: duration, strength: strength, vibrato: vibration, randomness: randomness, snapping: false, fadeOut: true);
            }
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
    void Update()
    {
        UpdateHealthBar();
    }
    public void ShieldUpdate()
    {
        float shield = player.GetShield().GetValue();
        shieldSlider.maxValue = player.GetShield().GetMaxValue();
        if (shield < shieldLastFrame || (shieldSlider.value <= 0 && shield > 20))
        {
            t = shieldSlider.DOValue(shield, tweenTime);
            shieldImage.DOColor(Color.white, .3f);
        }
        shieldLastFrame = shield;
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
