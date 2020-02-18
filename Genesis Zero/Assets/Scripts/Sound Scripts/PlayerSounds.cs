using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    // Start is called before the first frame update
    public string[] sounds = new string[0];
    private PlayerController PC;
    private AudioManager aManager;
    public string gunshot;
    public string jump;
    public string land;

    //bool applicationIsQuitting;
    // Start is called before the first frame update
    // Update is called once per frame
    void Awake()
    {
        
        GameObject temp = GameObject.FindWithTag("Player");
        PC = temp.GetComponent<PlayerController>();
        aManager = FindObjectOfType<AudioManager>();
    }

    private void OnDestroy()
    {

            if (sounds.Length > 0)
            {
            //if string is not empty, calls audio manager to play sound based on string
            // note this only works if audio manager has been told to load the sound at the beginnning of the game
                int rng = Random.Range(1, sounds.Length);
                rng --;
              aManager.PlaySoundOneShot(sounds[rng]);
            }
	}
}
