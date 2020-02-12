using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    List<Sound> Playlist = new List<Sound>();
    List<Sound> Soundlist = new List<Sound>();

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
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //=======================
    // Soundtrack functions
    //=======================

    public void PlayTrack (string name)
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();
        bool found = false;
        
        while (em.MoveNext() && !found)
        {
            if (em.Current.name == name)
            {
                found = true;
                em.Current.source.Play();
            }
        }
        if (!found)
        {
            Debug.LogWarning("Audio: '" + name + "' not found!");
            return;
        }
    }

    public void StopTrack(string name)
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();
        bool found = false;

        while (em.MoveNext() && !found)
        {
            if (em.Current.name == name)
            {
                found = true;
                em.Current.source.Stop();
            }
        }
        if (!found)
        {
            Debug.LogWarning("Audio: '" + name + "' not found!");
            return;
        }
    }

    public void PauseTrack(string name)
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();
        bool found = false;

        while (em.MoveNext() && !found)
        {
            if (em.Current.name == name)
            {
                found = true;
                em.Current.source.  Pause();
            }
        }
        if (!found)
        {
            Debug.LogWarning("Audio: '" + name + "' not found!");
            return;
        }
    }

    public void UnPauseTrack(string name)
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();
        bool found = false;

        while (em.MoveNext() && !found)
        {
            if (em.Current.name == name)
            {
                found = true;
                em.Current.source.UnPause();
            }
        }
        if (!found)
        {
            Debug.LogWarning("Audio: '" + name + "' not found!");
            return;
        }
    }

    public void ToggleTrackMute(string name)
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();
        bool found = false;
        Sound s;

        while (em.MoveNext() && !found)
        {
            if (em.Current.name == name)
            {   
                found = true;
                s = em.Current;
                s.source.mute = !s.source.mute;
            }
        }
        if (!found)
        {
            Debug.LogWarning("Audio: '" + name + "' not found!");
            return;
        }
    }

    // AddTrack() methods:
    // AddTrack(string name, string alias)
    // AddTrack(string name, string alias, bool looping, bool awake)
    // AddTrack(string name, string alias, float vol, float pit, bool looping, bool awake)

    public void AddTrack (string name, string alias)
    {
        AudioClip loadedSound = (AudioClip)Resources.Load("Sounds/" + name, typeof(AudioClip));
        Sound s = new Sound();

        s.name = alias;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.name = alias;
        s.source.clip = loadedSound;
        s.source.volume = 1;
        s.source.pitch = 1;
        s.source.loop = false;
        s.playOnAwake = false;

        if (s.playOnAwake)
        {
            s.source.Play();
        }

        // Add to playlist
        Playlist.Add(s);
    }
    public void AddTrack(string name, string alias, bool looping, bool awake)
    {
        AudioClip loadedSound = (AudioClip)Resources.Load("Sounds/" + name, typeof(AudioClip));
        Sound s = new Sound();

        s.name = alias;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.name = alias;
        s.source.clip = loadedSound;
        s.source.volume = 1;
        s.source.pitch = 1;
        s.source.loop = looping;
        s.playOnAwake = awake;

        if (s.playOnAwake)
        {
            s.source.Play();
        }

        // Add to playlist
        Playlist.Add(s);
    }
    public void AddTrack(string name, string alias, float vol, float pit, bool looping, bool awake)
    {
        AudioClip loadedSound = (AudioClip)Resources.Load("Sounds/" + name, typeof(AudioClip));
        Sound s = new Sound();

        s.name = alias;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.name = alias;
        s.source.clip = loadedSound;
        s.source.volume = vol;
        s.source.pitch = pit;
        s.source.loop = looping;
        s.playOnAwake = awake;

        if (s.playOnAwake)
        {
            s.source.Play();
        }

        // Add to playlist
        Playlist.Add(s);
    }

    //=================
    // Sound functions
    //=================

    public void PlaySound(string name)
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();
        bool found = false;

        while (em.MoveNext() && !found)
        {
            if (em.Current.name == name)
            {
                found = true;
                em.Current.source.Play();
            }
        }
        if (!found)
        {
            Debug.LogWarning("Audio: '" + name + "' not found!");
            return;
        }
    }

    public void StopSound(string name)
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();
        bool found = false;

        while (em.MoveNext() && !found)
        {
            if (em.Current.name == name)
            {
                found = true;
                em.Current.source.Stop();
            }
        }
        if (!found)
        {
            Debug.LogWarning("Audio: '" + name + "' not found!");
            return;
        }
    }

    public void PlaySoundOneShot(string name)
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();
        bool found = false;

        while (em.MoveNext() && !found)
        {
            if (em.Current.name == name)
            {
                found = true;
                em.Current.source.PlayOneShot(em.Current.source.clip, 1.0f);
            }
        }
        if (!found)
        {
            Debug.LogWarning("Audio: " + name + " not found!");
            return;
        }
    }

    // AddSound() methods:
    // AddSound(string name, string alias)
    // AddSound(string name, string alias, bool looping, bool awake)
    // AddSound(string name, string alias, float vol, float pit, bool looping, bool awake)

    public void AddSound(string name, string alias)
    {
        AudioClip loadedSound = (AudioClip)Resources.Load("Sounds/" + name, typeof(AudioClip));
        Sound s = new Sound();

        s.name = alias;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.name = alias;
        s.source.clip = loadedSound;
        s.source.volume = 1;
        s.source.pitch = 1;
        s.source.loop = false;
        s.playOnAwake = false;

        if (s.playOnAwake)
        {
            s.source.Play();
        }

        // Add to playlist
        Soundlist.Add(s);
    }

    public void AddSound(string name, string alias, bool looping, bool awake)
    {
        AudioClip loadedSound = (AudioClip)Resources.Load("Sounds/" + name, typeof(AudioClip));
        Sound s = new Sound();

        s.name = alias;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.name = alias;
        s.source.clip = loadedSound;
        s.source.volume = 1;
        s.source.pitch = 1;
        s.source.loop = looping;
        s.playOnAwake = awake;

        if (s.playOnAwake)
        {
            s.source.Play();
        }

        // Add to playlist
        Soundlist.Add(s);
    }

    public void AddSound(string name, string alias, float vol, float pit, bool looping, bool awake)
    {
        AudioClip loadedSound = (AudioClip)Resources.Load("Sounds/" + name, typeof(AudioClip));
        Sound s = new Sound();

        s.name = alias;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.name = alias;
        s.source.clip = loadedSound;
        s.source.volume = vol;
        s.source.pitch = pit;
        s.source.loop = looping;
        s.playOnAwake = awake;

        if (s.playOnAwake)
        {
            s.source.Play();
        }

        // Add to playlist
        Soundlist.Add(s);
    }
}
