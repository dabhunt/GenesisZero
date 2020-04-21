using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInDimension : MonoBehaviour
{
    public bool x = false;
    public bool y = false;
    public bool z = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (x)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
        if (y)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        if (z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }
}
