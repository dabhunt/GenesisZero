using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teleporter : MonoBehaviour
{
	public float destinationX = -120;
	public float destinationY = 16;
	public float destinationZ = 0;

	//Variables for animator
	private Transform player;
	public float portalactivedistance=1f;
	Animator ani;

	private void Start()
	{
		player = GameObject.FindWithTag("Player").transform;	//Grabs the player transform position to use in the rest of the script.
		ani = GetComponent<Animator>();

	}
    private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
    	{
			//thePlayer.transform.position = destination.position;

			//thePlayer.transform.position = new Vector3(-120, 16, 0);
		//	if(GetAnimationStateTime()>=1){
		//	teleport();
		ani.Play("TeleportAnimation");
			}
		}

	private void Update()
	{
		portalAnimation();
	}
	public void callteleportfromAnimation(){
		teleport();
	}
	private void teleport()
	{
		player.position = new Vector2(destinationX, destinationY);
		GameObject.FindWithTag("CamCollider").transform.position = new Vector2(destinationX, GameObject.FindWithTag("CamCollider").transform.position.y);
		TileManager.instance.curlevel++;
	}
	public void SetDestination(Vector2 destination)
	{
		destinationX = destination.x;
		destinationY = destination.y;
		destinationZ = 0;
	}

	public float GetAnimationStateTime(){
		return Mathf.Repeat(ani.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f)+.1f;
	}

	public void portalAnimation()
    {
			if(distanceFromPortal()<portalactivedistance){
				ani.SetBool("Open",true);
				ani.SetBool("Close",false);
			}
			if(distanceFromPortal()>portalactivedistance){
				ani.SetBool("Close",true);
				ani.SetBool("Open",false);
			}


    }

	public float distanceFromPortal()
    {
		float distance;


		distance=Vector3.Distance(transform.position,player.position); //keeps everything a positive value and returns the distance
		return distance;
    }
}
