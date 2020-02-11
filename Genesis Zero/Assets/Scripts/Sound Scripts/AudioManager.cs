using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;

    List<Sound> Playlist = new List<Sound>();

    // Use this for initialization
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

        sounds = Resources.LoadAll("Sounds", typeof(AudioClip));

        foreach (Sound s in sounds)
        {
            Playlist.Add(s);
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            if (s.playOnAwake)
            {
                s.source.Play();
            }
        }
    }

    void Start()
    {

    }

    public void Play()
    {
        foreach (Sound s in sounds)
        {
            s.source.Play();
        }
    }

    public void Play (string name)
    {
        Sound s = List.Find(instance.Playlist, Sound => Sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Audio: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
