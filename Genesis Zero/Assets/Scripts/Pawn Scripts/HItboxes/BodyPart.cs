using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Collider))]
//[RequireComponent(typeof(Rigidbody))]

[ExecuteInEditMode]
public class BodyPart : MonoBehaviour
{

    [Header("--Special Part?--")]
    public bool SpecialPart;
    [Tooltip("Applies only if SpecialPart is true")]
    public float damagemultipler = 1;

    private void Awake()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
            SphereCollider sc = GetComponent<SphereCollider>();
            sc.radius = .2f;
            sc.isTrigger = true;
        }
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        }
    }
    private void Start()
    {
        Collider collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Hitbox>())
        {
            //Debug.Log("Hit");
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Special");
    }


}
