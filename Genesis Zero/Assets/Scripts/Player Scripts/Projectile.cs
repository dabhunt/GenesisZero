using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 30f;
    public float gravity = 0f;
    public Vector3 direction;

    private Rigidbody rb;
    private float lifeTime = 1f;
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
            bool hitdetect = Physics.SphereCast(transform.position, col.radius, transform.forward, out hit, (speed * Time.fixedDeltaTime));
            if (hitdetect && hit.collider != col)
            {
                Vector3 dir = speed * transform.forward * Time.fixedDeltaTime;
                dir = dir.normalized * (hit.distance + col.radius * 2);
                //Debug.Log(transform.position +" : "+ hit.point);
                transform.position = hit.point;
                if (GetComponent<Hitbox>())
                {
                    if (GetComponent<Hitbox>().CheckCollisions(hit.collider))
                    {
                        coll = false;
                    }
                    else
                    {
                        coll = true;
                    }
                }
                return true;
            }
        }
        return false;
    }
}
