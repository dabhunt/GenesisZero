using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class CameraShakeOnTriggerEnter : MonoBehaviour
{
	private bool triggered;
	public float duration = .75f;
	public float strength = 1;
	public int virbrato = 5;
	public float randomness = 60;

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() && triggered == false)
		{
			Camera.main.gameObject.transform.DOShakePosition(duration: this.duration, strength: this.strength, vibrato: this.virbrato, randomness: this.randomness, snapping: false, fadeOut: true);
			triggered = true;
		}
	}
}
