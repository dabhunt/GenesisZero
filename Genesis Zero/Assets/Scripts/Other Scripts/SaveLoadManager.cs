using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    private static string pFileName = "pData";
    private static string mFileName = "mData";
    private static string sFileName = "sData";
    private static string eFilename = "eData";
    private string pPath;
    private string mPath;
    private string sPath;
    private string ePath;
    private static bool newgame = true;
    private static bool endless = false;
    public bool newGame
    { 
        get {return newgame;} 
        set {newgame = value;}
    }

    public bool endLess
    {
        get { return endless;}
        set {endless = value;}
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
        pPath = Application.persistentDataPath + "/" + pFileName + ".dat";
        mPath = Application.persistentDataPath + "/" + mFileName + ".dat";
        ePath = Application.persistentDataPath + "/" + eFilename + ".dat";
        sPath = Application.persistentDataPath + "/" + sFileName + ".dat";
    }

    private void Start()
    {

        if(SaveExists() && !CorrectVersion())
            DeleteSaveFiles();
    }

    /* Update and save relevant data that we want to save
     * in a hidden away binary file.
     */
    public void SaveGame()
    {
        Debug.Log("Saving Game");
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            PlayerData playerData = GetPlayerData(player);
            FileStream pFile = File.Create(pPath);
            FileStream mFile = File.Create(mPath);
            MapData mapData = GetMapData();
            if (endless)
            {
                FileStream eFile = File.Create(ePath);
                EndlessData endlessData = GetEndlessData();
                foreach (var d in endlessData.data)
                    Debug.Log(d);
                bf.Serialize(eFile, endlessData);
                eFile.Close();
            }
            //Serialize and write to the file
            bf.Serialize(pFile, playerData);
            pFile.Close();
            bf.Serialize(mFile, mapData);
            mFile.Close();
        }
        catch(NullReferenceException)
        {
            Debug.Log("Save unsucessful (player component doesn't exist)");
            return;
        }
    }

    public void SaveSettings(SettingsData data)
    {
        Debug.Log("Saving User Preferences");
        //sPath = Application.persistentDataPath + "/" + sFileName + ".dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream sFile = File.Create(sPath);
        bf.Serialize(sFile, data);
        sFile.Close();
    }

    public SettingsData LoadSettings()
    {
        //sPath = Application.persistentDataPath + "/" + sFileName + ".dat";
        if (File.Exists(sPath))
        {
            Debug.Log("Loading User Preferences");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(sPath, FileMode.Open);
            SettingsData data = bf.Deserialize(file) as SettingsData;
            file.Close();
            return data;
        }
        else
        {
            Debug.LogError("Settings file not found");
            return null;
        }
    }

    /* Load saved player data from the hidden
     * binary file.
     */
    public PlayerData LoadPlayerData()
    {
        //pPath = Application.persistentDataPath + "/" + pFileName + ".dat";
        if (File.Exists(pPath))
        {
            Debug.Log("Loading Character Data");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(pPath, FileMode.Open);            
            PlayerData data = bf.Deserialize(file) as PlayerData;
            file.Close();
            return data;
        }
        else
        {
            Debug.LogError("Player save file doesn't exist");
            return null;
        }
    }

    /* Load saved map data from the hidden
     * binary file.
     */
    public MapData LoadMapData()
    {
        //mPath = Application.persistentDataPath + "/" + mFileName + ".dat";
        if (File.Exists(mPath))
        {
            Debug.Log("Loading Map Data");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(mPath, FileMode.Open);
            MapData data = bf.Deserialize(file) as MapData;
            file.Close();
            return data;
        }
        else
        {
            Debug.LogError("Map data file doesn't exist");
            return null;
        }
    }

    public EndlessData LoadEndlessData()
    {
        //ePath = Application.persistentDataPath + "/" + eFilename + ".dat";
        if (File.Exists(ePath))
        {
            Debug.Log("Loading Endless data");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(ePath, FileMode.Open);
            EndlessData data = bf.Deserialize(file) as EndlessData;
            file.Close();
            return data;
        }
        else
        {
            Debug.LogError("Endless data file doesn't exist");
            return null;
        }
    }
    /* Applying saved player data to current game session.
     */
    public void ApplyPlayerData(PlayerData data)
    {
        Debug.Log("Applying Player data");
        Player player = Player.instance;
        SkillManager sM = player.GetSkillManager();
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
        player.SetEssence(data.essence);

        SkillObject sObject;
        for (int i = 0; i < data.skillList.Count; i++)
        {
            sObject = sM.GetSkillFromString(data.skillList[i]);
            sM.AddSkill(sObject);
            sM.SetCountByName(data.skillList[i], data.skillStacks[i]);
        }
        //print("x value" + data.playerPosition[0]);
        Player.instance.gameObject.transform.position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
        //Debug.Log("Player Data, Applied");
        //GameObject.FindGameObjectWithTag("CMcam").SetActive(true);
        //Debug.Log("Cam SetActive!");
    }
    /* Grabs map data from current session for saving.
     */
    private MapData GetMapData()
    {
        MapData data = new MapData();
        data.seed = TileManager.instance.GetSeed();
        Debug.Log("GetDATA " + data.seed);
        data.version = Application.version;
        return data;
    }

    private EndlessData GetEndlessData()
    {
        EndlessData eData = new EndlessData();
        eData.data = MayhemTimer.instance.GetData();
        return eData;
    }
    /* Returns a SaveData object with the most updated
     * values from the current game state.
     */
    private PlayerData GetPlayerData(Player player)
    {
        PlayerData data = new PlayerData();
        SkillManager sM = player.GetSkillManager();
        List<SkillObject> sList = sM.GetSkillObjects();
        Dictionary<string, int> dialougeDict = DialogueManager.instance.GetDialougePlayedDict();
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

        data.essence = player.GetEssenceAmount();

        data.dialougePlayedKeys = new List<string>(dialougeDict.Keys);
        data.dialougePlayedCounts = new List<int>();
        foreach (var key in data.dialougePlayedKeys)
        {
            data.dialougePlayedCounts.Add(dialougeDict[key]);
        }
        data.skillList = new List<string>();
        data.skillStacks = new List<int>();
        for (int i = 0; i < sList.Count; i++)
        {
            data.skillList.Add(sList[i].name);
            data.skillStacks.Add(sM.GetSkillStack(sList[i].name));
        }
        return data;
    }
    
    //Checks if the save files exists
    public bool SaveExists()
    {
        //pPath = Application.persistentDataPath + "/" + pFileName + ".dat";
        Debug.Log("Checking for Player data at: " + (pPath));
        //mPath = Application.persistentDataPath + "/" + mFileName + ".dat";    
        Debug.Log("Checking for Map data at: " + (mPath));
        if (File.Exists(pPath) && File.Exists(mPath))
        {
            Debug.Log("Save Exists");
            return true;
        }
        else
        {
            Debug.Log("Save Doesn't Exist");
            return false;
        }
    }

    public bool SettingsSaveExists()
    {
        //sPath = Application.persistentDataPath + "/" + sFileName + ".dat";
        Debug.Log("Checking for SettingsFile at: " + (sPath));
        if (File.Exists(sPath))
        {
            Debug.Log("SettingsFile exist");
            return true;
        }
        else
        {
            Debug.Log("SettingsFile doesn't exist");
            return false;
        }
    }

    public bool EndlessSaveExists()
    {
        //ePath = Application.persistentDataPath + "/" + eFilename + ".dat";
        Debug.Log("Checking for Endless save File at: " + (ePath));
        if (File.Exists(ePath))
        {
            Debug.Log("Endless save File exist");
            return true;
        }
        else
        {
            Debug.Log("Endless save File doesn't exist");
            return false;
        }
    }

    public void DeleteSaveFiles()
    {
        if (SaveExists())
        {
            Debug.Log("Deleting saves");
            File.Delete(pPath);
            File.Delete(mPath);
            if (EndlessSaveExists())
            {
                Debug.Log("Deleting Endless Save");
                File.Delete(ePath);
            }
        }
    }

    //return true if save files are not correct version
    public bool CorrectVersion()
    {
        MapData data = LoadMapData();
        if (data.version == null)
            return false;
        return (data.version == Application.version);
    }
}

[Serializable]
public class PlayerData
{
    public float[] playerPosition;
    //player stats
    public float health, damage, speed, attackSpeed, flatDamageReduction, damageReduction, dodgeChance, critChance, critdamage, range, shield, weight;
    public float essence;
    public Status invunerable, stunned, burning, slowed, stunimmune;
    public List<string> skillList;
    public List<int> skillStacks;
    public List<string> dialougePlayedKeys;
    public List<int> dialougePlayedCounts;
    public PlayerData()
    {
        playerPosition = new float[3];
    }
}
[Serializable]
public class MapData
{
    public int seed;
    public string version;
}

[Serializable]
public class SettingsData
{
    public bool muteMaster;
    public bool muteSFX;
    public bool muteMusic;
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
    public int resIndex;
    public bool fullScreen;
}

[Serializable]
public class EndlessData
{
    public float[] data;

    public EndlessData()
    {
        data = new float[4];
    }
}