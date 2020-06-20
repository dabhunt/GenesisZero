using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggers : MonoBehaviour
{
    private BUGE buge;
    private GameObject player;
    void Start()
    {
        buge = GetComponent<BUGE>();
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("CheckTriggers", 1, .5f);
    }
    public void CheckTriggers()
    {
        if (player != null && BUGE.instance.followingPlayer == true && Player.instance.GetInTutorial())
        {
            if (DialogueManager.instance.GetDialoguePlayedAmount("BUG-E_Heatbar") < 1 && player.GetComponent<OverHeat>().GetHeat() >= player.GetComponent<OverHeat>().GetMaxHeat()*.4f)
                DialogueManager.instance.TriggerDialogue("BUG-E_Heatbar", true);
            if (DialogueManager.instance.GetDialoguePlayedAmount("BUG-E_Modifiers") < 1 && Player.instance.GetSkillManager().GetModAmount() > 0)
                DialogueManager.instance.TriggerDialogue("BUG-E_Modifiers", true);
            if (DialogueManager.instance.GetDialoguePlayedAmount("BUG-E_Phasing") < 1 && Player.instance.GetHealth().GetValue() < Player.instance.GetHealth().GetMaxValue())
                DialogueManager.instance.TriggerDialogue("BUG-E_Phasing", true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
