using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindingBuge : MonoBehaviour
{
	private GameObject player;
	public GameObject invisWall;

	private void Start()
	{
		player = Player.instance.gameObject;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() == true)
		{

		}
	}
	private void Update()
	{
		if (StateManager.instance.InTutorial && Input.GetKeyDown(KeyCode.F))
		{
			GetComponent<BUGE>().followingPlayer = true;
			//play starting dialogue and pause the game with true param
			DialogueManager.instance.TriggerDialogue("StartDialogue", true, false);
			Destroy(gameObject.GetComponent<FindingBuge>());
			GetComponent<InteractPopup>().SetInteractable(false);
			GetComponent<InteractPopup>().DestroyPopUp();
			GetComponent<InteractPopup>().SetText("Right Click to Interact");
			Destroy(invisWall);
		}
	}
    void Cutscene()
    {
		
    }
}
