﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    private Pawn pawn;
    private Hurtbox hb;
    public GameObject healthBar;
    //public Image healthBar;
    private bool delayFinished = false;
    private float heightOffset = 0.0f;

    void Start()
    {
        Invoke("Delayed", .75f);
        heightOffset = healthBar.transform.localPosition.magnitude;
    }

    public void Delayed()
    {
        pawn = GetComponent<Pawn>();
        hb = GetComponent<Hurtbox>();
        healthBar.SetActive(false);
        delayFinished = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (delayFinished)
        {
            float hp = pawn.GetHealth().GetValue();
            if (hp < pawn.GetHealth().GetMaxValue())
            {
                healthBar.SetActive(true);
                healthBar.transform.localScale = new Vector3((hp / pawn.GetHealth().GetMaxValue()) * 1f, .088f, .71f);
                //healthBar.transform.Rotate(new Vector3(0, 0, 0), Space.World);
            }
            healthBar.transform.position = transform.position + Vector3.up * heightOffset;
            healthBar.transform.rotation = Quaternion.identity;
            print("Health Maxvalue: " + pawn.GetHealth().GetMaxValue());
        }

    }
}
