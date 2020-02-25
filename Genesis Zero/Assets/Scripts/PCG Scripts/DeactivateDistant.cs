using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateDistant : MonoBehaviour
{
    //at what minimum distance will enemies start to deactivate
    //enemies closer than this will be reactivated
    public float deactivateDist = 5f;
    public float updatesPerSecond=1f;
    //which game tags to check and deactivate if out of range
    public string[] tags = new string[] {"Enemy"};
    private Player player;
    void Start()
    {
       	GameObject temp = GameObject.FindWithTag("Player");
       	player = temp.GetComponent<Player>();
    	InvokeRepeating("Check", 1/updatesPerSecond, 1/updatesPerSecond);
    }
    // Update is called once per frame
    public void Check()
    {
    	//check all the tags provided
    	for (int t = 0; t < tags.Length; t ++)
    	{
    		//find the array of gameobjects associated with that tag
   			GameObject[] objects = GameObject.FindGameObjectsWithTag(tags[t]);
   			for (int i = 0; i < objects.Length; i++){
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
}
