using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class TriggerInstructions : MonoBehaviour

{

	public string InstructionText;
	public bool PauseGameWhenTriggered = true;
	public float delay = 1.0f;
	private GameObject player;
	private GameObject popup;
	private bool triggered;
	
	private void Start()
	{
		player = Player.instance.gameObject;
		popup = GameObject.FindGameObjectWithTag("CanvasUI").transform.Find("InstructionPopup").gameObject;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() == true && triggered == false)
		{
			triggered = true;
			Invoke("Delayed", delay);
		}
	}
	private void Delayed()
	
	{
		popup.transform.Find("Text").gameObject.GetComponent<Text>().text = InstructionText;
		popup.SetActive(true);
		if (PauseGameWhenTriggered)
			StateManager.instance.PauseGame();
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			StateManager.instance.UnpauseGame();
			Player.instance.GetComponent<PlayerController>().FallFaster(-45);
			popup.SetActive(false);
			Destroy(gameObject.GetComponent<TriggerInstructions>());
		}
	}
}