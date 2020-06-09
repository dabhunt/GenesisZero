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
        if (!canInteract)
            return null;
        List<GameObject> objects = new List<GameObject>();
        objects.AddRange(GameObject.FindGameObjectsWithTag(tag));
        if (objects.Count < 1)
            return null;
        //evaluates the distance between the player and each object of this tag
        return ClosestObjToPlayer(objects.ToArray());
    }
    public GameObject ClosestObjToPlayer(GameObject[] objects)
    {
        if (objects.Length < 1)
        {
            return null;
        }

        GameObject closest = objects[0];
        if (closest == null)
        {
            return null;
        }
        float shortest = Vector2.Distance(Player.instance.CenterPoint(), objects[0].transform.position);
        for (int i = 0; i < objects.Length; i++)
        {
            float dist = Vector2.Distance(Player.instance.CenterPoint(), objects[i].transform.position);
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (player.GetComponent<Player>().IsInteracting() == true)
                return;
            print("isInteracting should be true -> " + player.GetComponent<Player>().IsInteracting());
            //determine which interactable / pickup is closest, and perform interact
            string[] strArray = { "Pickups" , "Interactable", };
            GameObject obj = ClosestTaggedObj(strArray);
            //if the closestObject is too far away, it returns null
            print("closest Obj is... " + obj);
;           if (obj == null)
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
                {obj.GetComponent<ModConverter>().Interact(); return; }
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
        //ClosestWithTag[] closestWithTag = new ClosestWithTag[tags.Length];
        List<GameObject> closestObjectList = new List<GameObject>();
        GameObject closestPickup = null;
        for (int i = 0; i < tags.Length; i++)
        {
            if (ClosestTaggedObj(tags[i]) != null) //if the returned value is not null, add it to the list
            {
                closestObjectList.Add(ClosestTaggedObj(tags[i]));
                if (tags[i] == "Pickups")
                {
                    closestPickup = closestObjectList[i];
                }
            }
        }
        if (closestObjectList.Count < 1)
            return null;
        GameObject closestPriorityObj = ClosestObjToPlayer(closestObjectList.ToArray());
        if (closestPickup != null)
        {
            float pickupDist = Mathf.Abs(closestPickup.transform.position.x - Player.instance.transform.position.x); //set distance from player
            float otherObjDist = Mathf.Abs(closestPriorityObj.transform.position.x - Player.instance.transform.position.x); //set distan
            if ((pickupDist - 3 < otherObjDist))//
                closestPriorityObj = closestPickup;
        }
        return closestPriorityObj;
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
