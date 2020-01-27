using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * Basic movement script for testing.
 */
public class BasicMove : MonoBehaviour
{
    public float MoveSpeed = 1.0f;

    protected void Update()
    {
        Vector3 moveDir = Vector3.zero;
        moveDir += Input.GetKey(KeyCode.UpArrow) ? Vector3.up : Vector3.zero;
        moveDir += Input.GetKey(KeyCode.DownArrow) ? Vector3.down : Vector3.zero;
        moveDir += Input.GetKey(KeyCode.RightArrow) ? Vector3.right : Vector3.zero;
        moveDir += Input.GetKey(KeyCode.LeftArrow) ? Vector3.left : Vector3.zero;
        transform.Translate(moveDir.normalized * MoveSpeed * Time.deltaTime);
    }
}
