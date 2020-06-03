using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChangeZoomOnTriggerEnter : MonoBehaviour
{
	public float TargetFOV = 15;
	private GameObject player;
	private bool triggered;
	private void Start()
	{
		player = Player.instance.gameObject;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() == true && triggered == false)
		{
			Camera.main.GetComponent<BasicCameraZoom>().ChangeFieldOfView(TargetFOV, 1);
			triggered = true;
		}
	}

}
