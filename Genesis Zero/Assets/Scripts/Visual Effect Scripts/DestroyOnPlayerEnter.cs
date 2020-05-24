using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * Will destroy itself after detroying other listed gameobject
 * 
 */
[RequireComponent(typeof(Collider))]
public class DestroyOnPlayerEnter : MonoBehaviour
{
	public List<GameObject> Objects;

	public float delay;

	private bool triggered;

	public float lingertime; // Pulls player towards the center x -axis for the linger time.
	private bool destroyed;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (triggered && delay <= 0 && destroyed == false)
		{
			DestoryObjects();
		}
		else if (triggered)
		{
			delay -= Time.fixedDeltaTime;
		}

		if (destroyed && lingertime > 0)
		{
			lingertime -= Time.deltaTime;
			Transform t = GameObject.FindGameObjectWithTag("Player").transform;
			t.position += new Vector3((transform.position.x - t.position.x) * (Time.fixedDeltaTime * 2), 0, 0);
		}

	}

	private void OnTriggerStay(Collider other)
	{
		if (Objects.Count > 0 && other.GetComponentInParent<Player>() && triggered == false)
		{
			triggered = true;
		}
	}

	private void DestoryObjects()
	{
		foreach (GameObject g in Objects)
		{
			Destroy(g);
		}
		destroyed = true;
		Destroy(this.gameObject, lingertime);
	}
}
