using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnOnDestroy : MonoBehaviour
{
    public string vfxName;
    public string soundName;
    public int amount;
    public bool quitting;
    private Restart restartScript;
    //bool applicationIsQuitting;
    // Start is called before the first frame update
    // Update is called once per frame
    void Awake()
    {
        quitting = false;
        GameObject temp = GameObject.FindWithTag("EventSystem");
        restartScript = temp.GetComponent<Restart>();
    }


    private void OnDestroy()
    {
        //if the scene is being restarted or the player quits
        if (restartScript.ExitingScene() || quitting){
            return;
        }
        // otherwise play the effect
        else{
            if (soundName != "")
            {
            //if string is not empty, calls audio manager to play sound based on string
              FindObjectOfType<AudioManager>().PlaySoundOneShot(soundName);
            }
            //if vfx string is not empty
            if (vfxName != "")
            {
                for (int i = 0; i < amount; i++)
                {
                    //Calls VFX manager to play desired VFX effect based on string
                    GameObject emit = VFXManager.instance.PlayEffect(vfxName, new Vector3(transform.position.x, transform.position.y, transform.position.z));
                }
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
