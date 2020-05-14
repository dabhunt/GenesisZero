using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractInterface : MonoBehaviour
{
    public static InteractInterface instance;
    private string type;
    private float minProximity = 7f;
    private bool canInteract = true;
    private GameObject player;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
       player = GameObject.FindGameObjectWithTag("Player");
    }
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (player.GetComponent<Player>().IsInteracting == true)
                return;
            //determine which interactable / pickup is closest, and perform interact
            string[] strArray = { "Interactable", "Pickups" };
            GameObject obj = ClosestTaggedObj(strArray);
            //if the closestObject is too far away, it returns null
            if (obj == null)
            {
                AudioManager.instance.PlaySound("SFX_Nope");
                return;
            }
            AudioManager.instance.PlayRandomSFXType("Interact", null, .3f);
            if (obj.GetComponent<SkillPickup>() != null)
            { obj.GetComponent<SkillPickup>().Interact(); return; }
            if (obj.GetComponent<Merchant>() != null)
                { obj.GetComponent<Merchant>().Interact(); return; }
            if (obj.GetComponent<GodHead>() != null)
                { obj.GetComponent<GodHead>().Interact(); return; }
            if (obj.GetComponent<ModConverter>() != null)
                { obj.GetComponent<ModConverter>().Interact(); return; }
            if (obj.GetComponent<SafeBox>() != null)
                { obj.GetComponent<SafeBox>().Interact(); return; }
            if (obj.GetComponent<BUGE>() != null)
                { obj.GetComponent<BUGE>().Interact(); return; }
        }
    }
    public GameObject ClosestTaggedObj(string[] tags)
    {
        if (!canInteract)
            return null;
        List<GameObject> objects = new List<GameObject>();
        //add all gameObjects that contain the tags specified in the passed in array into a big list
        for (int i = 0; i < tags.Length;i++)
        {
            objects.AddRange(GameObject.FindGameObjectsWithTag(tags[i]));
        }
        if (objects.Count < 1)
            return null;
        //evaluates the distance between the player and each object of this tag
        GameObject closest = objects[0];
        float shortest = Vector2.Distance(player.transform.position, objects[0].transform.position);
        for (int i = 0; i < objects.Count; i++)
        {
            float dist = Vector2.Distance(player.transform.position, objects[i].transform.position);
            if (dist < shortest)
            {
                //if the Inactiveflag component is on the script, it ignores it
                //this solves the problem that there are many components with different active states being checked here
                if (objects[i].GetComponent<InactiveFlag>() == null)
                {
                    shortest = dist;
                    closest = objects[i];
                }
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
        GameObject closest = ClosestInteractable();
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
        GameObject closest = ClosestInteractable();
        closest.GetComponent<GodHead>().FinalConfirmSelection();
    }
    public void Decline()
    {
        GameObject closest = ClosestInteractable();
        closest.GetComponent<GodHead>().CloseUI();
    }
    //Merchant functions Purchase, or exitshop
    public void Purchase()
    {
        GameObject closest = ClosestInteractable();
        closest.GetComponent<Merchant>().FinalConfirmSelection();
    }
    public void ExitShop()
    {
        GameObject closest = ClosestInteractable();
        closest.GetComponent<Merchant>().CloseUI();
    }


}
