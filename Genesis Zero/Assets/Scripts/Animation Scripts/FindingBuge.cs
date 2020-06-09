using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindingBuge : MonoBehaviour
{
	private GameObject player;
	//public GameObject invisWall;
	public bool PlayDialogueOnStart = false;
	private void Start()
	{
		if (PlayDialogueOnStart)
		{
			BUGE.instance.FollowingPlayer(true);
			//play starting dialogue and pause the game with true param
			DialogueManager.instance.TriggerDialogue("StartDialogue", true, false);
			Destroy(gameObject.GetComponent<FindingBuge>());
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>() == true)
		{
			CutsceneController.instance.BugeCutscene();
			Destroy(this.GetComponent<FindingBuge>());
		}
	}
}
