﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioManager instance;
    public GameObject TestObject;

    void Awake()
    {
        // FindObjectOfType<AudioManager>() searches for an AudioManager object
        instance = FindObjectOfType<AudioManager>();
        instance.SetVolumeAllChannels(1.0f);
        instance.PlayTrack(1, "Music", "AmbientMusic", true, true);
        instance.PlayTrack(2, "Music", "CombatMusic", true, true,0,1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            instance.CrossFadeChannels(1, 5.0f, 2, 5.0f);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            instance.StopChannel(1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            instance.PlayAttachedSound("SFX_AOE", TestObject, 1, 1, false, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            instance.PlaySound("SFX_AOE");
        }
    }
}
