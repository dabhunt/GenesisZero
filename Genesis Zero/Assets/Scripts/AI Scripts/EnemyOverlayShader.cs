using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EnemyOverlayShader : MonoBehaviour
{
    private float lowHealthCutoff = .33f;
    private float delayBeforeDissolving = 0;
    private bool dissolveComplete = false;
    private float CurrentTimeEffect = 0;
    private bool DeathSFXPlayed = false;
    private float hpcurValue;
    private float hpvalueLastFrame;
    private float tweenDuration = .26f;
    private Tween onTween;
    private Tween hpTween;
    private float onOffVal = 0;
    private float hpVal = 0;
    private bool lowHPFlag = false;

    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetFloat("_OnOff", 0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_StunOnOff", 0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_LowHP", 0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveEffect", 0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_TimeEffect", 0);
    }
    void Update()
    {
        if (GetComponentInParent<Pawn>() == null)
            return;
        bool isStunned = GetComponentInParent<Pawn>().IsStunned();
        float ratio = GetComponentInParent<Pawn>().GetHealth().GetValue() / GetComponentInParent<Pawn>().GetHealth().GetMaxValue();
        hpcurValue = GetComponentInParent<Pawn>().GetHealth().GetValue();

        if (isStunned == true)
        {
            gameObject.GetComponent<Renderer>().material.SetFloat("_StunOnOff", 1);
        } else
        {
            gameObject.GetComponent<Renderer>().material.SetFloat("_StunOnOff", 0);
        }

        if (hpcurValue < hpvalueLastFrame && hpcurValue > 0)
        {
            onOffVal = -12;
            hpVal = .15f;
            Invoke("TweenBack", tweenDuration);
        }
        if (ratio < lowHealthCutoff && lowHPFlag == false)
        {
            lowHPFlag = true;
            GameObject sparks = VFXManager.instance.PlayEffectReturn("VFX_LoopingSparks", transform.position, 0, "");
            sparks.transform.SetParent(this.transform);
            //gameObject.GetComponent<Renderer>().material.SetFloat("_OnOff", -12f);
            //gameObject.GetComponent<Renderer>().material.SetFloat("_LowHP", .15f);
        }
        gameObject.GetComponent<Renderer>().material.SetFloat("_OnOff", onOffVal);
        gameObject.GetComponent<Renderer>().material.SetFloat("_LowHP", hpVal);
        hpvalueLastFrame = GetComponentInParent<Pawn>().GetHealth().GetValue();
        if (ratio <= 0)
        {
            if (!DeathSFXPlayed)
            {
                AudioManager.instance.PlayRandomSFXType("EnemyDeath", this.gameObject, .9f, 1.2f, 7f);
                DeathSFXPlayed = true;
            }
            if (!dissolveComplete)
            {
                //float deathDuration = GetComponentInParent<Pawn>().Stats.deathDuration;
                gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveEffect", 1);
                Invoke("FinishDissolve", delayBeforeDissolving);
            }
            else
            {
                float effectVal = 1;
                CurrentTimeEffect = Mathf.Clamp(CurrentTimeEffect + (1 * Time.deltaTime / GetComponentInParent<Pawn>().Stats.deathDuration), 0, 1);
                gameObject.GetComponent<Renderer>().material.SetFloat("_TimeEffect", CurrentTimeEffect);
                gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveEffect", effectVal);
            }

        }

    }
    public void TweenBack()
    {
        onTween = DOTween.To(() => onOffVal, x => onOffVal = x, 0, tweenDuration);
        hpTween = DOTween.To(() => hpVal, x => hpVal = x, 0, tweenDuration);
    }
    public void FinishDissolve()
    {
        dissolveComplete = true;
    }
}
