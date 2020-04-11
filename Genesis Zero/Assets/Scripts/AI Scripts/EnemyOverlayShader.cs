using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOverlayShader : MonoBehaviour
{
    private float lowHealthCutoff = .33f;
    private float delayBeforeDissolving = 0;
    private bool dissolveComplete = false;
    private float CurrentTimeEffect = 0;

    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetFloat("_OnOff", 0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_LowHP", 0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveEffect", 0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_TimeEffect", 0);
    }
    void Update()
    {
        float ratio = GetComponentInParent<Pawn>().GetHealth().GetValue() / GetComponentInParent<Pawn>().GetHealth().GetMaxValue();
        float onOffMulti = -12 * (1 - ratio);
        if (ratio < lowHealthCutoff)
        {
            //VFXManager.instance.PlayEffect("VFX_LoopingSparks", transform.position);
            gameObject.GetComponent<Renderer>().material.SetFloat("_OnOff", onOffMulti);
            gameObject.GetComponent<Renderer>().material.SetFloat("_LowHP", .15f);
        }

        if (ratio <= 0)
        {
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
    public void FinishDissolve()
    {
        dissolveComplete = true;
    }
}
