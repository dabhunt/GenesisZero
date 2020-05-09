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
        if (DialogueManager.instance.GetDialoguePlayedAmount("BUG-E_Heatbar") < 1 && player.GetComponent<OverHeat>().GetHeat() >= player.GetComponent<OverHeat>().GetMaxHeat())
            buge.AddOptionalDialoguePrompt("BUG-E_Heatbar");
        if (DialogueManager.instance.GetDialoguePlayedAmount("BUG-E_Modifiers") < 1 && player.GetComponent<Player>().GetSkillManager().GetModAmount() > 0)
            DialogueManager.instance.TriggerDialogue("BUG-E_Modifiers");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
