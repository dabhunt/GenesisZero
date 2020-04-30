using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TweenScale : MonoBehaviour
{
    // Start is called before the first frame update
    public float tweenDuration = 4f;
    public float sizeIncreaseMulti = 2f;
    public float sizeDecreaseMulti = .5f;
    private Tween tweenGrow;
    private Tween tweenShrink;
    void Start()
    {
        Shrink();
    }
    void Shrink()
    {
        tweenShrink = transform.DOScale(new Vector2(sizeDecreaseMulti, sizeDecreaseMulti), tweenDuration);
        Invoke("Grow", tweenDuration);
    }
    void Grow()
    {
        tweenGrow= transform.DOScale(new Vector2(sizeIncreaseMulti, sizeIncreaseMulti), tweenDuration);
        Invoke("Shrink", tweenDuration);
    }


}
