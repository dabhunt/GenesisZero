using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    
    //This pauses game
    public void PauseGame()
    {
        //Pauses Game
        Debug.Log("Pausing Game");
        Time.timeScale = 0;
    }

    //This unpauses game
    public void UnpauseGame()
    {
        //UnPauses Game
        Debug.Log("UnPausing Game");
        Time.timeScale = 1;
    }
}
