using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSounds : MonoBehaviour
{
    // Start is called before the first frame update

    public float MasterAmbientVolume;
    public float MasterSoundVolume;

    private Object[] allSounds;
    private Object[] ambient;
    private AudioManager aManager;
    void Start()
    {
        if (MasterAmbientVolume == 0 || MasterSoundVolume == 0){
            MasterAmbientVolume = 1f;
            MasterSoundVolume = .15f;
        }
        aManager = FindObjectOfType<AudioManager>();
        allSounds = Resources.LoadAll("Sounds/SFX");
        ambient = Resources.LoadAll("Sounds/Ambient");
        for (int i = 0; i < allSounds.Length; i++)
        {
            // (name of actual file, name to be called by, volume, pitch, bool looping, bool awake)
            aManager.AddSound("SFX/"+allSounds[i].name,allSounds[i].name, MasterSoundVolume, 1f, true, false);
        }
        for (int i = 0; i < ambient.Length; i++)
        {
            // (name of actual file, name to be called by, volume, pitch, bool looping, bool awake);
            aManager.AddTrack("Ambient/"+ambient[i].name,ambient[i].name, MasterAmbientVolume, 1f, true, false);
        }
        aManager.PlayTrack("Ambient/Ambient-1");
        // FindObjectOfType<AudioManager>() searches for an AudioManager object
        // FindObjectOfType<AudioManager>().AddTrack("PLACEHOLDER - Pillar Men theme", "Pillar Men"); // Adds track to a Playlist

        // FindObjectOfType<AudioManager>().PlayTrack("Pillar Men");  // Doesn't exist, generates a warning message

        // FindObjectOfType<AudioManager>().AddSound("PLACEHOLDER Bell - daniel simion", "Bell1"); // Adds sound to a Soundplaylist
        // FindObjectOfType<AudioManager>().AddSound("PLACEHOLDER Bell - daniel simion", "Bell2", 1.0f, 0.5f, true, false); // Adds sound to a Soundplaylist with extra settings
        // FindObjectOfType<AudioManager>().PlaySound("Bell2"); // Plays sound from the Soundplaylist
    }

    // Update is called once per frame
    void Update()
    {
        // if (Time.realtimeSinceStartup >= 5 && play)
        // {
        //     FindObjectOfType<AudioManager>().PlaySoundOneShot("Bell1"); // Plays sound from the Soundplaylist only once
        //     FindObjectOfType<AudioManager>().PlayTrack("Pillar Men"); // Plays track from Playlist
        //     play = false;
        // }

        // if (Time.realtimeSinceStartup >= 12 && !stop1)
        // {
        //     // FindObjectOfType<AudioManager>().ToggleMute("Pillar Men");
        //     FindObjectOfType<AudioManager>().PauseTrack("Pillar Men"); // Pauses specific music track
        //     stop1 = true;
        // }

        // if (Time.realtimeSinceStartup >= 14 && !stop2)
        // {
        //     // FindObjectOfType<AudioManager>().ToggleMute("Pillar Men");
        //     FindObjectOfType<AudioManager>().UnPauseTrack("Pillar Men"); // Unpauses specific music track
        //     stop2 = true;
        // }

        // if (Time.realtimeSinceStartup >= 24 && !stop3)
        // {
        //     FindObjectOfType<AudioManager>().StopTrack("Pillar Men"); // Stops specific music track
        //     FindObjectOfType<AudioManager>().StopSound("Bell2"); // Plays sound from the Soundplaylist
        //     stop3 = true;
        // }
    }
}
