using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateDistant : MonoBehaviour
{
    //at what minimum distance will enemies start to deactivate
    //enemies closer than this will be reactivated
    public float deactivateDist = 32f;
    public float updatesPerSecond=1f;
    public float enemyDistOffset = 5f;
    //which game tags to check and deactivate if out of range
    public string[] tags = new string[] {"Enemy"};
    private List<GameObject> objects = new List<GameObject>();
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
    	
    		//find the array of gameobjects associated with that tag
    		// if its the first check of the game, use a find by tag
    		//all other checks will use the already existing array of enemies to do this check
    		if (firstCheck){
                //NOTE Finding by tag does not work if they are deactive
                for (int t = 0; t < tags.Length; t ++){
                    objects.AddRange(GameObject.FindGameObjectsWithTag(tags[t]));
                }
    			firstCheck = false;
    		}
   			for (int i = 0; i < objects.Count; i++){
   				if (objects[i] != null){
                    //check each objects distance relative to the player transform, if outside the designated distance, it's inactive otherwise it's active.
                    float dist = Vector3.Distance(player.transform.position, objects[i].transform.position);
                    if (objects[i].gameObject.tag == "Enemy")
                    {
                        //make it so that enemies load in slower than cubbies, preventing them from falling through cubbies that haven't loaded yet
                        dist -= enemyDistOffset;
                    }
	   				
	   				
	   				if (dist > deactivateDist){
	   					objects[i].SetActive(false);
	   				}
	   				else{
	   					objects[i].SetActive(true);
	   				}
   				}
    		}
    
    	
    	//UpdateArray();
    }
}
