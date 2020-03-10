using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SacrificeInterface : MonoBehaviour
{
    private GodHead god;
    private bool selectionMade = false;
    public void Start()
    {

        god = closestGod().GetComponent<GodHead>();
            
    }

    public void UpdateMod()
    {

    }

    public GameObject closestGod()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] godHead = GameObject.FindGameObjectsWithTag("GodHead");
        GameObject closestGod = godHead[0];
        float longest = 0;
        for (int i = 0; i < godHead.Length; i++)
        {
            float dist = Vector3.Distance(player.transform.position, godHead[i].transform.position);
            if (dist > longest)
            {
                longest = dist;
                closestGod = godHead[i];
            }
        }
        return closestGod;
    }
    // button sends the int value of it's mod slot as a string, this gets sent to godhead script as updateselect + the mod slot
    public void SelectMod(string sint)
    {
        this.gameObject.GetComponent<Button>().interactable = true;
        int modnum = int.Parse(sint);
        god = closestGod().GetComponent<GodHead>();
        god.UpdateSelect(modnum);
    }
    public void Accept()
    {
        god.FinalConfirmSelection();
    }

    public void Decline()
    {
        god.CloseUI();
    }

}
