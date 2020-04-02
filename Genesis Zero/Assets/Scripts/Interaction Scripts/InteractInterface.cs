using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractInterface : MonoBehaviour
{
    private string type;
    private GameObject closest;
    private float minProximity = 7f;
    private bool canInteract = true;
    //returns the closest object in the game with the tag "Interactable"
    public GameObject ClosestInteractable()
    {
        if (!canInteract)
            return null;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("Interactable");
        GameObject closest = objArray[0];
        float shortest = Vector3.Distance(player.transform.position, objArray[0].transform.position);
        for (int i = 0; i < objArray.Length; i++)
        {
            float dist = Vector3.Distance(player.transform.position, objArray[i].transform.position);
            if (dist < shortest)
            {
                shortest = dist;
                closest = objArray[i];
            }
        }
        if (shortest >= minProximity)
            return null;
        canInteract = false;
        Invoke("Reset", .2f);
        return closest;
    }
    // button sends the int value of the selected UI obj as a string, this gets sent to godhead script as updateselect + the mod slot
    // this is used for sacrifice interface and merchant interface
    public void Select(string sint)
    {
        int num = int.Parse(sint);
        closest = ClosestInteractable();
        if (closest.GetComponent<GodHead>() != null)
        {
            closest.GetComponent<GodHead>().UpdateSelect(num);
        }
        else if (closest.GetComponent<Merchant>() != null)
        {
            closest.GetComponent<Merchant>().UpdateSelect(num);
        }
    }
    public void Reset()
    {
        canInteract = true;
    }
    //God head functions Accept offer, decline offer
    public void Accept()
    {
        closest = ClosestInteractable();
        closest.GetComponent<GodHead>().FinalConfirmSelection();
    }
    public void Decline()
    {
        closest = ClosestInteractable();
        closest.GetComponent<GodHead>().CloseUI();
    }
    //Merchant functions Purchase, or exitshop
    public void Purchase()
    {
        closest = ClosestInteractable();
        closest.GetComponent<Merchant>().FinalConfirmSelection();
    }
    public void ExitShop()
    {
        closest = ClosestInteractable();
        closest.GetComponent<Merchant>().CloseUI();
    }


}
