using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teleporter : MonoBehaviour
{
	public float destinationX = -120;
	public float destinationY = 16;
	public float destinationZ = 0;
	public bool BossRoomOverride = false;
	//Variables for animator
	private Transform player;
	private GameObject buge;
	public float portalactivedistance = 9.5f;
	Animator ani;

	private void Start()
	{
		buge = GameObject.FindWithTag("BUG-E");
		player = GameObject.FindWithTag("Player").transform;    //Grabs the player transform position to use in the rest of the script.
		ani = GetComponent<Animator>();

	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>())
		{
			//thePlayer.transform.position = destination.position;

			//thePlayer.transform.position = new Vector3(-120, 16, 0);
			//	if(GetAnimationStateTime()>=1){
			Teleport();
			ani.Play("TeleportAnimation");
		}
	}


	private void Update()
	{
		portalAnimation();
	}
	public void callteleportfromAnimation(){
		Teleport();
	}
	private void Teleport()
	{
		if (BossRoomOverride == true)
			player.position = StateManager.instance.BossRoomLocation;
		
		player.position = new Vector2(destinationX, destinationY);
		//temporary code
		
		//temporary code ^
		buge.transform.position = player.position;
		buge.GetComponent<BUGE>().FollowingPlayer(true);
		//Camera.main.transform.position = new Vector3(player.position.x, player.position.y, -35.6f);
		//GameObject.FindWithTag("CamCollider").transform.position = new Vector2(destinationX, GameObject.FindWithTag("CamCollider").transform.position.y);
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
    protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, 3);
	}

	public float distanceFromPortal()
    {
		float distance;

		if (player == null) 
			return -1;
		distance=Vector3.Distance(transform.position,player.position); //keeps everything a positive value and returns the distance
		return distance;
    }
}
