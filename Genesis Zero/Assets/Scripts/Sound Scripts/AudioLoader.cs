using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioLoader : MonoBehaviour
{

    Object[] myMusic; // declare this as Object array
    int trackNum;


    public static AudioLoader instance;
    public List<AudioClip> Playlist = new List<AudioClip>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        // Loads all Audio found in the "Resources/Sounds" folder; can be changed
        myMusic = Resources.LoadAll("Sounds", typeof(AudioClip));
        GetComponent<AudioSource>().clip = myMusic[0] as AudioClip;
        trackNum = 0;
        foreach (AudioClip s in myMusic)
        {
            Playlist.Add(s);
        }

    }

    void Start()
    {
        //GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!GetComponent<AudioSource>().isPlaying && trackNum < myMusic.Length-1)
        {
            Debug.LogWarning(myMusic.Length);
            trackNum++;
            PlayNextClip(trackNum);
        }*/
    }
    /*
    void PlayRandomMusic()
    {
        GetComponent<AudioSource>().clip = myMusic[Random.Range(0, myMusic.Length)] as AudioClip;
        GetComponent<AudioSource>().Play();
    }
    
    void PlayNextClip(int track)
    {
        GetComponent<AudioSource>().clip = myMusic[track] as AudioClip;
        GetComponent<AudioSource>().Play();
    }*/

    public void PlaySound(int track)
    {
        GetComponent<AudioSource>().clip = myMusic[track] as AudioClip;
        GetComponent<AudioSource>().Play();
    }
    /*
    public void PlayTrackImmediate(string name)
    {
        if (Playlist.Find(instance.audioFiles, AudioFile => AudioFile.audioName == name))
        {
            GetComponent<AudioSource>().clip = Playlist.Find(instance.Playlist, AudioFile => AudioFile.audioName == name) as AudioClip;
            GetComponent<AudioSource>().Play();
        }
    }*/
}