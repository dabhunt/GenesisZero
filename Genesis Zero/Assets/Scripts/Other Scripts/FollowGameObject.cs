using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
	public GameObject FollowObject;
	public Vector3 Offset;
	public float LerpStrength = 1;
	// Start is called before the first frame update
	void Start()
	{
		transform.position = FollowObject.transform.position + Offset;
	}

	// Update is called once per frame
	void Update()
	{
		if (FollowObject != null)
			transform.position = Vector3.Lerp(transform.position, FollowObject.transform.position + Offset, LerpStrength);
	}

	[ExecuteInEditMode]
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(transform.position, 1);
	}
}
