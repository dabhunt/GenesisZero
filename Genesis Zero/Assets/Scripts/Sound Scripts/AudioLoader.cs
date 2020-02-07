using UnityEngine;
using System.Collections;

public class AudioLoader : MonoBehaviour
{

    Object[] myMusic; // declare this as Object array
    int trackNum;

    void Awake()
    {
        // Loads all Audio found in the "Resources/Sounds" folder; can be changed
        myMusic = Resources.LoadAll("Sounds", typeof(AudioClip));
        GetComponent<AudioSource>().clip = myMusic[0] as AudioClip;
        trackNum = 0;
    }

    void Start()
    {
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying && trackNum < myMusic.Length-1)
        {
            Debug.LogWarning(myMusic.Length);
            trackNum++;
            playNextClip(trackNum);
        }
    }

    void playRandomMusic()
    {
        GetComponent<AudioSource>().clip = myMusic[Random.Range(0, myMusic.Length)] as AudioClip;
        GetComponent<AudioSource>().Play();
    }

    void playNextClip(int track)
    {
        GetComponent<AudioSource>().clip = myMusic[track] as AudioClip;
        GetComponent<AudioSource>().Play();
    }
}