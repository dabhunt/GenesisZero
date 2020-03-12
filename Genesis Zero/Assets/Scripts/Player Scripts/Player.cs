using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    SkillManager SkillManager;
    public Statistic Essence;
    public float healthPerStack = 4;

    private void Awake()
    {
        SkillManager = new SkillManager(this);
    }
    // Start is called before the first frame update
    new void Start()
    {
        InitializePlayerStats();
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SkillManager.SwapCurrentAbilities();
        }
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

    // This is called whenever the player kills a enemy
    public void TriggerEffectOnKill()
    {
        Heal(healthPerStack * GetSkillStack("Vampirism"));
    }

    public bool HasSkill(string name)
    {
        return SkillManager.HasSkill(name);
    }

    public int GetSkillStack(string name)
    {
        return SkillManager.GetSkillStack(name);
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
