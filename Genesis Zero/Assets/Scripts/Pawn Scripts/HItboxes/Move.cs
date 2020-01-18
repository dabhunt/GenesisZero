using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public GameObject target;
    public Vector3 direction;
    public bool active;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            active = true;
        }

        if (active)
        {
            transform.position = transform.position + direction * Time.deltaTime;
        }
    }
}
