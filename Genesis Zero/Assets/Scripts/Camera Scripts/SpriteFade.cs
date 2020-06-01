using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpriteFade : MonoBehaviour
{
    // Start is called before the first frame update
    public Color color;
    private Color resetColor;
    public bool startVisible = false;
    public bool FadeOutAtStart = false;
    public float tweenBack = .75f;
    void Start()
    {
        if (startVisible)
            color.a = 1;
        else 
        {
            color.a = 0;
        }
        GetComponent<Image>().enabled = true;
        color = GetComponent<Image>().color;
        if (FadeOutAtStart)
        {
            color.a = 1;
            FadeOut(6);
        }
        resetColor = color;
    }
    void Update()
    {
        GetComponent<Image>().color = color;
    }
    //fades the overlay opacity in over float seconds
    public void FadeIn(float seconds)
    {
        DOTween.To(() => color.a, x => color.a = x, 1, seconds);
    }
    //fades the overlay out over float seconds
    public void FadeOut(float seconds)
    {
        TweenParams tParms = new TweenParams().SetEase(Ease.InExpo);
        DOTween.To(() => color.a, x => color.a = x, 0, seconds);
        Invoke("ResetColor", seconds);
    }
    //optional secondary variable, if you want to fade the game out slightly during the gameover state
    public void Fade(float seconds, float opacity)
    {
        DOTween.To(() => color.a, x => color.a = x, opacity, seconds);
    }
    public void TweenBack()
    {
        DOTween.To(() => color.a, x => color.a = x, 0, tweenBack*.9f);
    }
    public void ResetColor()
    {
        color = new Color(resetColor.r, resetColor.g, resetColor.b, color.a);
    }
    public void HurtTween(float hurtamount)
    {
        DOTween.To(() => color.a, x => color.a = x, hurtamount, tweenBack/2);
        Invoke("TweenBack", tweenBack/2);
    }
}
