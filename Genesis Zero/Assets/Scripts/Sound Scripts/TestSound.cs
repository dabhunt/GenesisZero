using System.Collections;
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
        instance.PlayTrack(2, "Music", "CombatMusic", 0, 1, true, true);
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
            instance.ClearAllStatic();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            instance.AddStatic("SFX_AOE", TestObject);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            instance.PlayStatic("SFX_AOE");
        }
    }
}
