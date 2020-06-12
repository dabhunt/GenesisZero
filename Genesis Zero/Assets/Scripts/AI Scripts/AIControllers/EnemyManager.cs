using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * EnemyManager is used for keeping track of global information related to enemies
 */
public class EnemyManager : MonoBehaviour
{
    public static List<AIController> AllEnemies = new List<AIController>();
    public static float Difficulty = .75f;
    private float HealthIncreasePerDifficulty = 1.5f; //50%
    public static float MaxDifficulty = 100.0f;
    public static float NormalizedDifficulty { get { return Difficulty / Mathf.Max(0.01f, MaxDifficulty); } } // Range form 0 to 1 indicating current difficulty factor

    public static EnemyManager instance = null; // Singleton instance.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        AllEnemies.Clear();
    }
    public void DestroyAll()
    {
        foreach (AIController enemy in AllEnemies)
        {
            enemy.GetComponent<SpawnOnDestroy>().quitting = true;
            Destroy(enemy.gameObject);
        }
    }
    public GameObject SpawnEnemy(Vector2 spawn)
    {
        GameObject newEnemy = Instantiate(TileManager.instance.enemyPrefabs[Random.Range(0, TileManager.instance.enemyPrefabs.Length-2)], spawn, Quaternion.identity) as GameObject;
        //AIController enemy = newEnemy.GetComponent<AIController>();
       // if (enemy != null && enemy.GetHealth() != null) 
       // { 
            //enemy.GetHealth().SetMaxValue(enemy.GetHealth().GetValue() * GetHealthMultiplier);
            //print("Enemy Health:" + enemy.GetHealth().GetValue());
            //print("health multiplier: " + GetHealthMultiplier);
        //}
        return newEnemy;
    }
    // Increase enemy difficulty, based on the current difficulty multiplied by the number passed in.
    // Ex: 1.3 passed in makes enemies 30% more difficult
    public void MultiplyDifficulty(float multiplier)
    {
        Difficulty *= multiplier;
        SetDifficulty(Difficulty);
    }
    public void AddDifficulty(float moredif) 
    {
        SetDifficulty(Difficulty + moredif);
    }
    public float GetHealthMultiplier() 
    {
       return Mathf.Pow(HealthIncreasePerDifficulty, Difficulty);
    }
    public void SetDifficulty(float newDifficulty)
    {
        Difficulty = newDifficulty; //no longer multiplier because that doesn't scale well for most attributes
        Difficulty = Mathf.Clamp(Difficulty, 0, MaxDifficulty);
        GetHealthMultiplier();
        print("health multiplier: "+ GetHealthMultiplier());
        //HealthMultiplier = 1 + Difficulty / 2; //healthmultiplier is used for new enemies being spawned, but ones that are already in game are scaled based on % or previous health
        int i = 0;
        if (TileManager.instance.MayhemMode)
        {
            foreach (AIController enemy in AllEnemies)
            {
                float oldHP = enemy.GetHealth().GetValue();
                enemy.SetMaxHealth(oldHP* HealthIncreasePerDifficulty);//health can scale exponentially
                print("health of enemy is " + enemy.GetHealth().GetValue());
                i++;
            }
        }
    }
    public void ResetDifficulty(float newdif)
    {
        Difficulty = newdif;
        GetHealthMultiplier();
    }
    public void SetHealthMultiPerDifficulty(float newMulti) 
    {
        HealthIncreasePerDifficulty = newMulti;
        GetHealthMultiplier();
    }
}

// Class for properties that are multiplied by certain amounts based on the difficulty
[System.Serializable]
public class DifficultyMultiplier
{
    public float MinFactor = 0.5f;
    public float MaxFactor = 1.5f;
    public bool Invert = false;

    public float GetFactor()
    {
        return Invert ? Mathf.Lerp(MaxFactor, MinFactor, EnemyManager.NormalizedDifficulty) : Mathf.Lerp(MinFactor, MaxFactor, EnemyManager.NormalizedDifficulty);
    }
}