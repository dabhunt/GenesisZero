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

    private float setVolumeMaster;
    private float setVolumeMusic;
    private float setVolumeSound;
    private double startTime;

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

        setVolumeMaster = AudioListener.volume;
        setVolumeMusic = 1.0f;
        setVolumeSound = 1.0f;
        startTime = AudioSettings.dspTime;

    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //=========================
    // Global Audio Functions
    //=========================

    public void TogglePauseAll()
    {
        AudioListener.pause = !AudioListener.pause;
    }

    public void MuteAll()
    {
        AudioListener.volume = 0;
        /*
        if (AudioListener.volume != 0)
        {
            setVolume = AudioListener.volume;
            AudioListener.volume = 0;
            Debug.LogWarning("Muted, previous volume saved");
        }
        else
        {
            AudioListener.volume = setVolume;
            Debug.LogWarning("Unmuted");
        }*/
    }

    public void AdjustVolumeAll(float vol)
    {
        AudioListener.volume += vol;
    }

    public void SetVolumeAll(float vol)
    {
        AudioListener.volume = vol;
    }

    //=======================
    // Soundtrack functions
    //=======================

    public void AdjustVolumeMusic(float vol)
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();

        setVolumeMusic += vol;
        while (em.MoveNext())
        {
            em.Current.source.volume += vol;
        }
    }

    public void SetVolumeMusic(float vol)
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();

        setVolumeMusic = vol;
        while (em.MoveNext())
        {
            em.Current.source.volume = vol;
        }
    }

    public void ClearAllMusic()
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();

        while (em.MoveNext())
        {
            Destroy(em.Current.source);
        }
        Playlist.Clear();
    }

    public void PauseAllMusic()
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();

        while (em.MoveNext())
        {
            em.Current.source.Pause();
        }
    }

    public void UnPauseAllMusic()
    {
        List<Sound>.Enumerator em = Playlist.GetEnumerator();

        while (em.MoveNext())
        {
            em.Current.source.UnPause();
        }
    }

    public void PlayTrack (string name)
    {
       // bool active = false;
        AudioClip loadedSound = (AudioClip)Resources.Load("Sounds/" + name, typeof(AudioClip));
        Sound s = new Sound();

        s.name = name;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.name = name;
        s.source.clip = loadedSound;
        s.source.volume = 1;
        s.source.pitch = 1;
        s.source.loop = false;
        s.playOnAwake = true;

        double duration = (double)s.source.clip.samples / s.source.clip.frequency;

        // Add to playlist
        if (Playlist.Count > 0)
        {
            Playlist.Add(s);

            s.source.PlayScheduled(startTime);
            startTime += duration;
            Debug.LogWarning("Duration: " + startTime);
            Debug.LogWarning("End of playlist, adding: " + name);
        }
        else
        {
            Playlist.Add(s);
            if (s.playOnAwake)
            {
                s.source.Play();
                startTime += duration;
            }
            Debug.LogWarning("Duration: " + startTime);
            Debug.LogWarning("Playlist empty, adding: " + name);
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
                Destroy(em.Current.source);
                Playlist.Remove(em.Current);
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
                em.Current.source.Pause();
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

    public void AdjustVolumeSound(float vol)
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();

        setVolumeSound += vol;
        while (em.MoveNext())
        {
            em.Current.source.volume += vol;
        }
    }

    public void SetVolumeSound(float vol)
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();

        setVolumeSound = vol;
        while (em.MoveNext())
        {
            em.Current.source.volume = vol;
        }
    }

    public void ClearAllSound()
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();

        while (em.MoveNext())
        {
            Destroy(em.Current.source);
        }
        Soundlist.Clear();
    }

    public void PauseAllSound()
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();

        while (em.MoveNext())
        {
            em.Current.source.Pause();
        }
    }

    public void UnPauseAllSound()
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();

        while (em.MoveNext())
        {
            em.Current.source.UnPause();
        }
    }

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
    public void StopAllSounds()
    {
        List<Sound>.Enumerator em = Soundlist.GetEnumerator();
        while (em.MoveNext())
        {
            em.Current.source.Stop();
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
