using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CameraFade : MonoBehaviour
{
    // Start is called before the first frame update
    Color color;
    void Start()
    {
        GetComponent<Image>().enabled = true;
        color = GetComponent<Image>().color;
        color.a = 1;
        FadeIn(6);
    }
    void Update()
    {
        GetComponent<Image>().color = color;
    }
    public void FadeIn(float seconds)
    {
        print("runs");
        DOTween.To(() => color.a, x => color.a = x, 0, seconds);
    }
    public void FadeOut(float seconds)
    {
        DOTween.To(() => color.a, x => color.a = x, 1, seconds);
    }
    //optional secondary variable, if you want to fade the game out slightly during the gameover state
    public void Fade(float seconds, float opacity)
    {
        DOTween.To(() => color.a, x => color.a = x, opacity, seconds);
    }


}
