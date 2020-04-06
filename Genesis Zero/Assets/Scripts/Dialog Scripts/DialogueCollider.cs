using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCollider : MonoBehaviour
{
    public string DialogueFileName;
    public bool BugeFlysOver = false;
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
        //if the player has interacted with this type before, return
        if (DialogueManager.instance.GetInteractAmount(type) > 0)
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
