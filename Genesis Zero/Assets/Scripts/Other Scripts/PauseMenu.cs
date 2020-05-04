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
        StateManager.instance.LoadMenu();
    }
    public void RestartButton()
    {
        StateManager.instance.restart.RestartScene();
    }
    public void ContinueButton()
    {
        StateManager.instance.UnpauseGame();
    }

    public void OptionButton()
    {
        StateManager.instance.ToggleOptionsMenu(true);
    }

    public void OptionBackButton()
    {
        StateManager.instance.ToggleOptionsMenu(false);
    }
}
