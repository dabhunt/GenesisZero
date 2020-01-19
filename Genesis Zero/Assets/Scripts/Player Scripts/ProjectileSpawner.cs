using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject projectile;
    public Transform firePoint;

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
        Debug.Log(fireAction);
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
        if (fireAction == 1 && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / projectile.GetComponent<Projectile>().fireRate; 
            Instantiate(projectile, firePoint.position, firePoint.rotation);
        }
    }
}
