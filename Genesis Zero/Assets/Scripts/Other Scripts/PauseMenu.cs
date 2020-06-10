using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public void QuitButton()
    {
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
        for (int i = 0; i < sods.Length; i++)
        {
            sods[i].quitting = true;
            //Destroy(sods[i]);
        }
        SaveLoadManager.instance.SaveGame();
        //Player.instance.GetHealth().SetValue(0);
        StateManager.instance.LoadMenu();
        Invoke("LoadMainMenu", .3f);
    }
    public void LoadMainMenu()
    {
        print("this runs");
    }
    public void RestartButton()
    {
        EnemyManager.instance.ResetDifficulty(.75f);
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
