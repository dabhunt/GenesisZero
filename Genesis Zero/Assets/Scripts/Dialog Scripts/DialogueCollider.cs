using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCollider : MonoBehaviour
{
    public string DialogueFileName = "";
    public bool BugeFlysOver = true;
    public bool TriggerRepeatedly = false;
    private BUGE buge;
    [Tooltip("0 - Merchant, 1 - Godhead, 2 - AI God Boss, 3 - Terminus Mind")]
    [Range(0, 3)]
    public int type;
    private bool Triggered = false;

    private void Start()
    {
       buge =  GameObject.FindGameObjectWithTag("BUG-E").GetComponent<BUGE>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //if BUG-E has already pointed this gameObject out to the player
        if (DialogueFileName == null || DialogueFileName == "")
            return;
        //if we don't want this dialogue to play more than once, and it has already been played, don't play it
        if (!TriggerRepeatedly && DialogueManager.instance.GetDialoguePlayedAmount(DialogueFileName) > 0)
            return;
        if (other.GetComponent<Player>()) //if it's the player
        {
            if (Triggered)
                return;
            Triggered = true;
            DialogueManager.instance.TriggerDialogue(DialogueFileName);
            if (BugeFlysOver)
            {
                Vector2 vec = new Vector2(transform.position.x, transform.position.y + 2f);
                buge.AddWayPoint(vec, 4);
            }
        }
    }
}
