﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SpawnOnDestroy : MonoBehaviour
{
    public string vfxName;
    public float vfxScaleMultiplier;
    public string sound;
    public float volume = 1;
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
    private bool beingDestroyed = false;


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
        aManager = AudioManager.instance;
        if (vfxScaleMultiplier <= 0)
        {
            vfxScaleMultiplier = 1;
        }
    }
    private void OnDestroy()
    {
        beingDestroyed = true;
        //if a scene is being loaded or the player quits/dies
        if (player == null || SceneManager.sceneCount > 1 || restartScript == null || restartScript.ExitingScene() || quitting)
        {
            quitting = true;
            Destroy(gameObject.GetComponent<SpawnOnDestroy>());
            return;
        }
        // otherwise play the effect
        if (sound != null && sound != "" && sound != " ")
        {
            if (aManager != null)
            {
                //plays sound at this location
                aManager.PlayRandomSFXType(sound, this.gameObject, .8f, 1.2f , volume);
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
            if (maxEssenceDrop < 1)
                return;
            int amount = Random.Range(minEssenceDrop, maxEssenceDrop);
                //Goes into essence animator to determine size.
            //for (int i = 0; i < amount; i++)
            //{
            // if essence drop chance exceeds the random value from 0 to 1.0f, it drops
            float offset = (1.5f + Random.value * 4);
            GameObject essence = Instantiate(EssencePrefab, new Vector3(transform.position.x, transform.position.y, -4), Quaternion.identity);
            essence.GetComponent<EssenceScript>().Amount = amount;
            essence = Drop(essence);
                //Destroy(rb);
            //}
            // if modifier drop chance exceeds the random value from 0 to 1.0f, it drops
            if (ModifierDropChance > Random.value && player != null)
            {
                SkillManager sk = player.GetSkillManager();
                SkillObject mod = sk.GetRandomModByChance();
                GameObject modObj = sk.SpawnMod(transform.position + Vector3.up, mod.name);
            }
       
        }
    }

    public GameObject Drop(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        //float force = Random.Range(minDropVelocity, maxDropVelocity);
        float force = .5f;
        //random rotation and force applied
        rb.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * force;
        return obj;
    }

    void OnApplicationQuit()
    {
        quitting = true;
        Destroy(this.GetComponent<SpawnOnDestroy>());
        Destroy(gameObject);
    }
    public bool isBeingDestroyed(){ return beingDestroyed; }
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
