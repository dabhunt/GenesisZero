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
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /**
     * Base function for VFX
     * name: the name of the effect in resources/effects
     * position: where it spawns
     * delay: how long until it plays
     * emitter: name of the other effect when it dies
     */
    public GameObject PlayEffect(string name, Vector3 position, float delay, string emitter)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>("Effects/" + name), position, Quaternion.identity);

        VFXScript vfx = effect.GetComponent<VFXScript>();
        if (vfx != null && delay > 0)
        {
            vfx.delay = delay;
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
        return PlayEffect(name, position, delay, "");
    }

    public GameObject PlayEffect(string name, Vector3 position)
    {
        return PlayEffect(name, position, 0, "");
    }

    public void PlayGraphEffect(string name, Vector3 position)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>("Effects/GraphVFX/" + name), position, Quaternion.identity);
    }

    public GameObject PlayEffectReturn(string name, Vector3 position, float delay, string emitter)
    {
        return PlayEffect(name, position, delay, emitter);
    }
}
