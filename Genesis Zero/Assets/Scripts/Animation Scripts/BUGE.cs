using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BUGE : MonoBehaviour
{
	// private GameObject bugObj;

	
 //    void FixedUpdate()
 //    {
 //        bugObj.LookAt(playerController.crosshair.transform);
 //        if (transform.position.x < playerController.crosshair.transform.position.x)
 //            isAimingRight = true;
 //        else
 //            isAimingRight = false;
 //    }
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
     private float speedvar;
     private float distance;
     private bool prevFacingRight;
     private float curSpeed;

    void Start()
    {
		GameObject temp = GameObject.FindWithTag("Player");
		playerController = temp.GetComponent<PlayerController>();
		Player = temp.GetComponent<Transform>();
		speedvar = Speed;
		prevFacingRight = playerController.IsFacingRight();

	}
     void FixedUpdate()
     {
        if ((this.gameObject != null) && (Player != null)){
         	bool currentlyFacingRight = playerController.IsFacingRight();
         	// get current player speed
         	// if the player was going left and switches to right, or right switching to left this will change x offset
         	if (currentlyFacingRight != prevFacingRight){
         		followXoffset *= -1;
    		}
         	//get the players speed, but this is saved for the next fixed update check
         	prevFacingRight = playerController.IsFacingRight();
            curSpeed = playerController.GetCurrentSpeed();
            transform.LookAt(playerController.worldXhair.transform);
            //enable this line and delete the above line once Toan changes to UI cursor
            //transform.LookAt(playerController.worldXhair.transform);
             distance = Vector3.Distance(transform.position, Player.position);

             Vector3 follow = new Vector3(Player.position.x - followXoffset, Player.position.y+MinDistance, Player.position.z);
             if (distance >= MinDistance && distance < MaxDistance)
             {
             	//print("right in the sweet spot ;)");
             	if (speedvar > Speed){
             		speedvar = speedvar * deAccelerationMultiplier;
             	}
             	this.transform.position = Vector3.MoveTowards(this.transform.position, follow, speedvar * Time.deltaTime);
             }
             else if(distance >= MinDistance)
             {
                 
                 //print("player.position " + Player.position);
                 //setting always the same Y position
                 //remenber to use the new 'follow' position, not the Player.transform.position or else it'll move directly to the player
                 speedvar += acceleration;
                 //speedvar += distMulti;
                 this.transform.position = Vector3.MoveTowards(this.transform.position, follow, speedvar * Time.deltaTime);
             }
            //exaggerate the sin wave while the player runs vs the sin while floating idle next to player
         	var sin = sinAmplitude;
         	if (curSpeed > 0.5f)
         	  {sin = AmplitudeWhileRunning;}
            Vector3 sinPos = this.transform.position;
            sinPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * sinFrequency) * sin;
            //sinPos.x += Mathf.Sin (Time.fixedTime * Mathf.PI * sinFrequency) * sinAmplitude/4;
            this.transform.position = sinPos;
         }

     }

}
