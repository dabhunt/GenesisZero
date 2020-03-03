using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCD : MonoBehaviour
{
    Player player;
    public Image ability1;
    public Image ability2;
    private bool cooling;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    void Update()
    {
        if (cooling)
        {
            AbilityCasting ac = player.GetComponent<AbilityCasting>();
            ability1.fillAmount = ac.GetAbilityCooldownRatio1();
            ability2.fillAmount = ac.GetAbilityCooldownRatio2();
            if (ability1.fillAmount <= 0 && ability2.fillAmount <= 0)
            {
                cooling = false;
            }
        }
       
    }
    public void Cast()
    {
        cooling = true;
    }
}
