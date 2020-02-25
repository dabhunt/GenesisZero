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
   
    private AudioManager aManager;
    //bool applicationIsQuitting;
    // Start is called before the first frame update
    // Update is called once per frame
    void Start()
    {
        quitting = false;
        GameObject temp = GameObject.FindWithTag("EventSystem");
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
        if (restartScript.ExitingScene() || quitting){
            return;
        }
        // otherwise play the effect
        else{
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
             // if essence drop chance exceeds the random value from 0 to 1.0f, it drops
            int amount = Random.Range(minEssenceDrop, maxEssenceDrop);
            //print("dropping "+amount+" essence");
            for (int i = 0; i < amount; i++){
                //print("dropping some essence...");
                GameObject essence = Instantiate(EssencePrefab, new Vector3(transform.position.x, transform.position.y, 3), Quaternion.identity);
                float force = Random.Range(minDropVelocity, maxDropVelocity);
                Rigidbody rb = essence.GetComponent<Rigidbody>();
                rb.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * force;
                //Destroy(rb);
            }
            // if modifier drop chance exceeds the random value from 0 to 1.0f, it drops
            if (ModifierDropChance > Random.value)
            {
                GameObject randMod = Instantiate(ModifierPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                SkillPickup pickup = randMod.GetComponent<SkillPickup>();
                //gets random skill from skillmanagers resource folder
                pickup.skill = player.GetSkillManager().GetRandomSkill();
            }
        }

    }

    void OnApplicationQuit()
    {
        quitting = true;
    }
    public void isQuitting(){
        quitting = true;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //quitting = true;
    }

}
