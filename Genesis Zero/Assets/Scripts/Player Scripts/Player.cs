using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    SkillManager SkillManager;
    // Start is called before the first frame update
    new void Start()
    {
        Time.fixedDeltaTime = .01f; // <- this is here as a placeholder, no other good place to have this.
        SkillManager = new SkillManager(this);
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        //Debug.Log("Player" + GetHealth().GetValue() +" : "+ GetHealth().GetMaxValue());
        base.Update();
    }

    new public void TakeDamage(float amount)
    {
        //Add anything if there is class specific additions to taking damage
        base.TakeDamage(amount);
    }

    public bool HasSkill(string name)
    {
        return SkillManager.HasSkill(name);
    }
}
