using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Settings")]
    public GameObject projectile;
    public Transform firePoint;

    private PlayerInputActions inputActions;
    private float timeToFire = 0;
    private Player player;
    private float fireAction;
    private bool fired = false;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Fire.performed += ctx => fireAction = ctx.ReadValue<float>();
    }

    private void Start()
    {
        player = GetComponent<Player>();
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

    public bool Shoot()
    {
        if (fireAction > 0 && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / player.GetAttackSpeed().GetValue();
            Vector3 spawnpoint = new Vector3(firePoint.transform.position.x, firePoint.transform.position.y, 0);
            GameObject instance = (GameObject)Instantiate(projectile, spawnpoint, firePoint.transform.rotation);
            instance.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue(), player);
            fired = true;
        }
        else
        {
            fired = false;
        }
        return fired;
    }
}
