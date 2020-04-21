using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
   
    public float Yoffset;
    public float Xoffset;
    public float Zoffset;

    private Vector2 playerpos;
    private GameObject playerObj;
    void Start()
    {

        playerObj = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        playerpos = playerObj.transform.position;
        this.transform.position = new Vector3(playerpos.x+Xoffset, playerpos.y+Yoffset, 0+ Zoffset);
    }
}
