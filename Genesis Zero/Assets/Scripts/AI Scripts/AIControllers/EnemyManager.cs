﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * EnemyManager is used for keeping track of global information related to enemies
 */
public class EnemyManager : MonoBehaviour
{
    public static List<AIController> AllEnemies = new List<AIController>();
    public static float Difficulty = 1.0f;

    private void Start()
    {
        AllEnemies.Clear();
    }
}
