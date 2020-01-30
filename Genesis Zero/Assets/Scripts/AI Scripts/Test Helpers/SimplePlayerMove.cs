using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FakeRigidbody))]
/**
 * Justin Couch
 * Basic test class for making an object move and jump.
 */
public class SimplePlayerMove : MonoBehaviour
{
    private FakeRigidbody frb;
    public float MoveSpeed = 1.0f;
    public float MoveAcceleration = 1.0f;
    public float JumpSpeed = 10f;

    private void Awake()
    {
        frb = GetComponent<FakeRigidbody>();
    }

    private void FixedUpdate()
    {
        float moveInput = (Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0.0f);

        frb.Accelerate(Vector3.right * (moveInput * MoveSpeed - frb.GetVelocity().x) * MoveAcceleration);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            frb.AddVelocity(Vector3.up * JumpSpeed);
        }
    }
}
