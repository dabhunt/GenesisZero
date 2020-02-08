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
    public void PlayEffect(string name, Vector3 position, float delay, string emitter)
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
    }

    public void PlayEffect(string name, Vector3 position, float delay)
    {
        PlayEffect(name, position, delay, "");
    }

    public void PlayEffect(string name, Vector3 position)
    {
        PlayEffect(name, position, 0, "");
    }

}
