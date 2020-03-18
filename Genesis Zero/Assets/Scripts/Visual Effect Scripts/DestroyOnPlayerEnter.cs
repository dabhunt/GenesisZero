using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * Will destroy itself after detorying other listed gameobject
 * 
 */
[RequireComponent(typeof(Collider))]
public class DestroyOnPlayerEnter : MonoBehaviour
{
    public List<GameObject> Objects;

    public float delay;

    private bool triggered;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered && delay <= 0)
        {
            DestoryObjects();
        }
        else if(triggered)
        {
            delay -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Objects.Count > 0 && other.GetComponent<Player>() && triggered == false)
        {
            triggered = true;
        }
    }

    private void DestoryObjects()
    {
        foreach (GameObject g in Objects)
        {
            Destroy(g);
        }
        Destroy(this.gameObject);
    }
}
