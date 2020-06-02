using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    Dictionary<string, int> Skills;

    private List<SkillObject> skillobjects; // List of all skillobjects(mods and abilities) the player has.
    private List<SkillObject> playermods;
    private List<SkillObject> playerwhitemods; //  List of white mods the player has
    private List<SkillObject> playerbluemods; // List of blue mods the player has
    private List<SkillObject> playergoldmods;  //  List of gold mods the player has
    private List<SkillObject> playerabilities;
    private SkillObject AddedSkill; //Null when no skills have been added recently
    private Player player;
    private int skillamount;
    private int abilityamount;
    private int modamount;
    private int uniquemodamount;
    private string ability1 = "";
    private string ability2 = "";
    private int uniqueModLimit = 8;
    private int ClampModLimit = 11;

    private bool updated;

    // Reference lists for all mods in the game
    private List<SkillObject> whitemods; // List of whitemods in the game
    private List<SkillObject> bluemods; // List of bluemods in the game
    private List<SkillObject> goldmods;  // List of goldmods in the game
    private List<SkillObject> startermods; // List of startermods in the game
    private List<SkillObject> abilities;

    // Lower the number, lower the chance. Determined by this order (0 - 100)
    private int goldchance = 10; 
    private int bluechance = 40; // Realistically its bluechance - goldchance in code
    // No white chance becuase it is the default if the other two do not go through

    public SkillManager(Player p)
    {
        player = p;
        Skills = new Dictionary<string, int>();
        skillobjects = new List<SkillObject>();
        playermods = new List<SkillObject>();
        playerwhitemods = new List<SkillObject>();
        playerbluemods = new List<SkillObject>();
        playergoldmods = new List<SkillObject>();
        playerabilities = new List<SkillObject>();
        whitemods = new List<SkillObject>();
        bluemods = new List<SkillObject>();
        goldmods = new List<SkillObject>();
        startermods = new List<SkillObject>();
        abilities = new List<SkillObject>();
        InitializeLists();
    }

    /**
     * Adds stats to the player using the data in the SkillObject.
     * 
     */
    public void AddSkill(SkillObject skill)
    {
        if (Skills.ContainsKey(skill.name)) // Adds to the stack of skills if player already has one
        {
            Skills[skill.name] = Skills[skill.name] + 1;
        }
        else
        {
            if (skill.IsAbility)
            {
                skillobjects.Add(skill);
                Skills.Add(skill.name, 1);
                abilityamount++;
                playerabilities.Add(skill);
                if (ability1 == "")
                {
                    SetAbility1(skill.name);
                }
                else if (ability2 == "")
                {
                    SetAbility2(skill.name);
                }
            }
            else
            {
                if (GetUniqueModAmount() >= GetModSlotLimit())
                    return;
                skillobjects.Add(skill);
                Skills.Add(skill.name, 1);
                modamount++;
                playermods.Add(skill);
                switch (skill.Rarity)
                {
                    case 1:
                        playerwhitemods.Add(skill);
                        break;
                    case 2:
                        playerbluemods.Add(skill);
                        break;
                    case 3:
                        playergoldmods.Add(skill);
                        break;
                    default:
                        Debug.LogWarning("Skill Object [" + skill.name + "] is not initialized with a proper rarity (0 - 3)");
                        playerwhitemods.Add(skill);
                        break;
                }

            }
        }
        AddedSkill = skill;
        AddSkillStats(skill, true);
        skillamount++;
    }

    /**
     * Removes a stack of skill
     */
    public void RemoveSkill(SkillObject skill)
    {
        if (Skills.ContainsKey(skill.name)) // Removes the skill from the dictionary
        {
            Skills[skill.name] = Skills[skill.name] - 1;
            if (Skills[skill.name] <= 0)
            {
                Skills.Remove(skill.name);
                skillobjects.Remove(skill);
                if (skill.IsAbility)
                {
                    abilityamount--;
                    playerabilities.Remove(skill);
                    if (skill.name == ability1)
                    {
                        SetAbility1("");
                    }
                    else
                    {
                        SetAbility2("");
                    }
                }
                else
                {
                    modamount--;
                    playermods.Remove(skill);
                    switch (skill.Rarity)
                    {
                        case 1:
                            playerwhitemods.Remove(skill);
                            break;
                        case 2:
                            playerbluemods.Remove(skill);
                            break;
                        case 3:
                            playergoldmods.Remove(skill);
                            break;
                        default:
                            Debug.LogWarning("Skill Object [" + skill.name + "] is not initialized with a proper rarity (0 - 3)");
                            playerwhitemods.Remove(skill);
                            break;
                    }
                }
            }
            AddedSkill = null;
            AddSkillStats(skill, false);
            skillamount--;
        }
    }

    /**
     * Adds the stats to the player. If Adding is true
     * Then it is a positive addition, otherwise it is negative
     */
    private void AddSkillStats(SkillObject skill, bool IsAdding)
    {
        int multi = IsAdding ? 1 : -1;
        player.GetHealth().AddMaxValue(skill.health * multi);
        if (player.GetHealth().GetValue() < 1)
            player.GetHealth().SetValue(1);
        player.GetDamage().AddMaxValue(skill.damage * multi);
        player.GetSpeed().AddMaxValue(skill.speed * multi);
        player.GetAttackSpeed().AddMaxValue(skill.attackspeed * multi);
        player.GetFlatDamageReduction().AddMaxValue(skill.flatdamagereduction * multi);
        player.GetDamageReduction().AddMaxValue(skill.damagereduction * multi);
        player.GetDodgeChance().AddMaxValue(skill.dodgechance * multi);
        player.GetCritChance().AddMaxValue(skill.critchance * multi);
        player.GetCritDamage().AddMaxValue(skill.critdamage * multi);
        player.GetRange().AddMaxValue(skill.range * multi);
        player.GetShield().AddMaxValue(skill.shield * multi);
        player.GetWeight().AddMaxValue(skill.weight * multi);
        player.GetAbilityPower().AddMaxValue(skill.abilitypower * multi);
    }
    private void InitializeLists()
    {
        SkillObject[] skills = Resources.LoadAll<SkillObject>("Skills/Modifiers");
        for (int i = 0; i < skills.Length; i++)
        {
            switch (skills[i].Rarity)
            {
                case 1:
                    whitemods.Add(skills[i]);
                    break;
                case 2:
                    bluemods.Add(skills[i]);
                    break;
                case 3:
                    goldmods.Add(skills[i]);
                    break;
                default:
                    Debug.LogWarning("Skill Object [" + skills[i].name + "] is not initialized with a proper rarity (0 - 3)");
                    whitemods.Add(skills[i]);
                    break;
            }
        }

        SkillObject[] sskills = Resources.LoadAll<SkillObject>("Skills/Starter Mods");
        for (int i = 0; i < sskills.Length; i++)
        {
            startermods.Add(sskills[i]);
        }

        SkillObject[] abils = Resources.LoadAll<SkillObject>("Skills/Abilities");
        foreach (SkillObject ab in abils)
        {
            abilities.Add(ab);
        }
    }

    /**
     * Returns whether or not the SkillManager contains the skill
     */
    public bool HasSkill(string name)
    {
        bool hasskill = Skills.ContainsKey(name);
        return Skills.ContainsKey(name);
    }

    [System.Obsolete(" :P Kenny Here, this function is deprecated. Use GetRandomMod() instead ")]
    public SkillObject GetRandomSkill()
    {
        Object[] skills = Resources.LoadAll<SkillObject>("Skills/Modifiers");
        SkillObject skill = (SkillObject)skills[Random.Range(0, skills.Length)];
        //this fixes the problem of this function also returning abilities
        while (skill.IsAbility)
        {
            skill = (SkillObject)skills[Random.Range(0, skills.Length)];
        }
        return skill;
    }

    /**
    * Returns a random SkillObject that exists in the resources/skills/starter mods folder
    */
    public SkillObject GetRandomStarterMod()
    {
        SkillObject SM = (SkillObject)startermods[Random.Range(0, startermods.Count)];
        return SM;
    }

    /**
    * Returns random starter mods of specified rarity in the game pool. NOT from player
    */
    public List<SkillObject> GetRandomStarterMods(int amount)
    {
        List<SkillObject> returnlist = new List<SkillObject>();
        List<SkillObject> picklist = new List<SkillObject>();
        picklist.AddRange(startermods);

        // Once the the cases have been determined, random (non-duplicate) mods are picked from the game pool of mods
        for (int i = 0; i < amount; i++)
        {
            int num = Random.Range(0, picklist.Count);
            returnlist.Add(picklist[num]);
            picklist.Remove(picklist[num]);
        }
        return returnlist;
    }

    /**
    * Returns a random SkillObject that exists in the resources/skills folder
    * Includes whites, blues, and golds
    */
    public SkillObject GetRandomMod()
    {
        Object[] skills = Resources.LoadAll<SkillObject>("Skills/Modifiers");
        SkillObject skill = (SkillObject)skills[Random.Range(0, skills.Length)];
        //this fixes the problem of this function also returning abilities
        skill = (SkillObject)skills[Random.Range(0, skills.Length)];
        return skill;
    }

    /**
    * Returns a random SkillObject/mod incoporating chance
    * Includes whites, blues, and golds from the gamep pool. NOT from player
    */
    public SkillObject GetRandomModByChance()
    {
        int num = Random.Range(0, 100);

        if (num < goldchance) // Gold (0 - 10)
        {
            return GetRandomGolds(1)[0];
        }
        else if (num < bluechance) // Green (11 - 40)
        {
            return GetRandomGreens(1)[0];
        }
        else // White (41 - 100)
        {
            return GetRandomWhites(1)[0];
        }
    }

    /**
    * Returns an amount of random mods incoporating chance
    * Includes whites, blues, and golds from the gamep pool. NOT from player
    */
    public List<SkillObject> GetRandomModsByChance(int amount)
    {
        List<SkillObject> returnlist = new List<SkillObject>();
        for (int i = 0; i < amount; i++)
        {
            int num = Random.Range(0, 100);

            if (num < goldchance) // Gold (0 - 10)
            {
                returnlist.Add(GetRandomGolds(1)[0]);
            }
            else if (num < bluechance) // Green (11 - 40)
            {
                returnlist.Add(GetRandomGreens(1)[0]);
            }
            else // White (41 - 100)
            {
                returnlist.Add(GetRandomWhites(1)[0]);
            }
        }
        return returnlist;
    }

    /**
    * Returns a random SkillObject that exists in the resources/skills/abilities folder
    */
    public SkillObject GetRandomAbility()
    {
        SkillObject skill = (SkillObject)abilities[Random.Range(0, abilities.Count)];
        return skill;
    }

    public List<SkillObject> GetSkillObjects()
    {
        return skillobjects;
    }

    public List<SkillObject> GetPlayerMods()
    {
        return playermods;
    }

    /**
     * Returns random mods of specified rarity in the game pool. NOT from player
     */
    public List<SkillObject> GetRandomMods(int amount, int rarity)
    {
        List<SkillObject> returnlist = new List<SkillObject>();
        List<SkillObject> picklist = new List<SkillObject>();
        switch (rarity)
        {
            case 1:
                picklist.AddRange(whitemods);
                break;
            case 2:
                picklist.AddRange(bluemods);
                break;
            case 3:
                picklist.AddRange(goldmods);
                break;
            default:
                Debug.LogWarning("Improper rarity used in function call. Defaulting to white rarity for mods");
                picklist.AddRange(whitemods);
                break;
        }

        // Once the the cases have been determined, random (non-duplicate) mods are picked from the game pool of mods
        for (int i = 0; i < amount; i++)
        {
            int num = Random.Range(0, picklist.Count);
            returnlist.Add(picklist[num]);
            picklist.Remove(picklist[num]);
        }
        return returnlist;
    }

    public void SetCountByName(string name, int num)
    {
        if (Skills.ContainsKey(name)) // Adds to the stack of skills
        {
            Skills[name] = num;
        }
    }

    public List<SkillObject> GetRandomWhites(int amount)
    {
        return GetRandomMods(amount, 1);
    }

    public List<SkillObject> GetRandomGreens(int amount)
    {
        return GetRandomMods(amount, 2);
    }

    public List<SkillObject> GetRandomGolds(int amount)
    {
        return GetRandomMods(amount, 3);
    }

    /**
     * Returns a random (non-duplicate) number of mods of a specific rarity from the player
     */
    public List<SkillObject> GetRandomModsFromPlayer(int amount, int rarity)
    {
        List<SkillObject> returnlist = new List<SkillObject>();
        List<SkillObject> picklist = new List<SkillObject>();
        switch (rarity)
        {
            case 1:
                if (amount > playerwhitemods.Count)
                {
                    return playerwhitemods;
                }
                picklist.AddRange(playerwhitemods);
                break;
            case 2:
                if (amount > playerbluemods.Count)
                {
                    return playerbluemods;
                }
                picklist.AddRange(playerbluemods);
                break;
            case 3:
                if (amount > playergoldmods.Count)
                {
                    return playergoldmods;
                }
                picklist.AddRange(playergoldmods);
                break;
            default:
                Debug.LogWarning("Improper rarity used in function call. Defaulting to white rarity for mods");
                if (amount > playerwhitemods.Count)
                {
                    return playerwhitemods;
                }
                picklist.AddRange(playerwhitemods);
                break;
        }

        // Once the the cases have been determined, random (non-duplicate) mods are picked from what the player has
        for (int i = 0; i < amount; i++)
        {
            int num = (int)Random.Range(0, picklist.Count);
            returnlist.Add(picklist[num]);
            picklist.Remove(picklist[num]);
        }
        return returnlist;
    }

    /**
     * Returns a number of random (non-duplicate) mods from the players (does not care about the rarity)
     */
    public List<SkillObject> GetRandomModsFromPlayer(int amount)
    {
        if (amount > playermods.Count)
        {
            return playermods;
        }

        List<SkillObject> returnlist = new List<SkillObject>();
        List<SkillObject> picklist = new List<SkillObject>();
        picklist.AddRange(playermods);

        // Random (non-duplicate) mods are picked from what the player has
        for (int i = 0; i < amount; i++)
        {
            int num = (int)Random.Range(0, picklist.Count);
            returnlist.Add(picklist[num]);
            picklist.Remove(picklist[num]);
        }
        return returnlist;
    }

    public List<SkillObject> GetRandomWhitesFromPlayer(int amount)
    {
        return GetRandomModsFromPlayer(amount, 1);
    }

    public List<SkillObject> GetRandomGreensFromPlayer(int amount)
    {
        return GetRandomModsFromPlayer(amount, 2);
    }

    public List<SkillObject> GetRandomGoldsFromPlayer(int amount)
    {
        return GetRandomModsFromPlayer(amount, 3);
    }
    public GameObject SpawnAbility(Vector3 position, string name)
    {
        SkillObject so = (SkillObject)Resources.Load("Skills/Abilities/" + name);
        GameObject emit = (GameObject)GameObject.Instantiate(Resources.Load("Pickups/AbilityPickup"), position, Quaternion.identity);
        emit.GetComponent<SkillPickup>().skill = so;
        emit.GetComponent<SimpleTooltip>().infoLeft = so.name + "\n";
        emit.GetComponent<SimpleTooltip>().infoLeft += so.Description;
        return emit;
    }

    public GameObject SpawnMod(Vector3 position, string name)
    {
        SkillObject so = Resources.Load<SkillObject>("Skills/Modifiers/" + name);
        GameObject modObj = (GameObject)GameObject.Instantiate(Resources.Load("Pickups/ModPickup"), position, Quaternion.identity);
        modObj.GetComponent<SkillPickup>().skill = so;
        modObj.GetComponent<SimpleTooltip>().infoLeft = so.name + "\n";
        modObj.GetComponent<SimpleTooltip>().infoLeft += so.Description;
        //change the color of the mod to the color based on rarity
        VFXManager.instance.ChangeColor(modObj, GetColor(so));
        return modObj;
    }
    public void clearSkills()
    {
        List<SkillObject> skills = GetSkillObjects();
        for(int i = 0; i < skills.Count; i ++)
        {
            RemoveSkill(skills[i]);
        }
    }
    public int GetSkillStack(string name)
    {
        if (Skills.ContainsKey(name))
        {
            int num = Skills[name];
            //Debug.Log(skillobjects.Count);
            return num;
        }
        return 0;
    }

    public int GetAmount()
    {
        return skillamount;
    }

    public int GetModAmount()
    {
        return modamount;
    }
    public int GetUniqueModAmount()
    {
        return playermods.Count;
    }

    public int GetAbilityAmount()
    {
        return abilityamount;
    }

    public void SetAbility1(string name)
    {
        ability1 = name;
        updated = true;
    }

    /**
     * Returns the Skillobject for the ability in slot 1, if it doesn't exists, it reutrns null
     */
    public SkillObject GetAbility1()
    {
        foreach (SkillObject so in skillobjects)
        {
            if (so.name == ability1)
            {
                return so;
            }
        }
        return null;
    }

    public void SetAbility2(string name)
    {
        ability2 = name;
        updated = true;
    }

    /**
     * Returns the Skillobject for the ability in slot 2, if it doesn't exists, it reutrns null
     */
    public SkillObject GetAbility2()
    {
        foreach (SkillObject so in skillobjects)
        {
            if (so.name == ability2)
            {
                return so;
            }
        }
        return null;
    }

    /**
     * Swaps the abilties.
     */
    public void SwapCurrentAbilities()
    {
        string temp = ability2;
        SetAbility2(ability1);
        SetAbility1(temp);
        if (player.GetComponent<AbilityCasting>())
        {
            player.GetComponent<AbilityCasting>().SwapAbilityCooldowns();
            player.GetComponent<AbilityCasting>().CastUI(0);
            player.GetComponent<AbilityCasting>().CastUI(1);
        }
        SetUpdated(true);
    }

    public void SetUpdated(bool boolean)
    {
        updated = boolean;
    }
    //returns a skillobject mod when given a string name of a mod
    public SkillObject GetSkillFromString(string name)
    {
        Object skill = Resources.Load<SkillObject>("Skills/Modifiers/" + name);
        if (skill == null)
            skill = Resources.Load<SkillObject>("Skills/Abilities/" + name);
        if (skill == null)
            return null;
        return (SkillObject)skill;
    }
    public Color GetColor(SkillObject sk)
    {
        Color color;
        switch (sk.Rarity)
        {
            case 2:
                color = new Color(0, .93f, 1f);
                break;
            case 3:
                color = new Color(1, 0.83f, 0);
                break;
            default:
                color = Color.white;
                break;
        }
        return color;
    }
    public string GetRarityString(SkillObject sk)
    {
        string rarity;
        switch (sk.Rarity)
        {
            case 2:
                rarity = "Rare";
                break;
            case 3:
                rarity = "Legendary";
                break;
            default:
                rarity = "Common";
                break;
        }
        return rarity;
    }

    public bool IsActive()
    {
        return true;
    }
    public void SetActive()
    {

    }
    public int GetModSlotLimit()
    {
        return uniqueModLimit;
    }
    public void SetModLimit(float newLimit)
    {
        newLimit = Mathf.Clamp(newLimit, 7, ClampModLimit);
        uniqueModLimit = (int)newLimit;
    }
    //This function returns a multiplier value, based on how many of that skill the player has
    // pass in the name of modifier, and how much additional stacks past 1 should be multiplied by, (mainly used for explosion radius increases right now)
    public float GetSkillStackAsMultiplier(string skill, float multiPerStack)
    {
        int stacks = GetSkillStack(skill);
        float multi = 1;
        if (stacks > 1)
            multi = 1 + (multiPerStack - 1) * (stacks - 1);
        return multi;
    }
    public bool GetUpdated()
    {
        return updated;
    }

    public SkillObject GetAddedSkill()
    {
        return AddedSkill;
    }
    public List<SkillObject> GetAllAbilities()
    {
        return abilities;
    }
    public void SetAddedSkill(SkillObject so)
    {
        AddedSkill = so;
    }
}
