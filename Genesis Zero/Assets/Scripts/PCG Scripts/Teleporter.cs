using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	public float destinationX = -120;
	public float destinationY = 16;
	public float destinationZ = 0;
	private void Start() 
	{
	}
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

	}
	private void teleport()
	{
		GameObject.FindWithTag("Player").transform.position = new Vector2(destinationX, destinationY);
		TileManager.instance.curlevel++;
	}
	public void SetDesination(Vector2 destination)
	{
		destinationX = destination.x;
		destinationY = destination.y;
		destinationZ = 0;
	}
}
 