using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    [Tooltip("Name of file for player data")]
    public string pFileName = "pData";
    [Tooltip("Name of file for map data")]
    public string mFileName = "mData";

    private string pPath;
    private string mPath;
    private static bool newgame = true;
    public bool newGame
    { 
        get {return newgame;} 
        set {newgame = value;}
    }
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
        pPath = "/" + pFileName + ".dat";
        mPath = "/" + mFileName + ".dat";
    }

    /* Update and save data that we wanna save
     * in a hidden away binary file.
     */
    public void SaveGame()
    {
        Debug.Log("Saving Game");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + pPath);
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        PlayerData playerData = GetPlayerData(player);
        //Serialize and write to the file
        bf.Serialize(file, playerData);
        file.Close();
    }

    /* Load saved player data from the hidden
     * binary file.
     */
    public PlayerData LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + pPath))
        {
            Debug.Log("Loading Last Save");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + pPath, FileMode.Open);            
            PlayerData playerData = bf.Deserialize(file) as PlayerData;
            file.Close();
            return playerData;
        }
        else
        {
            Debug.LogError("Save Files not found");
            return null;
        }
    }

    /* Load saved map data from the hidden
     * binary file.
     */
    public MapData LoadMapData()
    {
        return null;
    }

    /* Applying saved player data to current game session.
     */
    public void ApplyPlayerData(PlayerData data, GameObject playerObj)
    {
        Player player = playerObj.GetComponent<Player>();
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
        playerObj.transform.position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
    }

    /* Applying saved player data to current game session.
     */
    public void ApplyMapData(MapData data)
    {

    }
    /* Returns a SaveData object with the most updated
     * values from the current game state.
     */
    private PlayerData GetPlayerData(Player player)
    {
        PlayerData data = new PlayerData();
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

        data.invunerable = player.GetInvunerableStatus();
        data.stunned = player.GetStunnedStatus();
        data.burning = player.GetBurningStatus();
        data.slowed = player.GetSlowedStatus();
        data.stunimmune = player.GetStunImmuneStatus();

        return data;
    }

    private MapData GetMapData()
    {
        return null;
    }

    public bool SaveExists()
    {
        if (File.Exists(Application.persistentDataPath + pPath) /*&& File.Exists(Application.persistentDataPath + mPath)*/)
            return true;
        else
            return false;
    }
}

[Serializable]
public class PlayerData
{
    public float[] playerPosition;
    //player stats
    public float health, damage, speed, attackSpeed, flatDamageReduction, damageReduction, dodgeChance, critChance, critdamage, range, shield, weight;
    public Status invunerable, stunned, burning, slowed, stunimmune;
    public PlayerData()
    {
        playerPosition = new float[3];
    }
}

public class MapData
{
    public float seed;
}
