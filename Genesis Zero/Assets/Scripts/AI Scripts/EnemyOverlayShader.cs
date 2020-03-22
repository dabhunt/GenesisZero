using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOverlayShader : MonoBehaviour
{
    private float lowHealthCutoff = .33f;
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetFloat("_OnOff", 1);
    }
    void Update()
    {
        float ratio = GetComponentInParent<Pawn>().GetHealth().GetValue() / GetComponentInParent<Pawn>().GetHealth().GetMaxValue();
        float onOffMulti = -12 * (1 - ratio); 
        if (ratio < lowHealthCutoff)
            gameObject.GetComponent<Renderer>().material.SetFloat("_OnOff", onOffMulti);
    }
}
