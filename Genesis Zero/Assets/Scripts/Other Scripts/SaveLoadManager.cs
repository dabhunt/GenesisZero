using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    public string filename;

    private string path;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        path = "/" + filename + ".dat";
    }

    /* Update and save data that we wanna save
     * in a hidden away binary file.
     */
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + path);
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        SaveData data = SaveData(player);
        //Serialize and write to the file
        bf.Serialize(file, data);
        file.Close();
    }

    /* Load saved data from the hidden
     * binary file.
     */
    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + path, FileMode.Open);            
            SaveData data = (SaveData) bf.Deserialize(file);
            file.Close();
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            LoadData(data, player);
        }
    }

    /* Load saved data from file and apply
     * it to the current game.
     */
    private void LoadData(SaveData data, Player player)
    {
        player.GetHealth().SetValue(data.health);
        player.GetDamage().SetValue(data.damage);
        player.GetSpeed().SetValue(data.speed);
        player.GetAttackSpeed().SetValue(data.attackSpeed);
        player.GetFlatDamageReduction().SetValue(data.flatDamageReduction);
        player.GetDamageReduction().SetValue(data.damageReduction);
        player.GetDodgeChance().SetValue(data.dodgeChance);
        player.GetCritChance().SetValue(data.critChance);
        player.GetRange().SetValue(data.range);
        player.GetShield().SetValue(data.shield);
        player.GetWeight().SetValue(data.weight);
    }

    /* Returns a SaveData object with the most updated
     * values from the current game state.
     */
    private SaveData SaveData(Player player)
    {
        SaveData data = new SaveData();
        //data.seed = ;
        data.playerPosition[0] = player.gameObject.transform.position.x;
        data.playerPosition[1] = player.gameObject.transform.position.y;
        data.playerPosition[2] = player.gameObject.transform.position.z;
        data.health = player.GetHealth().GetValue();
        data.damage = player.GetDamage().GetValue();
        data.speed = player.GetSpeed().GetValue();
        data.attackSpeed = player.GetAttackSpeed().GetValue();
        data.flatDamageReduction = player.GetFlatDamageReduction().GetValue();
        data.damageReduction = player.GetDamageReduction().GetValue();
        data.dodgeChance = player.GetDodgeChance().GetValue();
        data.critChance = player.GetCritChance().GetValue();
        data.range = player.GetRange().GetValue();
        data.shield = player.GetShield().GetValue();
        data.weight = player.GetWeight().GetValue();
        //data.deathDuration = player.Stats.deathDuration;
        
        return data;
    }
}

[Serializable]
public class SaveData
{
    //private Status invunerable, stunned, burning, slowed, stunimmune;
    public float seed; //PCG seed
    public float[] playerPosition;
    //player stats
    public float health, damage, speed, attackSpeed, flatDamageReduction, damageReduction, dodgeChance, critChance, critdamage, range, shield, weight;
    public bool invunerable, stunned, burning, slowed, stunimmune;
    public float burntime, burndamage, burntick;
    private float slowtime, knockbackforce;
    public SaveData()
    {
        playerPosition = new float[3];
        invunerable = false;
        stunned = false;
        burning = false;
        slowed = false;
        stunimmune = false;
    }
}
