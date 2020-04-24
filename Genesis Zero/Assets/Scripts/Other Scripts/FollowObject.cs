using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    // Start is called before the first frame update
   
    public float Yoffset;
    public float Xoffset;
    public float Zoffset;

    public GameObject followObj;
    private Vector2 pos;
    private GameObject playerObj;
    void Start()
    {

        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        updatePos();
    }
    public void updatePos()
    {
        if (playerObj == null)
            return;
        if (followObj == null)
            followObj = playerObj;
        pos = followObj.transform.position;
        this.transform.position = new Vector3(pos.x + Xoffset, pos.y + Yoffset, 0 + Zoffset);
    }
}
