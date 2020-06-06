using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TweenTo : MonoBehaviour
{

    public Vector3 NewLocalPos;
    public float tweenDuration;
    private bool animPlayed;
    private Vector3 OriginalPos;
    private void Start()
    {
        OriginalPos = transform.position;
    }
    public void Move()
    {
        this.transform.DOLocalMove(NewLocalPos, 1);
        animPlayed = true;
    }
    public void Reset()
    {
        this.transform.DOLocalMove(OriginalPos, 1);
        animPlayed = false;
    }
    public Vector3 GetOriginalPos() { return OriginalPos; }
    public bool HasPlayed() { return animPlayed; }
}
