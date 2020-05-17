using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTool : MonoBehaviour
{
    // Start is called before the first frame update
    SkillManager skMan;
    void Start()
    {
        InvokeRepeating("CheckAbilityCount", 1f, 1f);
        skMan = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().GetSkillManager();
    }

    public void CheckAbilityCount()
    {
        if (skMan.GetAbilityAmount() > 0)
            if (this.GetComponent<SimpleTooltip>() != null)
            {
                Destroy(this.GetComponent<SimpleTooltip>());
                Destroy(this.GetComponent<RemoveTool>());
            }

    }
}
