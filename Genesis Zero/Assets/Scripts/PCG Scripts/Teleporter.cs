using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	public int destinationX = -120;
	public int destinationY = 16;
	public int destinationZ = 0;
	
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
		GameObject.FindWithTag("Player").transform.position = new Vector3(destinationX, destinationY, destinationZ);
	}
}
