using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SpawnOnDestroy : MonoBehaviour
{
    public string vfxName;
    public float vfxScaleMultiplier;
    public string[] sounds = new string[0];
    public bool quitting;
    private Restart restartScript;
    //.3f is 30% drop chance
    public int minEssenceDrop;
    public int maxEssenceDrop;
    public float ModifierDropChance;
    public float minDropVelocity;
    public float maxDropVelocity;
    public GameObject EssencePrefab;
    public GameObject ModifierPrefab;

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
         restartScript = temp.GetComponent<Restart>();
        temp = GameObject.FindWithTag("Player");
        player = temp.GetComponent<Player>();
        //populate the list of potential modifier drops
        aManager = FindObjectOfType<AudioManager>();
        if (vfxScaleMultiplier <= 0){
            vfxScaleMultiplier = 1;
        }
    }
    private void OnDestroy()
    {
        //if the scene is being restarted or the player quits
        if (restartScript == null || restartScript.ExitingScene() || quitting){
            return;
        }
        // otherwise play the effect
        else
        {
            if (sounds.Length > 0)
            {
                //if string is not empty, calls audio manager to play sound based on string
                // note this only works if audio manager has been told to load the sound at the beginnning of the game
                int rng = Random.Range(1, sounds.Length);
                rng --;
                if (aManager != null){
                    aManager.PlaySoundOneShot(sounds[rng]);
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
                    GameObject essence = Instantiate(EssencePrefab, new Vector3(transform.position.x, transform.position.y, -4), Quaternion.identity);
                    essence = Drop(essence);
                    //Destroy(rb);
                }
                // if modifier drop chance exceeds the random value from 0 to 1.0f, it drops
                if (ModifierDropChance > Random.value)
                {
                    GameObject randMod = Instantiate(ModifierPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                    randMod = Drop(randMod);
                    SkillPickup pickup = randMod.GetComponent<SkillPickup>();
                    pickup.skill = player.GetSkillManager().GetRandomSkill();
                    //gets random skill from skillmanagers resource folder 
                }
             
            }

        }

    }
    public GameObject Drop(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        float force = Random.Range(minDropVelocity, maxDropVelocity);
        //random rotation and force applied
        obj.transform.rotation = Random.rotation;
        rb.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * force/2;
        return obj;       
    }
    void OnApplicationQuit()
    {
        quitting = true;
        Destroy(this.GetComponent<SpawnOnDestroy>());
        Destroy(gameObject);
    }
    public void isQuitting(){
        quitting = true;
    }
    public void noDrops(){
        canDrop = false;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //quitting = true;
    }

}
