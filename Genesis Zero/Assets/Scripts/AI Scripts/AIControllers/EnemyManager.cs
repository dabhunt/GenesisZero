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
    public static float MaxDifficulty = 100.0f;
    public static float NormalizedDifficulty { get { return Difficulty / Mathf.Max(0.01f, MaxDifficulty); } } // Range form 0 to 1 indicating current difficulty factor

    public static EnemyManager instance = null; // Singleton instance.

    // Initialize the singleton instance.
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
    // Increase enemy difficulty, based on the current difficulty multiplied by the number passed in.
    // Ex: 1.3 passed in makes enemies 30% more difficult
    public void ModifyDifficultyMulti(float multiplier)
    {
        float newDificulty = Difficulty + multiplier; //no longer multiplier because that doesn't scale well for most attributes
        Difficulty = Mathf.Clamp(newDificulty, 1f, MaxDifficulty);
        int i = 0;
        foreach (AIController enemy in AllEnemies)
        {

            float oldHP = enemy.GetHealth().GetValue();
            enemy.SetMaxHealth(oldHP * 1.2f);//health can scale exponentially
            if (i == 0)
                print("health of enemy is " + enemy.GetHealth().GetValue());
            i++;
        }
        print("new difficulty: " + Difficulty);
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