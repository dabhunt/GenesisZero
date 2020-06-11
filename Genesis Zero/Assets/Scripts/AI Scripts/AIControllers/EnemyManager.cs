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
    public static float HealthMultiplier = 1;
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
        GameObject newEnemy = Instantiate(TileManager.instance.enemyPrefabs[Random.Range(0, TileManager.instance.enemyPrefabs.Length)], spawn, Quaternion.identity) as GameObject;
        AIController enemyHp = newEnemy.GetComponent<AIController>();
        if (enemyHp != null && enemyHp.GetHealth() != null)
            enemyHp.GetHealth().SetMaxValue(enemyHp.GetHealth().GetValue() *HealthMultiplier);
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
    public void SetDifficulty(float newDifficulty)
    {
        Difficulty = newDifficulty; //no longer multiplier because that doesn't scale well for most attributes
        Difficulty = Mathf.Clamp(Difficulty, 1f, MaxDifficulty);
        HealthMultiplier = 1 + Difficulty / 2;
        int i = 0;
        foreach (AIController enemy in AllEnemies)
        {
            float oldHP = enemy.GetHealth().GetValue();
            enemy.SetMaxHealth(HealthMultiplier);//health can scale exponentially
            if (i == 0)
                print("health of enemy is " + enemy.GetHealth().GetValue());
            i++;
        }
    }
    public void ResetDifficulty(float newdif)
    {
        HealthMultiplier = 1;
        Difficulty = newdif;
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