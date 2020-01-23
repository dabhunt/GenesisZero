using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 30f;
    public float gravity = 0f;
    public float lifeTime = 20f;
    public Vector3 direction;

    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("DestroyObject", lifeTime);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        direction = speed * transform.right * Time.deltaTime;
        transform.position += direction;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
