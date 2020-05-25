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
    private ParticleSystem ps;
    private bool played;
    public float delay;
    public GameObject emitter;

    [HideInInspector]
    public float duration = 0;

    // Use this for initialization
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            ps = gameObject.GetComponentInChildren<ParticleSystem>();
        }
        if (ps != null && ps.isPlaying == false && duration != 0)
        {
            var main = ps.main;
            main.duration = duration;
        }
        played = false;
    }
    void Update()
    {
        if (ps && delay <= 0 && played == false)
        {
			try
			{
				ps.Play();
			}
			catch { }

            played = true;
        }
        else
        {
            delay -= Time.deltaTime;
        }

        if (ps && ps.isPlaying == false && played == true)
        {
            if (emitter)
            {
                GameObject effect = Instantiate(emitter, ps.transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    /**
     * Sets the gameobject that gets emitted by the the VFX when it is destroyed
     */
    public void SetEmitter(GameObject emit)
    {
        emitter = emit;
    }
}
