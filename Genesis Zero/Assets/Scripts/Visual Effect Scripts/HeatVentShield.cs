using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HeatVentShield : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		Hitbox hitbox = other.GetComponentInParent<Hitbox>();
		if (hitbox && hitbox.HurtsAllies == true)
		{
			Destroy(hitbox.gameObject);
		}
	}
}
