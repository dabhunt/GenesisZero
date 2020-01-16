using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    // Start is called before the first frame update
    new void Start()
    {

        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetHealth().AddBonus(-10,15,1);
        }
        Debug.Log("Player" + GetHealth().GetValue() +" : "+ GetHealth().GetMaxValue());
        base.Update();
    }

    new public void TakeDamage(float amount, GameObject source)
    {
        //Add anthing if there is class specific additions to taking damage
        base.TakeDamage(amount, source);
    }
}
