using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class DialogueTriggerZone : MonoBehaviour
{
	[Header("DialogueInfo")]
	public string DialogueFileName = "Empty";

	public bool BugeFlysOver = false;
    public bool TriggerRepeatedly = false;
	public bool PauseGameWhenTriggered = false;

	public Vector3 BUGEflyposition;
	public float BUGEflyduration = 0;
    private BUGE buge;
    private GameObject player;

	private bool triggered;
    private void Start()
    {
       buge =  GameObject.FindGameObjectWithTag("BUG-E").GetComponent<BUGE>();
       player = GameObject.FindGameObjectWithTag("Player");
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>() == true && triggered == false)
		{
			if (BugeFlysOver)
			{
				WayPoint newpoint = ScriptableObject.CreateInstance<WayPoint>();
				newpoint.SetValues(transform.position + BUGEflyposition, BUGEflyduration);

				DialogueInfo info = ScriptableObject.CreateInstance<DialogueInfo>();
				info.SetValues(newpoint, DialogueFileName, BugeFlysOver, GetInstanceID());
				buge.GetComponent<BUGE>().AddOptionalDialoguePrompt(info);
			}

			DialogueManager.instance.TriggerDialogue(DialogueFileName, PauseGameWhenTriggered);
			//buge.GetComponent<BUGE>().FollowingPlayer(true);

			triggered = true;
		}
	}
	private void CheckDist() 
    {
        //if BUG-E has already pointed this gameObject out to the player
        if (DialogueFileName == null || DialogueFileName == "")
            return;
        //if we don't want this dialogue to play more than once, and it has already been played, don't play it
        if (!TriggerRepeatedly && DialogueManager.instance.GetDialoguePlayedAmount(DialogueFileName) > 0)
            return;
        if (player == null)
            return;
        float dist = Vector3.Distance(player.transform.position, this.transform.position);
        
        Vector2 vec = new Vector2(transform.position.x, transform.position.y + 2f);
        WayPoint newpoint = ScriptableObject.CreateInstance<WayPoint>();
        newpoint.SetValues(vec, 3);
        DialogueInfo info = ScriptableObject.CreateInstance<DialogueInfo>();
        info.SetValues(newpoint, DialogueFileName, BugeFlysOver, GetInstanceID());
        if (dist < 3)
        {
            if (buge.GetComponent<BUGE>().followingPlayer == true)
            {
                buge.GetComponent<BUGE>().AddOptionalDialoguePrompt(info);
            }
            else 
            {
                //play starting dialogue and pause the game with true param
                DialogueManager.instance.TriggerDialogue("StartDialogue", true, false);
                buge.GetComponent<BUGE>().FollowingPlayer(true);
            }            
        }
        else 
        {
            buge.GetComponent<BUGE>().TooFar(info);
        }
    }
    /**
    * Draw visual representations of trigger radius
    */
    void OnDrawGizmosSelected()
    {
       Gizmos.color = Color.magenta;
       Gizmos.DrawWireSphere(transform.position + BUGEflyposition, .3f);
    }
}
