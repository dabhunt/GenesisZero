using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Settings")]
    public GameObject projectile;
    public Transform firePoint;

    private float timeToFire = 0;
    private bool gunFired;
    private Player player;
    private Animator animator;
    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        if (Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / player.GetAttackSpeed().GetValue();
            Vector3 spawnpoint = new Vector3(firePoint.transform.position.x, firePoint.transform.position.y, 0);
            GameObject instance = (GameObject)Instantiate(projectile, spawnpoint, firePoint.transform.rotation);
            instance.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue(), player);
            animator.SetTrigger("gunFired");
        }
    }
}
