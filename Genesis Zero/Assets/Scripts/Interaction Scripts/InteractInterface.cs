using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractInterface : MonoBehaviour
{
    private string type;
    private GameObject closest;
    private float minProximity = 7f;
    private bool canInteract = true;
    //returns the closest object in the game with the tag "Interactable"
    public GameObject ClosestInteractable()
    {
        return ClosestTaggedObj("Interactable");
    }
    public GameObject ClosestTaggedObj(string tag)
    {
        //convert single string into array
        string[] strArray = {tag};
        return ClosestTaggedObj(strArray);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            string[] strArray = { "Interactable", "Pickups" };
            GameObject obj = ClosestTaggedObj(strArray);
            if (obj.GetComponent<Merchant>() != null)
                { obj.GetComponent<Merchant>().Interact(); return; }
            if (obj.GetComponent<GodHead>() != null)
                { obj.GetComponent<GodHead>().Interact(); return; }
            if (obj.GetComponent<SkillPickup>() != null)
                { obj.GetComponent<SkillPickup>().Interact(); return; }

        }
    }
    public GameObject ClosestTaggedObj(string[] tags)
    {
        if (!canInteract)
            return null;
        List<GameObject> objects = new List<GameObject>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //add all gameObjects that contain the tags specified in the passed in array into a big list
        for (int i = 0; i < tags.Length;i++)
        {
            objects.AddRange(GameObject.FindGameObjectsWithTag(tags[i]));
        }
        //evaluates the distance between the player and each object of this tag
        float shortest = Vector3.Distance(player.transform.position, objects[0].transform.position);
        for (int i = 0; i < objects.Count; i++)
        {
            float dist = Vector3.Distance(player.transform.position, objects[i].transform.position);
            if (dist < shortest)
            {
                shortest = dist;
                closest = objects[i];
            }
        }
        if (shortest >= minProximity)
            return null;
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
