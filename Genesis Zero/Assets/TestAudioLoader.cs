using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudioLoader : MonoBehaviour
{
    public int track;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioLoader>().PlaySound(track);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
