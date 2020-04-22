using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    SkillManager SkillManager;
    public Statistic Essence, Keys, AbilityPower;
    private List<Statistic> playerStatistics;
    public float baseAbilityPower = 16;
    private float EssenceHardCap = 60f;
    private float MaxEssence = 50f;
    private float MaxKeys = 3f;
    private float MaxCapsules = 5;
    private float healthPerStack = 3;

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
        foreach (Statistic stat in playerStatistics)
        {
            stat.UpdateStatistics();
        }
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
        int TH_stacks = GetSkillStack("Thrill of the Hunt");
        //reduce cooldowns by .5 seconds for each stack.
        GetComponent<AbilityCasting>().ReduceCooldowns(TH_stacks/2);
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
        playerStatistics = new List<Statistic>();
        Essence = new Statistic(EssenceHardCap);
        Essence.SetValue(0);
        playerStatistics.Add(Keys = new Statistic(MaxKeys));
        Keys.SetValue(0);
        playerStatistics.Add(AbilityPower = new Statistic(16));
        AbilityPower.SetValue(baseAbilityPower);
    }
    public Statistic GetAbilityPower()
    {
        return AbilityPower;
    }
    public float GetAbilityPowerAmount()
    {
        return AbilityPower.GetValue();
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
        num = Mathf.Clamp(num, 0, 3);
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
    //this changes how many capsules the player can store essence in
    public void SetMaxCapsuleAmount(float amount)
    {
        amount = Mathf.Clamp(amount, 4, 6);
        MaxEssence += GetEssencePerCapsule();
        MaxCapsules = amount;
        Canvas canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
        canvasRef.transform.Find("EssencePanel").gameObject.GetComponent<EssenceFill>().SetCapsuleAmount(amount);
    }
    //this refers to how many full canisters of esessence the player has (not how much essence fits in a canister)
    public int GetFullCapsuleAmount()
    {
        int amount = (int)(Essence.GetValue() / GetEssencePerCapsule());
        return amount;
    }
    //this returns how much essence can fit in a single capsule
    public int GetEssencePerCapsule()
    {
        return (int)(MaxEssence / MaxCapsules);
    }
    public void SetEssence(float amount)
    {
        float num = amount;
        num = Mathf.Clamp(num, 0, MaxEssence);
        GetEssence().SetValue(num);
    }
    public OverHeat GetOverHeat()
    {
        return GetComponent<OverHeat>();
    }
    public void AddEssence(int amount)
    {
        SetEssence(GetEssenceAmount() + amount);
    }
}
