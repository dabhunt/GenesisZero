﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    SkillManager SkillManager;
    public Statistic Essence;
    public Statistic Keys;
    private float MaxEssence = 100f;
    private float MaxKeys = 3f;
    private float MaxCapsules = 5;
    public float healthPerStack = 4;

    private void Awake()
    {
        SkillManager = new SkillManager(this);
    }
    new void Start()
    {
        InitializePlayerStats();
        base.Start();
    }

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
        Essence = new Statistic(MaxEssence); Essence.SetValue(0);
        Keys = new Statistic(MaxKeys); Keys.SetValue(0);
    }

    public Statistic GetEssence()
    {
        return Essence;
    }

    public float GetEssenceAmount()
    {
        return Essence.GetValue();
    }
    public float GetMaxEssenceAmount()
    {
        return MaxEssence;
    }
    public Statistic GetKeys()
    {
        return Keys;
    }
    public void SetKeys(float amount)
    {
        float num = amount;
        num = Mathf.Clamp(num, 0, MaxEssence);
        GetKeys().SetValue(num);
    }
    public void AddKeys(int amount)
    {
        SetKeys(GetKeysAmount() + amount);
    }
    public float GetKeysAmount()
    {
        return Keys.GetValue();
    }
    public float GetMaxKeysAmount()
    {
        return MaxKeys;
    }
    public float GetMaxCapsuleAmount()
    {
        return MaxCapsules;
    }
    public void SetMaxCapsuleAmount(float amount)
    {
        Mathf.Clamp(amount, 4, 6);
        MaxEssence += GetFullCapsuleAmount();
        MaxCapsules = amount;
    }
    public int GetFullCapsuleAmount()
    {
        int amount = (int)Essence.GetValue() / (int)MaxCapsules;
        return amount;
    }
    public void SetEssence(float amount)
    {
        float num = amount;
        Mathf.Clamp(num, 0, MaxEssence);
        GetEssence().SetValue(num);
    }

    public void AddEssence(int amount)
    {
        SetEssence(GetEssenceAmount() + amount);
    }
}
