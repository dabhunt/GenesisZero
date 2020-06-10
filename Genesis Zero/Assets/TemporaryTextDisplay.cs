using System.Collections;
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
    public static TemporaryTextDisplay instance;
    private float tween;
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
        txt.alpha = 0;
    }
    public void ShowText(string text, float duration, float tweentime)
    {
        txt.text = text;
        tween = tweentime;
        txt.DOFade(1, tweentime);
        Invoke("FadeText", duration);
    }
    public void FadeText()
    {
        txt.DOFade(0, tween);
    }
}
