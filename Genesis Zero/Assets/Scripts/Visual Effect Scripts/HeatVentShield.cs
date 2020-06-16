using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HeatVentShield : MonoBehaviour
{

	private void OnTriggerEnter(Collider other)
	{
		Hitbox hitbox = other.GetComponentInParent<Hitbox>();
		if (hitbox != null && hitbox.HurtsAllies == true)
		{
			if (GetComponent<Pawn>() && hitbox.Damage >= GetComponent<Pawn>().GetHealth().GetValue())
			{
				AbilityCasting ac = Player.instance.gameObject.GetComponent<AbilityCasting>();
				ac.EndActive(ac.IsAbilityActive("Heat Vent Shield"));//end the active period early for heat vent shield if it gets killed early so it goes on cooldown
			}
			Destroy(hitbox.gameObject);
		}
	}
}
