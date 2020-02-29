﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateDistant : MonoBehaviour
{
    //at what minimum distance will enemies start to deactivate
    //enemies closer than this will be reactivated
    public float deactivateDist = 32f;
    public float updatesPerSecond=1f;
    //which game tags to check and deactivate if out of range
    public string[] tags = new string[] {"Enemy"};
    private GameObject[] objects;
    private Player player;
    private bool firstCheck = true;
    void Start()
    {
       	GameObject temp = GameObject.FindWithTag("Player");
       	player = temp.GetComponent<Player>();
    	InvokeRepeating("Check", 1/updatesPerSecond, 1/updatesPerSecond);
    }
    // Check is currently called once a second, but can also be called manually when the player is teleported to prevent visual latency
  	void Check()
  	{
    	//check all the tags provided
    	for (int t = 0; t < tags.Length; t ++){
    		//find the array of gameobjects associated with that tag
    		// if its the first check of the game, use a find by tag
    		//all other checks will use the already existing array of enemies to do this check
    		if (firstCheck){
    			firstCheck = false;
    			//NOTE Finding by tag does not work if they are deactive
    			objects = GameObject.FindGameObjectsWithTag(tags[t]);
    		}
   			for (int i = 0; i < objects.Length; i++){
   				if (objects[i] != null){
	   				//check each objects distance relative to the player transform, if outside the designated distance, it's inactive otherwise it's active.
	   				float dist =  Vector3.Distance(player.transform.position, objects[i].transform.position);
	   				if (dist > deactivateDist){
	   					objects[i].SetActive(false);
	   				}
	   				else{
	   					objects[i].SetActive(true);
	   				}
   				}
    		}
    
    	}
    	//UpdateArray();
    }
}