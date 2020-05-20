using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealOnTriggerEnter : MonoBehaviour
{
	private GameObject player;
	private bool triggered;
	public int AmountHealed;
	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() == true && triggered == false)
		{
			other.GetComponentInParent<Player>().Heal(AmountHealed);
			triggered = true;
		}
	}
}
