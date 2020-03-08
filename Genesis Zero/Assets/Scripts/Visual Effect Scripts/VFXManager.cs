using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * VFXManager is a singleton class that handles instanstiating effects from
 * calls form other scripts in scene
 */
public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // I am very proud of this, I came up with it myself
    private void RecursiveChildScale(Transform t, float scaleMultiplier)
    {
        foreach (Transform child in t)
        {
            child.localScale = new Vector3(child.localScale.x * scaleMultiplier, child.localScale.y * scaleMultiplier, child.localScale.z * scaleMultiplier);
            if (child.childCount > 0)
            {
                RecursiveChildScale(child, scaleMultiplier);
            }
        }
    }
    public GameObject ChangeColor(GameObject vfx, Color color)
    {
        RecursiveChildColor(vfx.transform, color);
        return vfx;
    }
    public void RecursiveChildColor(Transform t, Color color)
    {
        foreach (Transform child in t)
        {
            //if the child has a particle system, trail renderer, or mod related shader,  set those values to the desired color
            if (child.gameObject.GetComponent<ParticleSystem>() != null)
            {
                var main = child.gameObject.GetComponent<ParticleSystem>().main;
                main.startColor = color;
            }
            if (child.gameObject.GetComponent<TrailRenderer>() != null)
            {
                child.gameObject.GetComponent<TrailRenderer>().startColor = color;
            }
            Renderer r = child.gameObject.GetComponent<Renderer>();
            if (r != null && r.material.HasProperty("_EmissiveColor"))
            {
                child.gameObject.GetComponent<Renderer>().material.SetColor("_EmissiveColor", color);
            }
            if (r != null && r.material.HasProperty("_ShallowColor"))
            {
              
                child.gameObject.GetComponent<Renderer>().material.SetColor("_ShallowColor", color);
            }
            if (child.childCount > 0)
            {
                RecursiveChildColor(child, color);
            }
        }
    }
    /**
  * Base function for VFX
  * name: the name of the effect in resources/effects
  * position: where it spawns
  * delay: how long until it plays
  * emitter: name of the other effect when it dies
  */
    public GameObject PlayEffect(string name, Vector3 position, float delay, string emitter, float duration, float scaleMultiplier)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>("Effects/" + name), position, Quaternion.identity);
        Transform tEffect = effect.transform;
        RecursiveChildScale(tEffect, scaleMultiplier);
        VFXScript vfx = effect.GetComponent<VFXScript>();
        if (vfx != null && delay > 0)
        {
            vfx.delay = delay;
        }
        if (vfx != null && duration != 0)
        {
            vfx.duration = duration;
        }

        GameObject emit = Resources.Load<GameObject>("Effects/" + emitter);
        if (emit != null)
        {
            vfx.SetEmitter(emit);
        }

        return effect;
    }

    public GameObject PlayEffect(string name, Vector3 position, float delay)
    {
        return PlayEffect(name, position, delay, "", 0,1);
    }

    public GameObject PlayEffect(string name, Vector3 position)
    {
        return PlayEffect(name, position, 0, "", 0,1);
    }
    public GameObject PlayEffect(string name, Vector3 position, float delay, float scaleMultiplier)
    {
        return PlayEffect(name, position, 0, "", 0, scaleMultiplier);
    }
    public void PlayGraphEffect(string name, Vector3 position)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>("Effects/GraphVFX/" + name), position, Quaternion.identity);
    }

    public GameObject PlayEffectReturn(string name, Vector3 position, float delay, string emitter)
    {
        return PlayEffect(name, position, delay, emitter, 0,1);
    }

    public GameObject PlayEffectForDuration(string name, Vector3 position, float delay, string emitter, float duration)
    {
        return PlayEffect(name, position, delay, emitter, duration,1);
    }

    public GameObject PlayEffectForDuration(string name, Vector3 position, float duration)
    {
        return PlayEffect(name, position, 0, "", duration,1);
    }
}
