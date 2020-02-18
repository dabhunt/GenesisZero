﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Justin Couch
 * Basic test class for projectiles.
 */
public class ProjectileTest : MonoBehaviour
{
    public float Speed = 1.0f;
    public float LifeDuration = 1.0f;
    private float lifeTime = 0.0f;
    public UnityEvent DestroyEvent;

    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Speed * Time.fixedDeltaTime, Space.Self);
        lifeTime += Time.fixedDeltaTime;
        if (lifeTime >= LifeDuration)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        DestroyEvent.Invoke();
    }
}
