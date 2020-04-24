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
    public static float Difficulty = 2.0f;
    public static float MaxDifficulty = 4.0f;
    public static float normalizedDifficulty { get { return Difficulty / Mathf.Max(0.01f, MaxDifficulty); } } // Range form 0 to 1 indicating current difficulty factor

    private void Start()
    {
        AllEnemies.Clear();
    }
}

// Class for properties that are multiplied by certain amounts based on the difficulty
[System.Serializable]
public class DifficultyMultiplier
{
    public float MinFactor = 0.5f;
    public float MaxFactor = 1.5f;

    public float GetFactor()
    {
        return Mathf.Lerp(MinFactor, MaxFactor, EnemyManager.normalizedDifficulty);
    }
}