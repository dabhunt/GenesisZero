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
        Debug.Log("Player");
        base.Update();
    }
}
