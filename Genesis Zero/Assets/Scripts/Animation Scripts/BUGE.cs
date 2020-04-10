using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    private Vector2 follow;
    private GameObject LookAtObj;
    private float minDistReset;
    private float maxDistReset;
    private Queue<WayPoint> animWaypoints;
    private bool AnimEnabled = false;
    private bool lookOverride = false;
    void Start()
    {
        animWaypoints = new Queue<WayPoint>();
        GameObject temp = GameObject.FindWithTag("Player");
		playerController = temp.GetComponent<PlayerController>();
		Player = temp.GetComponent<Transform>();
		speedvar = Speed;
		prevFacingRight = playerController.IsFacingRight();
        minDistReset = MinDistance;
        maxDistReset = MaxDistance;
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
                if (MouseInputUIBlocker.BlockedByUI)
                {
                    AddWayPoint(playerController.screenXhair.transform.position, 1f);
                    print("blocked by UI");
                }
                else
                {
                    AddWayPoint(playerController.worldXhair.transform.position, 1f);
                }
            }
            
        }
    }
    void FixedUpdate()
     {
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
                follow = new Vector3(animWaypoints.Peek().Destination.x, animWaypoints.Peek().Destination.y, 0);
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
                    StartCoroutine(DequeueAfterSeconds(animWaypoints.Peek().DurationAtWayPoint));
                    speedvar = Speed;
                } 
            }
            else if (distance >= MinDistance)
            {
                speedvar += acceleration;
                //this.transform.position = Vector3.MoveTowards(this.transform.position, follow, speedvar * Time.deltaTime);
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
    public void AddWayPoint(Vector3 V, float T)
    {
        WayPoint newpoint = ScriptableObject.CreateInstance<WayPoint>();
        newpoint.SetValues(V, T);
        animWaypoints.Enqueue(newpoint);
    }
    public void ClearWayPoints()
    {
        animWaypoints.Clear();
    }
    public void LookAt( GameObject lookat, float seconds)
    {
        print("lookat running");
        LookAtObj = lookat;
        lookOverride = true;
        Invoke("StopLook", seconds);
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

    //Real seconds are used so that timescale doesn't effect it
    public static IEnumerator WaitForRealSeconds(float delay)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + delay)
        {
            yield return null;
        }
    }
}
