using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeakPoint : MonoBehaviour
{
    private void Start()
    {
        Collider collider = GetComponent<Collider>();
    }

}
