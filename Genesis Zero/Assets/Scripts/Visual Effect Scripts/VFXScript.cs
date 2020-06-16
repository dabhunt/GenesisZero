using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * VFXScript is a script that should be attached to all objects/effects in Resources/Effects
 * It allows for the VFXManager to create and edit objects in the folder.
 */
public class VFXScript : MonoBehaviour
{
    private ParticleSystem[] ps;
    private bool played;
    public float delayKill;
    public GameObject emitter;

    [HideInInspector]
    public float duration = 0;
    [HideInInspector]
    public float longestDur = 0;
    // Use this for initialization
    void Start()
    {
        ps = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in ps)
        {
            if (p != null)
            {
                
                if (longestDur < p.main.duration)
                {
                    longestDur = p.main.duration; //set longestDur
                }
            }
        }
        Invoke("Kill", longestDur+delayKill);
        played = false;
    }
    private void Kill()
    {
        played = true;
        if (emitter)
        {
            GameObject effect = Instantiate(emitter, ps[0].transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    /**Sets the gameobject that gets emitted by the the VFX when it is destroyed
     */
    public void SetEmitter(GameObject emit)
    {
        emitter = emit;
    }
}
