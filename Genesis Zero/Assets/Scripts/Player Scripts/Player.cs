﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    // Start is called before the first frame update
    new void Start()
    {
        Time.fixedDeltaTime = .01f; // <- this is here as a placeholder, no other good place to have this.

        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        //Debug.Log("Player" + GetHealth().GetValue() +" : "+ GetHealth().GetMaxValue());
        base.Update();
    }

    new public void TakeDamage(float amount)
    {
        //Add anything if there is class specific additions to taking damage
        base.TakeDamage(amount);
    }
}
