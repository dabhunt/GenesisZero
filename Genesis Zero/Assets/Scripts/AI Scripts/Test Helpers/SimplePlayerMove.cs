using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FakeRigidbody))]
/**
 * Justin Couch
 * Basic test class for making an object move and jump.
 */
public class SimplePlayerMove : Pawn
{
    private FakeRigidbody frb;
    public float MoveSpeed = 1.0f;
    public float MoveAcceleration = 1.0f;
    public float JumpSpeed = 10f;

    private void Awake()
    {
        frb = GetComponent<FakeRigidbody>();
    }

    new private void FixedUpdate()
    {
        float moveInput = (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? 1.0f : 0.0f);

        frb.Accelerate(Vector3.right * (moveInput * MoveSpeed - frb.GetVelocity().x) * MoveAcceleration);
    }

    new private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            frb.AddVelocity(Vector3.up * JumpSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AIController[] allEnemies = FindObjectsOfType<AIController>();
            AIController closestEnemy = null;
            float closeDist = Mathf.Infinity;
            foreach (AIController curEnemy in allEnemies)
            {
                if ((transform.position - curEnemy.transform.position).sqrMagnitude < closeDist && curEnemy.gameObject.activeInHierarchy)
                {
                    closeDist = (transform.position - curEnemy.transform.position).sqrMagnitude;
                    closestEnemy = curEnemy;
                }
            }

            if (closestEnemy != null)
            {
                closestEnemy.TakeDamage(1.0f, this);
            }
        }
    }
}
