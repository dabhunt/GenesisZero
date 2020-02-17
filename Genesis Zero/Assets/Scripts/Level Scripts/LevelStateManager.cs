using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/**
 * Justin Couch
 * Kenny Doan (referenced from VFXManager)
 * LevelStateManager controls the state of the current level including entering/exiting the level
 * and associated events/sequences.
 */
public class LevelStateManager : MonoBehaviour
{
    public static LevelStateManager instance;

    public UnityEvent levelEnterEvent;
    public UnityEvent levelExitEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*
     * Invokes the level exit event when a level is left
     */
    public static void GoToLevel(string nextLevel)
    {
        if (instance != null)
        {
            instance.levelExitEvent.Invoke();
        }
        SceneManager.LoadScene(nextLevel);
    }

    /*
     * Invokes the level enter event when a level is loaded
     */
    public void OnLevelWasLoaded(int level)
    {
        if (instance != null)
        {
            instance.levelEnterEvent.Invoke();
        }
    }
}
