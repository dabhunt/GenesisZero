using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EmitOnTriggerEnter : MonoBehaviour
{
	public GameObject Emit;
	public Vector3 position;
	public float delay;
	// Start is called before the first frame update
	private bool triggered;
	private bool spawned;

	private void Update()
	{
		if (delay < 0 && triggered && spawned == false)
		{
			GameObject emit = (GameObject)Instantiate(Emit, transform.position + position, Quaternion.identity);
			spawned = true;
		}
		if (triggered)
		{
			delay -= Time.deltaTime;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() && triggered == false)
		{		
			triggered = true;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position + position, 1);
	}
}
