using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

    public void TimeEffect(float duration,float intensity)
    {
        GetComponentInChildren<VFX_TimeDilation>().timeEffect(duration);
        var distort = ScriptableObject.CreateInstance<LensDistortion>();
        distort.enabled.Override(true);
        distort.intensity.Override(1f);
    }
    public void RecursiveChildScale(Transform t, float scaleMultiplier)
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
    //changes the trail color of the bullet vfx when passed the name of the transform name of the trail, and starting and ending colors
    //use Color.black to not change the color
    public GameObject ChangeMainTrail(GameObject vfx, Color startColor, Color endColor)
    {
        GameObject trail = vfx.transform.Find("vfx_Bullet").Find("Trail").gameObject;
        if (trail == null || trail.GetComponent<TrailRenderer>() == null)
            return vfx;
        //color.black is treated like a null since you can't pass null colors
        if (startColor != Color.black)
            trail.GetComponent<TrailRenderer>().startColor = startColor;
        if (endColor != Color.black)
            trail.GetComponent<TrailRenderer>().endColor = endColor;
        return vfx;
    }
    public GameObject ChangeInnerTrail(GameObject vfx, Color color)
    {
        GameObject trail = vfx.transform.Find("vfx_Bullet").Find("Bullet").gameObject;
        if (trail == null || trail.GetComponent<ParticleSystem>() == null)
            return vfx;
        var parTrail = trail.GetComponent<ParticleSystem>().trails;
        parTrail.colorOverTrail = NewGradient(color, color);
        print("Inner trail changing");
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
                //child.gameObject.GetComponent<ParticleSystem>().SetTrails(trails);
                main.startColor = color;
            }
            if (child.gameObject.GetComponent<TrailRenderer>() != null)
            {
                child.gameObject.GetComponent<TrailRenderer>().startColor = color;
                child.gameObject.GetComponent<TrailRenderer>().endColor = color;
                //child.gameObject.GetComponent<TrailRenderer>().endColor = Color.black;

            }
            Renderer r = child.gameObject.GetComponent<Renderer>();
            if (r != null)
            { 
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_EmissiveColor"))
                    {
                        child.gameObject.GetComponent<Renderer>().material.SetColor("_EmissiveColor", color);
                    }
                    if (mat.HasProperty("_ShallowColor"))
                    {
                        child.gameObject.GetComponent<Renderer>().material.SetColor("_ShallowColor", color);
                    }
                }
            }
            if (child.childCount > 0)
            {
                RecursiveChildColor(child, color);
            }
        }
    }
    public Gradient NewGradient(Color startColor, Color endColor)
    {
        Gradient gradient;
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;
        gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = startColor;
        colorKey[0].time = 0.0f;
        colorKey[1].color = endColor;
        colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
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
