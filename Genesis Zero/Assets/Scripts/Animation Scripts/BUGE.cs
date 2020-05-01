using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
public class BUGE : MonoBehaviour
{
    private PlayerController playerController;
    public float MinDistance = 35;
    public float MaxDistance = 10;
    public float Speed;
    public float acceleration;
    public float deAccelerationMultiplier = .6f;

    [Header("Sin floating effect")]
    public float sinAmplitude = 0.1f;
    public float AmplitudeWhileRunning = .3f;
    public float sinFrequency = 1.2f;
    public float followXoffset = .6f;

    private Transform Player;
    public float speedvar;
    private float distance;
    private bool prevFacingRight;
    private float curPlayerSpeed;
    private Vector3 follow;
    private GameObject LookAtObj;
    private float minDistReset;
    private float maxDistReset;
    private Queue<WayPoint> animWaypoints;
    private List<DialogueInfo> dialogueInfo;
    private bool AnimEnabled = false;
    private bool lookOverride = false;
    private EventSystem eventSystem;
    private GameObject alertObj;
    private bool mouseOver = false;
    private GameObject playerObj;
    public bool followingPlayer = false;
    private GameObject flashlight;
    void Start()
    {
        animWaypoints = new Queue<WayPoint>();
        dialogueInfo = new List<DialogueInfo>();
        playerObj = GameObject.FindWithTag("Player");
		playerController = playerObj.GetComponent<PlayerController>();
		Player = playerObj.GetComponent<Transform>();
		speedvar = Speed;
		prevFacingRight = playerController.IsFacingRight();
        minDistReset = MinDistance;
        maxDistReset = MaxDistance;
        alertObj = GameObject.FindGameObjectWithTag("BUG-EAlert");
        flashlight = transform.Find("flashlightouter").gameObject;
        flashlight.SetActive(false);
        alertObj.SetActive(false);

        //Test waypoint system
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) //rightclick to test waypoint system
        {
            //if the mouse is NOT in the top 10% of the screen
            if (lookOverride)
                return;
            if (Input.mousePosition.y < Screen.height - (Screen.height * .1f))
            {
                ClearWayPoints();
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    AddWayPoint(playerController.screenXhair.transform.position+new Vector3(0,1f,0), -1);
                    //play UI_Essence txt, don't pause, and dequeue on completion
                    DialogueManager.instance.TriggerDialogue("UI_Essence", false, true);
                    LookAtObj = new GameObject();
                    LookAtObj.transform.position = animWaypoints.Peek().Destination;
                    LookAt(LookAtObj, 5);
                    print("blocked by UI: screnXhair.Z " + playerController.screenXhair.transform.position.z);
                }
                else
                {//if the player mouse is not on BUGE, add a waypoint
                    if (mouseOver == false)
                        AddWayPoint(playerController.worldXhair.transform.position, 1f);
                }
            }
            
        }
    }
    public void FollowingPlayer(bool boo)
    {
        followingPlayer = boo;
        flashlight.SetActive(boo);
    }
    private void OnMouseOver()
    {
        mouseOver = true;
        if (Input.GetMouseButtonDown(1))
        {
            Interact();

            GetComponent<InteractPopup>().interactable = false;
            GetComponent<InteractPopup>().DestroyPopUp();
        }
    }
    private void OnMouseEnter()
    {
        LookAt(playerObj, 3f);
        
        if (alertObj.activeSelf)
        {
            VFXManager.instance.ChangeColor(alertObj, Color.red);
            GetComponent<InteractPopup>().interactable = true;
        }
           
    }
    private void OnMouseExit()
    {
        if(alertObj.activeSelf)
        {
            VFXManager.instance.ChangeColor(alertObj, Color.white);
            GetComponent<InteractPopup>().interactable = false;
            mouseOver = false;
        }
    }
    void FixedUpdate()
     {
        if (!followingPlayer)
            return;
        if (animWaypoints.Count > 0)
            AnimEnabled = true;
        else { AnimEnabled = false; }
        if ((this.gameObject != null) && (Player != null)){
            var sin = sinAmplitude;
            if (!AnimEnabled) //during normal gameplay this runs
            {
                MinDistance = minDistReset;
                MaxDistance = maxDistReset;
                bool currentlyFacingRight = playerController.IsFacingRight();
                // get current player speed
                // if the player was going left and switches to right, or right switching to left this will change x offset
                if (currentlyFacingRight != prevFacingRight)
                {
                    followXoffset *= -1;
                }
                //get the players speed, but this is saved for the next fixed update check
                prevFacingRight = playerController.IsFacingRight();
                curPlayerSpeed = playerController.GetCurrentSpeed();
                transform.LookAt(playerController.worldXhair.transform);
                if (lookOverride)
                    transform.LookAt(LookAtObj.transform);
                //enable this line and delete the above line once Toan changes to UI cursor
                distance = Vector3.Distance(transform.position, Player.position);
                follow = new Vector3(Player.position.x - followXoffset, Player.position.y + MinDistance, Player.position.z);
                //exaggerate the sin wave while the player runs vs the sin while floating idle next to player
                if (curPlayerSpeed > 0.5f)
                { sin = AmplitudeWhileRunning; }
            }
            else //when animEnabled,(During cutscenes) this runs
            {
                MinDistance = 1f;
                MaxDistance = 1.7f;
                follow = new Vector3(animWaypoints.Peek().Destination.x, animWaypoints.Peek().Destination.y, animWaypoints.Peek().Destination.z);
                
                distance = Vector3.Distance(transform.position, animWaypoints.Peek().Destination);
                //if BUG-E is At the waypoint location, then he looks back toward the player
                if (animWaypoints.Peek().AtLocation)
                    transform.LookAt(Player);
                else
                {
                    transform.LookAt(follow);
                }
                    
            }

            distance = Vector3.Distance(transform.position, follow);
            if (distance >= MinDistance && distance < MaxDistance)
            {
                if (speedvar > Speed)
                {
                    speedvar = speedvar * deAccelerationMultiplier;
                }
                
                if (animWaypoints.Count > 0 && !animWaypoints.Peek().AtLocation) //dequeue the current waypoint, after the delay, if the waypoint is 
                {
                    animWaypoints.Peek().AtLocation= true;
                    //now that BUG-E is at desired location, he stays there based on durationat waypoint
                    StartCoroutine(DequeueAfterSeconds(animWaypoints.Peek().DurationAtWayPoint));
                    speedvar = Speed;
                } 
            }
            if (distance >= MinDistance)
            {
                speedvar += acceleration;
                //this.transform.position = Vector3.MoveTowards(this.transform.position, follow, speedvar * Time.deltaTime);
            }
            else
            {
                if (speedvar > 3)
                    speedvar -= acceleration;
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, follow, speedvar * Time.deltaTime);
            Vector3 sinPos = this.transform.position;
            sinPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * sinFrequency) * sin;
            this.transform.position = sinPos;
        }
     }
    public void SetWayPointQueue(Queue<WayPoint> newQueue)
    {
        for (int i = 0; i < newQueue.Count; i++)
        {
            WayPoint newpoint = newQueue.Dequeue();
            animWaypoints.Enqueue(newpoint);
        }
    }
    //add a waypoint location that Bug-E will fly over to when this reaches the top of the queue
    public void AddWayPoint(Vector3 V, float T)
    {
        WayPoint newpoint = ScriptableObject.CreateInstance<WayPoint>();
        if (T == -1)
            T = 999;
        newpoint.SetValues(V, T);
        animWaypoints.Enqueue(newpoint);
    }
    public void ClearWayPoints()
    {
        animWaypoints.Clear();
    }
    //move to the next waypoint in the queue, if any and return the Waypoint type
    public WayPoint DequeueWayPoint()
    {
        if (animWaypoints.Count > 0)
            return animWaypoints.Dequeue();
        else 
            return null;
    }
    public void LookAt( GameObject lookat, float seconds)
    {
        LookAtObj = lookat;
        lookOverride = true;
        Invoke("StopLook", seconds);
    }
    /* called when player interacts with BUG-E */
    public void Interact() 
    {
        DialogueInfo info;
        if (dialogueInfo.Count < 1)
            return;
        info = dialogueInfo[0];
        dialogueInfo.RemoveAt(0);
        DialogueManager.instance.TriggerDialogue(info.dialogueName);
        if (dialogueInfo.Count < 1)
        {
            //turn off exclaimation mark
            alertObj.SetActive(false);
        }
        if (info.flysOver)
        {
            animWaypoints.Enqueue(info.waypoint);
        }
    }
    /* Remove dialogue from queue if the player leaves the range
     */
    public void TooFar(DialogueInfo info)
    {
        ClearWayPoints();
        if (dialogueInfo.Count > 0)
        {
            for (int i = 0; i < dialogueInfo.Count; i++)
            {
                if (info.ID == dialogueInfo[i].ID)
                {
                    dialogueInfo.RemoveAt(i);
                }  
            }
        }
        else {
            alertObj.SetActive(false);
        }
    }
    public void AddOptionalDialoguePrompt(DialogueInfo info)
    {
        //if the queue already has that file, don't add it
        if (dialogueInfo.Count > 0)
        {
            for (int i = 0; i < dialogueInfo.Count; i++)
            {
                if (dialogueInfo[i].dialogueName == info.dialogueName)
                    return;
            }
        }
        alertObj.SetActive(true);
        alertObj.GetComponent<FollowObject>().updatePos();
        dialogueInfo.Add(info);
    }
    private void StopLook()
    {
        transform.Find("Arrow").gameObject.SetActive(false);
        lookOverride = false;
    }
    IEnumerator DequeueAfterSeconds(float seconds)
    {
        yield return StartCoroutine(WaitForRealSeconds(seconds));
        if (animWaypoints.Count > 0)
        {
            animWaypoints.Dequeue();
        }
    }
    //Real seconds are used so that timescale doesn't effect it in paused parts
    public static IEnumerator WaitForRealSeconds(float delay)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + delay)
        {
            yield return null;
        }
    }
}
