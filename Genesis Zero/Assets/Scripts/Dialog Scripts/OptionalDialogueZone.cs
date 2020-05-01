using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class OptionalDialogueZone : MonoBehaviour
{
    public string DialogueFileName = "";
    public bool BugeFlysOver = true;
    public bool TriggerRepeatedly = false;
    [Tooltip("0 - Merchant, 1 - Godhead, 2 - AI God Boss, 3 - Terminus Mind")]
    [Range(0, 3)]
    public int type;
    [Header("DialogueInfo")]
    public float triggerRadius = 10f;
    public float updatesPerSecond = 2f;
    public float BugeDuration = 4f;
    private BUGE buge;
    private GameObject player;
    private void Start()
    {
       buge =  GameObject.FindGameObjectWithTag("BUG-E").GetComponent<BUGE>();
       player = GameObject.FindGameObjectWithTag("Player");
       InvokeRepeating("CheckDist", 1 / updatesPerSecond, 1 / updatesPerSecond);
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
        newpoint.SetValues(vec, BugeDuration);
        DialogueInfo info = ScriptableObject.CreateInstance<DialogueInfo>();
        info.SetValues(newpoint, DialogueFileName, BugeFlysOver, GetInstanceID());
        if (dist < triggerRadius)
        {
            buge.GetComponent<BUGE>().FollowingPlayer(true);
            buge.GetComponent<BUGE>().AddOptionalDialoguePrompt(info);
        }
        else 
        {
            buge.GetComponent<BUGE>().TooFar(info);
        }
    }
    /**
    * Draw visual representations of trigger radius
    */
    protected virtual void OnDrawGizmos()
    {
       Gizmos.color = Color.magenta;
       Gizmos.DrawWireSphere(transform.position, triggerRadius*.8f);
    }
}
