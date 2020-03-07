using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	public GameObject thePlayer;
	
    private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
    	{
			//thePlayer.transform.position = destination.position;
			//thePlayer.transform.position = new Vector3(-120, 16, 0);
			teleport();
		}
		
	}
	private void Update()
	{
		if (Input.GetKey(KeyCode.ScrollLock))
		{
			teleport();
		}
	}
	private void teleport()
	{
		GameObject.FindWithTag("Player").transform.position = new Vector3(-120, 16, 0); ;
	}
}
