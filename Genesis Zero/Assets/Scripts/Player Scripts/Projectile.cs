using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 30f;
    public float gravity = 0f;
    public float lifeTime = 1f;

    public Vector3 direction;
    private Rigidbody rb;

    private bool coll;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("DestroyObject", lifeTime);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {

    }

    private void Move()
    {       
        bool collided = CheckCollisions();
        if (coll == false)
        {
            direction = speed * transform.forward * Time.fixedDeltaTime;
            transform.position += direction;
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public void SetDirection(Vector3 vec)
    {
        direction = vec;
    }

    public bool CheckCollisions()
    {
        RaycastHit hit;
        if (GetComponent<SphereCollider>())
        {
            SphereCollider col = GetComponent<SphereCollider>();
            bool hitdetect = Physics.SphereCast(transform.position, col.radius, transform.forward, out hit, (speed * 1f * Time.fixedDeltaTime));
            if (hitdetect && hit.collider != col)
            {
                Vector3 dir = speed * transform.forward * Time.fixedDeltaTime;
                dir = dir.normalized * (hit.distance + col.radius * 2);
                transform.position = hit.point;
                coll = true;
                if (GetComponent<Hitbox>())
                {
                    if (GetComponent<Hitbox>().CheckCollisions(hit.collider))
                    {
                        coll = true;
                    }
                    else
                    {
                        coll = false;
                    }
                }
                return true;
            }
        }
        return false;
    }
}
