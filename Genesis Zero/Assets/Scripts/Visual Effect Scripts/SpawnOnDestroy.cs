using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SpawnOnDestroy : MonoBehaviour
{
    public string vfxName;
    public string[] sounds = new string[0];
    public bool quitting;
    private Restart restartScript;
    //.3f is 30% drop chance
    public float EssenceDropChance;
    public float ModifierDropChance;
    public GameObject EssencePrefab;
    public GameObject ModifierPrefab;

    public SkillObject[] ModTypes;
   
    private AudioManager aManager;
    //bool applicationIsQuitting;
    // Start is called before the first frame update
    // Update is called once per frame
    void Awake()
    {
        quitting = false;
        GameObject temp = GameObject.FindWithTag("EventSystem");
        restartScript = temp.GetComponent<Restart>();
        aManager = FindObjectOfType<AudioManager>();
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
              aManager.PlaySoundOneShot(sounds[rng]);
            }
            //if vfx string is not empty
            if (vfxName != "")
            {
                //Calls VFX manager to play desired VFX effect based on string
                GameObject emit = VFXManager.instance.PlayEffect(vfxName, new Vector3(transform.position.x, transform.position.y, transform.position.z));
                
            }
             // if essence drop chance exceeds the random value from 0 to 1.0f, it drops
            if (EssenceDropChance >= Random.value)
            {
                Instantiate(EssencePrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
            // if modifier drop chance exceeds the random value from 0 to 1.0f, it drops
            if (ModifierDropChance > Random.value)
            {
                GameObject randMod = Instantiate(ModifierPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                SkillPickup pickup = randMod.GetComponent<SkillPickup>();
                pickup.skill = ModTypes[Random.Range(0,ModTypes.Length-1)];
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
