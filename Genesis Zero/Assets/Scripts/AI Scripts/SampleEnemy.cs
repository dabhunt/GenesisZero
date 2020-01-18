using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEnemy: Pawn
{

    //AI Stuff

    // Start is called before the first frame update
    new void Start()
    {
        //Start Method
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        //Update methods (and AI Stuff)
        base.Update();
    }

    new public void TakeDamage(float amount)
    {
        //Add anything if there is class specific additions to taking damage
        base.TakeDamage(amount);
    }
}