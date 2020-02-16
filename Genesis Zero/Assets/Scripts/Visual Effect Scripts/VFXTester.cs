using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * VFXTester is a script that calls the VFXManager to instantiate the effect with the same name
 * in the public fields Name by pressing "V". It can also apply a delay to the VFX
 */
public class VFXTester : MonoBehaviour
{
    [Header("VFX")]
    public string Name;
    public float delay;
    [Header("VFX Graph")]
    public string Name2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            //VFXManager.instance.PlayEffect(Name, transform.position, 0, "Emit"); // You could set a emitter in script. Probably should be in editor
            VFXManager.instance.PlayEffect(Name, transform.position, delay);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            VFXManager.instance.PlayGraphEffect(Name2, transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, .5f);
    }
}
