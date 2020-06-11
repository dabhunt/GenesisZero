using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public class UniqueEffects : MonoBehaviour
{
	//this script contains non bullet related unique effects of modifiers outside the data structure of stats
	//if this is 2, it checks twice per second
	public bool phaseTrigger = false;
	public float checksPerSecond = 4f;
	[Header("Music Cross Fade durations")]
	public float fadeIn = 3f;
	public float fadeOut = 5f;
	public float durationUntilDecay = 6f;
	//these are all done in multipliers, not in flat amounts
	// 1.1 = 10% increase, .9f = 10% reduction
	[Header("Power Surge")]
	public float PS_APmulti = 2;
	private float PreBonusAP = 0;
	[Header("Adrenaline Rush")]
	public float coolReductionPerStack = .5f;
	public float coolReductionSingleStack = 1;
	[Header("Better Coolant")]
	public float coolRatePerStack = 1.1f;
	[Header("Boiling Point")]
	public float BP_MaxExtraHeat = 3.5f;
	public float BP_MaxExtraDMG = 3.5f;
	[Header("Heat Expulsion")]
	public float HE_Damage = 20;
	public float HE_TotalBurnDMG = 21;
	public float HE_StackMulti = 1.4f;
	[Header("Chemical Accelerant")]
	public float CA_MaxAttackSpeedPerStack = .50f;
	public float CA_MaxCritChancePerStack = .25f;
	[Header("Amplified Essence")]
	public float AE_MaxAPMulti = 3f;
	[Header("Phase Fuse")]
	public float PF_CD_Firststack = 2.5f;
	public float PF_ExtraStacks = 1f;
	[Header("Time Hack")]
	public float TH_Firststack = .75f;
	public float TH_ExtraStacks = .4f;
	[Header("Superheated Essence")]
	public float SE_MaxAttackSpeed = 1f;
	public float SE_MaxExtraBloom = .35f;
	[Header("Thermite Core")]
	public float TC_MaxAttackSpeed = 1f;
	public float TC_MaxExtraBloom = .35f;
	[Header("Heat Sink")]
	public float HS_APBonus = 1.5f;
	[Header("Cooling Cell")]
	public float heatReductionPerStack = 1.1f;
	[Header("Spartan Laser")]
	public float SL_cooldown = 4f;
	public float SL_bonusPerKill = 10f;
	public float SL_maxDmg = 100f;
	public float SL_decayTime = 10f;
	//how many seconds before bonus damage goes away
	private float SL_decay;
	private int SL_killCount;
	//time in seconds before music starts to fade out after combat
	private float MusicDecay = 10;
	private float currentAttackSpeed = 0;
	private float currentCritChance = 0;
	private AudioManager aManager;
	public bool CombatChangesMusic = true;
	private Player player;
	private OverHeat overheat;
	private bool incombat = false;
	void Start()
	{
		player = GetComponent<Player>();
		overheat = GetComponent<OverHeat>();

		float repeatRate = 1 / checksPerSecond - (Time.deltaTime * 2);
		if (repeatRate > 0.01f)
			InvokeRepeating("CheckUniques", 0, repeatRate);
		aManager = AudioManager.instance;
	}
	public void CheckUniques()
	{
		//Generic Triggers based on repeated checks
		HeatReduction();
		HalfHeatTrigger();
		//specific Mod effects
		BoilingPoint();
		ChemicalAccelerant();
		AmplifiedEssence();
		ConcentratedEssence();
		UnstableEssence();
		SuperHeatedEssence();
		ThermiteCore();
		MusicTimer();
		
		if (phaseTrigger)
		{
			PowerSurge();
		}
		player.UpdateStats();
		StatDisplay.instance.UpdateStats();
	}
	private void Update()
	{
		StackDecayTimer();
	}
	private void MusicTimer()
	{
		MusicDecay -= 1 / checksPerSecond;
		if (MusicDecay < 0)
		{
			ExitCombatMusic();
			Invoke("ExitCombat", fadeOut);
		}
	}
	public void OverHeatTrigger()
	{
		int HE_stacks = player.GetSkillStack("Heat Expulsion");
		if (HE_stacks > 0)
		{
			//HE_stack multi is 1.2f, which represents a 20% increase in size for each modifier past 1
			float multi = player.GetSkillManager().GetSkillStackAsMultiplier("Heat Expulsion", HE_StackMulti);
			GameObject hitbox = SpawnGameObject("ExpulsionHitbox", transform.position, Quaternion.identity);
			Hitbox hit = hitbox.GetComponent<Hitbox>();
			//Sets damage to double player's * stack multiplier of 20% per stack, false means it can't crit
			hitbox.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue() * 2f * multi, player, false);
			hitbox.transform.parent = transform;
			hit.SetLifeTime(.3f);
			hit.Burn = new Vector2(3, HE_TotalBurnDMG / 3);
			hit.SetStunTime(1f);
			hitbox.GetComponent<SphereCollider>().radius = 2 * multi;
			VFXManager.instance.PlayEffect("VFX_HeatExpulsion", hitbox.transform.position,0, 2*multi);
			aManager.PlaySoundOneShot("SFX_ExplosionEnemy");
			print("Overheat is happening boss");
			overheat.SetHeat(0);
		}
	}
	//while above 50% heat, the player does this
	public void HalfHeatTrigger()
	{
		float stacks = player.GetSkillStack("Heat Sink");
		float heat = overheat.GetHeat();
		if (stacks > 0 && heat >= 50)
		{
			float bonusAP = player.GetAbilityPower().GetBaseValue() * HS_APBonus * (heat / 100);
			player.GetAbilityPower().AddRepeatingBonus(bonusAP, bonusAP, .8f, "HeatSink_AP");
			player.GetAbilityPower().CheckBonuses();
		}
	}

	public void AbilityTrigger()
	{
		//before abilities are casted in AC
	}
	//this is done after the ability so that heat vent shield isn't useless
	public void AfterAbilityTrigger()
	{
		float stacks = player.GetSkillStack("Heat Sink");
		float heat = overheat.GetHeat();
		if (stacks > 0 && heat >= 50)
		{
			player.GetAbilityPower().EndRepeatingBonus("HeatSink_AP");
			player.GetAbilityPower().CheckBonuses();
			overheat.SetHeat(0);
		}
	}
	public void WeakPointHit()
	{
		float stacks = player.GetSkillManager().GetSkillStack("Adrenaline Rush");
		player.GetComponent<AbilityCasting>().ReduceCooldowns(stacks);
		//other Modifier effects can be put inside this function
	}
	public void DamageGivenTrigger()
	{
		MusicDecay = durationUntilDecay;
		print("damage given");
		if (CombatChangesMusic && incombat == false)
			EnterCombatMusic(); //if not in combat and combat should change music
	}
	public bool IsInCombat()
	{
		return incombat;
	}
	private void ExitCombat()
	{
		incombat = false;
	}
	public void EnterCombatMusic()
	{
		incombat = true;
		//Camera.main.GetComponent<BasicCameraZoom>().ChangeFieldOfViewTemporary(20, 9, .5f);
		print("entercombatmusic called");
		AudioManager.instance.CrossFadeChannels(1, 5.0f, 2, fadeIn);
		//AudioManager.instance.CrossFadeChannels(2, "Music", "CombatMusic", true, 15);
	}
	public void ExitCombatMusic()
	{
		incombat = false;
		//Camera.main.GetComponent<BasicCameraZoom>().StopTempFieldOfViewChange();
		if (CombatChangesMusic)
		{
			AudioManager.instance.CrossFadeChannels(2, 5.0f, 1, fadeOut);
		}
	}
	void StackDecayTimer()
	{
		if (SL_killCount > 0)
		{
			SL_decay += Time.fixedDeltaTime;
			if (SL_decay > SL_decayTime)
			{
				SL_decay = 0;
				SL_killCount = 0;
			}
		}
	}
	private void HeatReduction()
	{
		float reduction = 1 - (Mathf.Pow(heatReductionPerStack, player.GetSkillStack("Better Coolant")));
		float lessHeat = overheat.GetHeatAddedPerShot().GetBaseValue() * -reduction;
		overheat.GetHeatAddedPerShot().AddRepeatingBonus(lessHeat, 0, 1 / checksPerSecond, "BetterCoolant");
		reduction = 1 - Mathf.Pow(coolRatePerStack, player.GetSkillStack("Cooling Cell"));
		float lessDelay = overheat.GetDelayBeforeCooling().GetBaseValue() * -reduction;
		overheat.GetDelayBeforeCooling().AddRepeatingBonus(lessDelay, 0, 1 / checksPerSecond, "CoolingCell");
	}
	//Player deals bonus damage proportionate to how much essence they have, up to a cap of their base damage
	// Player has less attack speed proportionate to how much essence they have, up to a cap of half as much base attack speed
	private void ConcentratedEssence()
	{
		int stacks = player.GetSkillStack("Concentrated Essence");
		if (stacks > 0)
		{
			float ratio = player.GetEssenceAmount() / player.GetMaxEssenceAmount();
			//Sets the players bonus AP proportionate to how much AP they have, up to a cap of x3 bonus
			float cur_ADbonus = (player.GetDamage().GetBaseValue() * 1f) * stacks * ratio;
			float cur_ASdebuff = (player.GetAttackSpeed().GetBaseValue() * -.5f) * stacks * ratio;
			player.GetDamage().AddRepeatingBonus(cur_ADbonus, cur_ADbonus, 1 / checksPerSecond, "ConcentratedEssence_DMG");
			player.GetAttackSpeed().AddRepeatingBonus(cur_ASdebuff, 0, 1 / checksPerSecond, "ConcentratedEssence_AS");
		}
	}
	//Player gains bonus attack speed proportional to how much heat AND essence the player has
	private void SuperHeatedEssence()
	{
		int stacks = player.GetSkillStack("Superheated Essence");
		if (stacks > 0)
		{
			float ratio = player.GetEssenceAmount() / player.GetMaxEssenceAmount();
			float heat = player.GetComponent<OverHeat>().GetHeat();
			//Sets the players bonus attack speed equal to the max value proportionate to how much heat you have, up to the max of 100 heat
			float multiplier = ratio * stacks * (heat / 100);
			float cur_ASbonus = (player.GetAttackSpeed().GetBaseValue() * SE_MaxAttackSpeed) * multiplier;
			float cur_bloomDebuff = player.GetOverHeat().GetBloomMultiplier().GetBaseValue() * SE_MaxExtraBloom * multiplier;
			player.GetAttackSpeed().AddRepeatingBonus(cur_ASbonus, cur_ASbonus, 1 / checksPerSecond, "SuperheatedEssence_AS");
			player.GetOverHeat().GetBloomMultiplier().AddRepeatingBonus(cur_bloomDebuff, cur_bloomDebuff, 1 / checksPerSecond, "SuperheatedEssence_Bloom");
		}
	}
	//Player gains bonus attack speed and loses accuracy proportional to how much heat the player has
	private void ThermiteCore()
	{
		int stacks = player.GetSkillStack("Thermite Core");
		if (stacks > 0)
		{
			float heat = player.GetComponent<OverHeat>().GetHeat();
			//Sets the players bonus attack speed equal to the max value proportionate to how much heat you have, up to the max of 100 heat
			float multiplier = stacks * (heat / 100);
			float cur_ASbonus = (player.GetAttackSpeed().GetBaseValue() * TC_MaxAttackSpeed) * multiplier;
			float cur_bloomDebuff = player.GetOverHeat().GetBloomMultiplier().GetBaseValue() * TC_MaxExtraBloom * multiplier;
			player.GetAttackSpeed().AddRepeatingBonus(cur_ASbonus, cur_ASbonus, 1 / checksPerSecond, "ThermiteCore_AS");
			player.GetOverHeat().GetBloomMultiplier().AddRepeatingBonus(cur_bloomDebuff, cur_bloomDebuff, 1 / checksPerSecond, "ThermiteCore_Bloom");
		}
	}
	//Player deals bonus damage proportionate to how much essence they have, up to a cap of their base damage x2 (x3 total including base AD)
	// Player receives extra damage, proporitionate to how much essence they have, up to a cap of double damage received
	private void UnstableEssence()
	{
		int stacks = player.GetSkillStack("Unstable Essence");
		if (stacks > 0)
		{
			float ratio = player.GetEssenceAmount() / player.GetMaxEssenceAmount();
			float cur_ADbonus = (player.GetDamage().GetBaseValue() * 2f) * stacks * ratio;
			player.GetDamage().AddRepeatingBonus(cur_ADbonus, cur_ADbonus, 1 / checksPerSecond, "UnstableEssence_DMG");
		}
	}
	//Player abilities deal bonus damage proportionate to how much essence they have, up to a cap of their base AP damage
	private void AmplifiedEssence()
	{
		int stacks = player.GetSkillStack("Amplified Essence");
		if (stacks > 0)
		{
			float ratio = player.GetEssenceAmount() / player.GetMaxEssenceAmount();
			//Sets the players bonus AP proportionate to how much AP they have, up to a cap of x3 bonus
			float currentAPbonus = (player.GetAbilityPower().GetBaseValue() * 1f) * stacks * ratio;
			player.GetAbilityPower().AddRepeatingBonus(currentAPbonus, currentAPbonus, 1 / checksPerSecond, "AmplifiedEssence");
		}
	}
	//extra damage based on how much heat
	private void ChemicalAccelerant()
	{
		int stacks = player.GetSkillStack("Chemical Accelerant");
		if (stacks > 0)
		{
			float heat = player.GetComponent<OverHeat>().GetHeat();
			//Sets the players bonus attack speed equal to the max value proportionate to how much heat you have, up to the max of 100 heat
			currentAttackSpeed = CA_MaxAttackSpeedPerStack * stacks * (heat / 100);
			currentCritChance = CA_MaxCritChancePerStack * stacks * (heat / 100);
			player.GetAttackSpeed().AddRepeatingBonus(currentAttackSpeed, currentAttackSpeed, 1 / checksPerSecond, "ChemicalAccelerant_AS");
			player.GetCritChance().AddRepeatingBonus(currentCritChance, currentCritChance, 1 / checksPerSecond, "ChemicalAccelerant_CC");
		}
	}
	public void PhaseTrigger()
	{
		phaseTrigger = true;
		int stacks = player.GetSkillStack("Phase Fuse");
		int TH_stacks = player.GetSkillStack("Time Hack");
		if (stacks > 0)
		{
			float reduction = ((stacks - 1) * PF_ExtraStacks) + PF_CD_Firststack;
			player.GetComponent<AbilityCasting>().ReduceSlotCooldown(reduction, 1);
		}
		if (TH_stacks > 0)
		{
			print("you got some Timehack");
			float duration = ((TH_stacks - 1) * TH_ExtraStacks) + TH_Firststack;
			StateManager.instance.ChangeTimeScale(.4f,duration);
		}
	}
	private void PowerSurge()
	{
		int stacks = player.GetSkillStack("Power Surge");
		if (stacks > 0)
		{
			float ap;
			if (PreBonusAP == 0) //if prebonus has not been set, set it to current ap value
				PreBonusAP = player.GetAbilityPower().GetValue();
			ap = PreBonusAP;
			float bonus = ap * (PS_APmulti - 1 + stacks) - ap;
			player.GetAbilityPower().AddRepeatingBonus(bonus, 0, 1 / checksPerSecond, "PS_TempAbilityMultiplier");
		}
	}
	public void ResetPhaseTrigger()
	{
		phaseTrigger = false;
		PreBonusAP = 0;
	}
	//Bullets deal bonus damage, based on how much heat you have, up to a cap of 35% more damage
	//Player has a constant 35% more heat per shot fired.
	private void BoilingPoint()
	{
		int stacks = player.GetSkillStack("Boiling Point");
		if (stacks > 0)
		{
			float heat = player.GetComponent<OverHeat>().GetHeat();
			//Sets the players bonus attack speed equal to the max value proportionate to how much heat you have, up to the max of 100 heat
			float currentDamage = BP_MaxExtraDMG * stacks * (heat / 100);
			float moreHeat = BP_MaxExtraHeat * overheat.GetHeatAddedPerShot().GetBaseValue() * stacks;
			overheat.GetHeatAddedPerShot().AddRepeatingBonus(moreHeat, 0, 1 / checksPerSecond, "BoilingPoint_HeatPerShot");
			player.GetDamage().AddRepeatingBonus(currentDamage, currentAttackSpeed, 1 / checksPerSecond, "BoilingPoint_DMG");
		}
	}
	public float SL_CalculateDmg()
	{
		aManager.PlaySoundOneShot("SFX_AOE");
		float AP = player.GetAbilityPower().GetValue();
		return Mathf.Clamp(AP * 1.5f + SL_bonusPerKill * SL_killCount, AP, SL_maxDmg);
	}
	public int GetKillCount()
	{
		return SL_killCount;
	}
	public void IncrementKillCount()
	{
		SL_killCount++;
		SL_decay = 0;
	}
	private GameObject SpawnGameObject(string name, Vector2 position, Quaternion quat)
	{
		GameObject effect = Instantiate(Resources.Load<GameObject>("Hitboxes/" + name), position, quat);
		return effect;
	}
}
