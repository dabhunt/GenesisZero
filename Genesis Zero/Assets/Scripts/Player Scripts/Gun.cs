using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Settings")]
    public GameObject projectile;
    public Transform firePoint;
    //[Range(0.0f, 10.0f)]
    private float spreadAngle;
    private OverHeat overheat;

    private Player player;
    private void Start() 
    {
        player = GetComponent<Player>();
        overheat = GetComponent<OverHeat>();
    }
    public void Shoot()
    {   
        spreadAngle = overheat.CalculateBloom();
        print("spreadAngle: "+spreadAngle);
        Vector3 spawnpoint = new Vector3(firePoint.transform.position.x, firePoint.transform.position.y, 0);
        GameObject instance = (GameObject) Instantiate(projectile, spawnpoint, firePoint.transform.rotation);
        instance.transform.Rotate(Vector3.forward,Random.Range(-spreadAngle, spreadAngle),Space.World);
        instance.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue(), player);
    }
}