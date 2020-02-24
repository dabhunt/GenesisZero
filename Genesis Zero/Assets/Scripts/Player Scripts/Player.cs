using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    SkillManager SkillManager;
    public Statistic Essence;

    private void Awake()
    {
        SkillManager = new SkillManager(this);
    }
    // Start is called before the first frame update
    new void Start()
    {
        //Time.fixedDeltaTime = .01f; // <- this is here as a placeholder, no other good place to have this.
        InitializePlayerStats();
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Burn(3, 20);
        }
        //Debug.Log("Player" + GetHealth().GetValue() +" : "+ GetHealth().GetMaxValue());
        base.Update();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override float TakeDamage(float amount, Pawn source)
    {
        //Add anything if there is class specific additions to taking damage
        return base.TakeDamage(amount, source);
    }

    public bool HasSkill(string name)
    {
        return SkillManager.HasSkill(name);
    }

    public SkillManager GetSkillManager()
    {
        return SkillManager;
    }

    public void InitializePlayerStats()
    {
        Essence = new Statistic(500); Essence.SetValue(0);
    }

    public Statistic GetEssence()
    {
        return Essence;
    }

    public float GetEssenceAmount()
    {
        return Essence.GetValue();
    }

    public void SetEssence(float amount)
    {
        float num = amount;
        Mathf.Clamp(num, 0, 500);
        GetEssence().SetValue(num);
    }

    public void AddEssence(int amount)
    {
        SetEssence(GetEssenceAmount() + amount);
    }
}
