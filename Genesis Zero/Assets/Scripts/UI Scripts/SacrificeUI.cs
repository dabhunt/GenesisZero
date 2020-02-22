using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeUI : MonoBehaviour
{
    private Player player;

    void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Player");
        player = temp.GetComponent<Player>();
    }
    //

    //if player presses on a hardware mod in the bar above, it highlights
    void changeSelection(){

    }
    //remove skill function
    public void ScrapMod()
    {
    	//call skill manager, and scrap the mod
    }
}
