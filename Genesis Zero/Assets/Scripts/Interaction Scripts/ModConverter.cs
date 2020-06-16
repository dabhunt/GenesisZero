﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModConverter : MonoBehaviour
{
    private List<SkillObject> modList;
    private SkillManager sk;
    private GameObject player;
    public bool isActive = true;
    /* This script controls what happens when the player interacts with the mod converter, and the math behind what mod you get in return
     * Essentially, you cannot get back a modifier of lower quality than you put in, and putting in more modifiers increases the likelyhood
     * of a higher tier mod coming out
     * ScrapConverter
     */
    private void Start()
    {
        modList = new List<SkillObject>();
        player = Player.instance.gameObject;
        sk = Player.instance.GetSkillManager();
        gameObject.AddComponent<InactiveFlag>();
    }
    //When you press the Interact button, the Merchant/Machine determines what random Mod to give you
    public void Interact()
    {
        if (!isActive || player == null)
            return;
        if (GetComponent<InteractPopup>().visible == true)
        {
            if (modList.Count >= 1)
            {
                sk.SpawnMod(player.transform.position, GetNewMod().name);
                //marks this object as no longer able to be interacted with
                gameObject.AddComponent<InactiveFlag>();
                this.transform.Find("TopSection").GetComponent<TweenTo>().Move();
            }
        }
    }
    //returns a new mod based on what mod's are being held in the list
    public SkillObject GetNewMod()
    {
        float value = 0;
        for (int i = 0; i < modList.Count; i++)
        {
            value += EvaluateMod(i);
        }
        //the amount of mods you put in increases the guaranteed lowest rarity you can receive, and increases how likely you are to get a better mod
        List<SkillObject> mod = new List<SkillObject>();
        if (value >= 8)
            mod = sk.GetRandomGolds(1);
        else if (value >= 3)
            mod = sk.GetRandomGreens(1);
        SkillObject newMod = sk.GetRandomModByChance();
        //start at 1, so that this only runs at values > 1
        for (int i = 1; i < value; i++)
        {
            SkillObject oldMod = newMod;
            //for each 'value' roll a new mod by chance, if that mod exceeds the value of the stored mod, replace it
            newMod = sk.GetRandomModByChance();
            if (newMod.Rarity > oldMod.Rarity) 
                newMod = oldMod;
        }
        //randomly rolls a mod by chance, giving the player the chance to potentially receive a better mod
        if (value >= 3 && mod[0].Rarity > newMod.Rarity)
            newMod = mod[0]; //if the mod is better replace it, otherwise it stays the same
        newMod = GetFreshMod(newMod.Rarity); //guarantees the mod you get back was not one of the mods you put in
        modList.Clear();
        //deactivate Scrap Converter
        isActive = false;
        InteractPopup iPop = GetComponent<InteractPopup>();
        iPop.DestroyPopUp();
        Destroy(iPop);
        this.transform.Find("Light").gameObject.SetActive(false);
        //play breaking down animation?
        return newMod;
    }
    //makes sure you don't get the same modifier that you put in
    private SkillObject GetFreshMod(int rarity)
    {
        SkillObject newSkill = sk.GetRandomMods(1, rarity)[0];
        int failSafe = 0;
        while (ListContainsMod(newSkill)) //if it tries 200 times, then there probably is no mod of that rarity that you didn't put in, so we exit
        {
            newSkill = sk.GetRandomMods(1, rarity)[0]; //keep getting random mods of that rarity until ListContainsMod is false
            failSafe++;
            if (failSafe > 200)
                break;
        }
        return newSkill;
    }
    private bool ListContainsMod(SkillObject skill)
    {
        foreach (SkillObject hasMod in modList)
        {
            if (skill.name == hasMod.name) //if the passed in skill is also inside the modlist, return true
                return true;
        }
        return false;
    }
    public void AddMod(SkillObject skill)
    {
        Destroy(GetComponent<InactiveFlag>());
        print("adding mod to converter");
        modList.Add(skill);
        string s = "";
        if (modList.Count > 1)
            s = "s";
        if (GetComponent<InteractPopup>() != null)
            GetComponent<InteractPopup>().SetText("Press [F] to convert [ "+modList.Count+" ] mod"+s+" into 1 new Mod of the same or better rarity");
    }
    private float EvaluateMod(int i)
    {
        //this value is the value of the 'common' mod
        float value = 1;
        switch (modList[i].Rarity)
        {
            case 2: //2 is rare
                value = 3;
                break;
            case 3: //3 is legendary
                value = 8;
                break;
            default: //defaults to 1
                break;
        }
        return value;
    }
}

