using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void QuitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void MenuButton()
    {
        //statemanager.
    }
    public void RestartButton()
    {
        StateManager.instance.restart.RestartScene();
    }
    public void ContinueButton()
    {
        StateManager.instance.UnpauseGame();
    }
}
