using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void QuitButton()
    {
        Application.Quit();
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
