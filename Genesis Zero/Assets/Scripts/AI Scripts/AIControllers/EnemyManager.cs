using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static List<AIController> AllEnemies = new List<AIController>();

    private void Start()
    {
        AllEnemies.Clear();
    }
}
