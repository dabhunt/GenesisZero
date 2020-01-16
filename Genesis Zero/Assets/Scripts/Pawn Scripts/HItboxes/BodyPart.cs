using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyPart : MonoBehaviour
{

    [Header("--Special Part?--")]
    public bool SpecialPart;
    [Tooltip("Applies only if SpecialPart is true")]
    public float damagemultipler;

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Hitbox>())
        {
            Debug.Log("Hit");
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Special");
    }

}
