using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOverlayShader : MonoBehaviour
{
    private float lowHealthCutoff = .33f;
    private float delayBeforeDissolving = 1f;
    private bool dissolveComplete = false;
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetFloat("_OnOff",0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_LowHP",0);
        gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveEffect", 0);
    }
    void Update()
    {
        float ratio = GetComponentInParent<Pawn>().GetHealth().GetValue() / GetComponentInParent<Pawn>().GetHealth().GetMaxValue();
        float onOffMulti = -12 * (1 - ratio);
        if (ratio < lowHealthCutoff)
        {
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
                float effectVal = 1 - Time.deltaTime*2;
                gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveEffect",effectVal);
                if (effectVal <= 0)
                    Destroy(this.gameObject);
            }

        }
            
    }
    public void FinishDissolve()
    {
        dissolveComplete = true;
    }
}
