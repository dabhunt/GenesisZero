using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOverlayShader : MonoBehaviour
{
    private float lowHealthCutoff = .33f;
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
            gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveEffect", 1);
        }
            
    }
}
