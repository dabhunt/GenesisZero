using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject projectile;
    public Transform firePoint;
    public float fireRate = 5;

    private PlayerInputActions inputActions;
    private float timeToFire = 0;
    private float fireAction;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Fire.performed += ctx => fireAction = ctx.ReadValue<float>();
    }
    private void Update()
    {
        Shoot();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void Shoot()
    {
        if (fireAction > 0 && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate; 
            Instantiate(projectile, firePoint.transform.position, firePoint.transform.rotation);
        }
    }
}
