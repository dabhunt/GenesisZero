using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class DialogueTriggerZone : MonoBehaviour
{
	[Header("DialogueInfo")]
	public string DialogueFileName = "Empty";

	public bool PauseGameWhenTriggered = false;
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
			DialogueManager.instance.TriggerDialogue(DialogueFileName, PauseGameWhenTriggered);
			triggered = true;
		}
	}

    void OnDrawGizmosSelected()
    {
       Gizmos.color = Color.magenta;
       //Gizmos.DrawWireSphere(transform.position + BUGEflyposition, .3f);
    }
}
