using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ScaleOverTime : MonoBehaviour
{
    // Start is called before the first frame update
    public float scale;
    public float tweenTime;
    public float delay;
    public bool SetScaleAtStart = false;
    public float initialScale;

    
    void Start()
    {
        if (SetScaleAtStart)
            this.transform.localScale = new Vector3(initialScale, initialScale, initialScale);
        Invoke("Shrink", + delay);
    }
    public void Shrink()
    {
        this.transform.DOScale(new Vector3(scale,scale,scale), tweenTime);
    }
}
