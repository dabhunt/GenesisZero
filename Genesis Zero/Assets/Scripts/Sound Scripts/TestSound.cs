﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    // Start is called before the first frame update
    //public AudioClip[] allSounds;
    //public AudioClip[] ambient;
    //private AudioManager instance;
    //public float MasterAmbientVolume;
    //public float MasterSoundVolume;
    public AudioListener audioMaster;
    void Awake()
    {
        // FindObjectOfType<AudioManager>() searches for an AudioManager object
        //FindObjectOfType<AudioManager>().AddTrack("PLACEHOLDER - Pillar Men theme", "Pillar Men"); // Adds track to a Playlist
        //FindObjectOfType<AudioManager>().PlayTrack("PLACEHOLDER Bell - daniel simion");
       // FindObjectOfType<AudioManager>().PlayTrack("SFX_Explosion"); 
        //FindObjectOfType<AudioManager>().PlayTrack("PLACEHOLDER - Pillar Men theme");  // Doesn't exist, generates a warning message

        // FindObjectOfType<AudioManager>().AddSound("PLACEHOLDER Bell - daniel simion", "Bell1"); // Adds sound to a Soundplaylist
        // FindObjectOfType<AudioManager>().AddSound("PLACEHOLDER Bell - daniel simion", "Bell2", 1.0f, 0.5f, true, false); // Adds sound to a Soundplaylist with extra settings
        // FindObjectOfType<AudioManager>().PlaySound("Bell2"); // Plays sound from the Soundplaylist
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
