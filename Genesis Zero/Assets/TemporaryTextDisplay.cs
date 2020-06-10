﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class TemporaryTextDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    //float defaultDuration = 4f;
    //float defaultTweenTime = .75f;
    TextMeshProUGUI txt;
    private float tween;
    public static TemporaryTextDisplay instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
        txt.DOFade(0, 0.1f);
        txt.enabled = false;
    }
    public void ShowText(string text, float duration, float tweentime)
    {
        txt.enabled = true;
        txt.text = text;
        tween = tweentime;
        txt.DOFade(1, tweentime);
        Invoke("FadeText", duration);
    }
    public void FadeText()
    {
        txt.DOFade(0, tween);
        Invoke("Disable", tween);
    }
    public void Disable()
    {
        txt.enabled = false;
    }

}
