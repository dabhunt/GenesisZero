using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMaterial: MonoBehaviour
{
    OverHeat overheat;
    void Start()
    {
        GameObject temp = GameObject.FindWithTag("Player");
        overheat = temp.GetComponent<Player>().GetComponent<OverHeat>();
        gameObject.GetComponent<Renderer>().material.SetFloat("_HeatControl", 0);
    }
    void Update()
    {
        float ratio = overheat.GetHeat() / overheat.GetMaxHeat();
        gameObject.GetComponent<Renderer>().material.SetFloat("_HeatControl", ratio);
    }
}