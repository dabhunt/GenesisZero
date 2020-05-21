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
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() == true && triggered == false)
		{
			Camera.main.GetComponent<BasicCameraZoom>().ChangeFieldOfView(TargetFOV);
			triggered = true;
		}
	}

}
