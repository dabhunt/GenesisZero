using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	public Transform destination;
	public GameObject thePlayer;
	
    private void OnTriggerEnter(Collider other)
	{
		thePlayer.transform.position = destination.transform.position;
	}
}
