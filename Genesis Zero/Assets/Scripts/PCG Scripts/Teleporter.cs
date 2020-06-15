using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
public class Teleporter : MonoBehaviour
{
	public float destinationX = -120;
	public float destinationY = 16;
	public float destinationZ = 0;
	public bool BossRoomOverride = false;
	public bool active = true;
	//Variables for animator

	private Transform player;
	private GameObject buge;
	public float portalactivedistance = 9.5f;
	private GameObject canvas;
	public bool hasPair = false;
	Animator ani;

	private void Start()
	{
		buge = GameObject.FindWithTag("BUG-E");
		player = GameObject.FindWithTag("Player").transform;    //Grabs the player transform position to use in the rest of the script.
		ani = GetComponent<Animator>();
		canvas = GameObject.FindGameObjectWithTag("CanvasUI");
		InvokeRepeating("portalAnimation", 1f, .3f);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() && active)
		{
			TeleportWithAnim();
		}
	}
	public void TeleportWithAnim()
	{
		print("teleport being called");
		active = false;
		Invoke("ReactivatePortal", 1.75f);
		StateManager.instance.Teleport(new Vector2(destinationX, destinationY), BossRoomOverride, hasPair, this.name);
		hasPair = true;
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
	private void ReactivatePortal()
	{
		active = true;
	}

	public void portalAnimation()
    {
			if(distanceFromPortal()<portalactivedistance){
				ani.SetBool("Open",true);
				if (AudioManager.instance.IsPlaying("TeleporterAmbient") < 1)
					AudioManager.instance.PlayAttachedSound("SFX_TeleporterAmbient", this.gameObject, .5f, 1, true, 0);
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
