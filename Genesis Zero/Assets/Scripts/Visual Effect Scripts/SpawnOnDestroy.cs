using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SpawnOnDestroy : MonoBehaviour
{
    public string vfxName;
    public float vfxScaleMultiplier;
    public string sound;
    public bool quitting;
    private Restart restartScript;
    //.3f is 30% drop chance
    public int minEssenceDrop;
    public int maxEssenceDrop;
    public float ModifierDropChance;
    public float minDropVelocity;
    public float maxDropVelocity;
    public GameObject EssencePrefab;

    private SkillObject ModDrop;
    private Player player;
    private bool canDrop = true;
    private AudioManager aManager;
    //bool applicationIsQuitting;
    // Start is called before the first frame update
    // Update is called once per frame
    void Start()
    {
        quitting = false;
        GameObject temp = GameObject.FindWithTag("StateManager");
        if (temp != null)
        {
            restartScript = temp.GetComponent<Restart>();
        }

        temp = GameObject.FindWithTag("Player");
        if (temp != null)
        {
            player = temp.GetComponent<Player>();
        }

        //populate the list of potential modifier drops
        aManager = FindObjectOfType<AudioManager>();
        if (vfxScaleMultiplier <= 0)
        {
            vfxScaleMultiplier = 1;
        }
    }
    private void OnDestroy()
    {
        //if the scene is being restarted or the player quits
        if (player == null || restartScript == null || restartScript.ExitingScene() || quitting)
        {
            quitting = true;
            return;
        }
        // otherwise play the effect
        else
        {
            if (sound != null && sound != "")
            {
                if (aManager != null)
                {
                    //plays sound at this location
                    aManager.PlayRandomSFXType(sound, this.gameObject, .2f);
                }
            }
            //if vfx string is not empty
            if (vfxName != "")
            {
                //Calls VFX manager to play desired VFX effect based on string
                GameObject emit = VFXManager.instance.PlayEffect(vfxName, new Vector3(transform.position.x, transform.position.y, transform.position.z), 0f, vfxScaleMultiplier);
            }
            if (canDrop)
            {
                int amount = Random.Range(minEssenceDrop, maxEssenceDrop);
                for (int i = 0; i < amount; i++)
                {
                    // if essence drop chance exceeds the random value from 0 to 1.0f, it drops
                    float offset = i / (1.5f + Random.value * 4);
                    GameObject essence = Instantiate(EssencePrefab, new Vector3(transform.position.x + offset, transform.position.y + offset, -4), Quaternion.identity);
                    essence = Drop(essence);
                    //Destroy(rb);
                }
                // if modifier drop chance exceeds the random value from 0 to 1.0f, it drops
                if (ModifierDropChance > Random.value && player != null)
                {
                    SkillManager sk = player.GetSkillManager();
                    SkillObject mod = sk.GetRandomModByChance();
                    GameObject modObj = sk.SpawnMod(transform.position, mod.name);
                }
            }
        }
    }

    public GameObject Drop(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        //float force = Random.Range(minDropVelocity, maxDropVelocity);
        float force = .5f;
        //random rotation and force applied
        obj.transform.rotation = Random.rotation;
        rb.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * force;
        return obj;
    }

    void OnApplicationQuit()
    {
        quitting = true;
        Destroy(this.GetComponent<SpawnOnDestroy>());
        Destroy(gameObject);
    }

    public void isQuitting()
    {
        quitting = true;
    }

    public void noDrops()
    {
        canDrop = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //quitting = true;
    }
}
