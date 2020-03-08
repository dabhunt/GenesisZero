using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOverlayShader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Renderer>().material.SetFloat("_ColorStrength", 1 - GetComponentInParent<Pawn>().GetHealth().GetValue() / GetComponentInParent<Pawn>().GetHealth().GetMaxValue());
    }
}
