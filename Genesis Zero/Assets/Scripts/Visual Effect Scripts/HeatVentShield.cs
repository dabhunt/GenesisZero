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
			if (GetComponentInParent<Pawn>())
			{
				GetComponentInParent<Pawn>().GetHealth().AddValue(-hitbox.Damage);
				if (GetComponentInParent<Pawn>().GetHealth().GetValue() <= 0)
				{
					EndHeatVent();
				}
			}
			Destroy(hitbox.gameObject);
		}
	}
	void OnDestroy()
	{
		EndHeatVent();
	}
	void EndHeatVent()
	{
		AbilityCasting ac = Player.instance.gameObject.GetComponent<AbilityCasting>();
		ac.EndActive(ac.IsAbilityActive("Heat Vent Shield"));//end the active period early for heat vent shield if it gets killed early so
		Player.instance.SetInvunerable(0);
	}
}
