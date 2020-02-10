using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    public GameObject emitter;
    public int amount = 1;
    public bool quitting;
    //bool applicationIsQuitting;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnDestroy()
    {
        if (quitting) return;

        for (int i = 0; i < amount; i++)
        {
            GameObject emit = (GameObject)Instantiate(emitter, transform.position, Quaternion.identity);
        }

    }

    void OnApplicationQuit()
    {
        quitting = true;
    }

}
