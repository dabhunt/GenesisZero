using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void QuitButton()
    {
        //Save Game
        SaveLoadManager.instance.SaveGame();
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void MenuButton()
    {

        //StateManager.instance.LoadMenu();
        //Save Game
        StateManager.instance.gameObject.GetComponent<Restart>().exitingScene = true;
        var sods = Resources.FindObjectsOfTypeAll<SpawnOnDestroy>();
        foreach (SpawnOnDestroy s in sods)
        {
            s.quitting = true;
        }
        SaveLoadManager.instance.SaveGame();
        //Player.instance.GetHealth().SetValue(0);
        StateManager.instance.LoadMenu();
    }
    public void RestartButton()
    {
        SaveLoadManager.instance.newGame = true;
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
