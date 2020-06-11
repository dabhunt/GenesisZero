using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public Canvas canvas;
    public string mainSceneName;
    public string endlessSceneName;
    private string sceneName;
    private AsyncOperation operation;
    private Slider loadBar;
    private Text loadPercentage;

    private GameObject mainMenuScreen;
    private GameObject menuButtons;
    private GameObject optionsScreen;
    private GameObject loadingScreen;

    public void Start()
    {
        //Caching some canvas groups
        mainMenuScreen = canvas.transform.Find("MainMenuScreen").gameObject;
        loadingScreen = canvas.transform.Find("LoadingScreen").gameObject;
        optionsScreen = canvas.transform.Find("OptionsScreen").gameObject;
        loadBar = loadingScreen.transform.Find("LoadBar").gameObject.GetComponent<Slider>();
        loadPercentage = loadBar.transform.Find("LoadPercentage").gameObject.GetComponent<Text>();
        menuButtons = mainMenuScreen.transform.Find("Buttons").gameObject;

        GameObject conbttn = menuButtons.transform.Find("Continue").gameObject;
        if (SaveLoadManager.instance.SaveExists())
        {
            conbttn.SetActive(true);
            //conbttn.SetActive(false);
        }
        else
        {
            conbttn.SetActive(false);
        }
        Cursor.visible = true;
    }

    public void ContinueButton()
    {
        mainMenuScreen.SetActive(false);
        loadingScreen.SetActive(true);
        if (SaveLoadManager.instance.EndlessSaveExists())
        {
            SaveLoadManager.instance.endLess = true;
            mainMenuScreen.SetActive(false);
            loadingScreen.SetActive(true);
            sceneName = endlessSceneName;
            LoadScene(false);
            return;
        }
        sceneName = mainSceneName;
        LoadScene(false);
    }

    //Onclick Event for Start Button
    public void StartButton()
    {
        //hiding MainMenuScreen.
        mainMenuScreen.SetActive(false);
        loadingScreen.SetActive(true);
        SaveLoadManager.instance.endLess = false;
        //Deletes old save files
        SaveLoadManager.instance.DeleteSaveFiles();
        sceneName = mainSceneName;
        LoadScene(true);
    }
    //Onclick Event for Options Button
    public void OptionsButton()
    {
        mainMenuScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }
    public void MayhemButton()
    {
        mainMenuScreen.SetActive(false);
        loadingScreen.SetActive(true);
        SaveLoadManager.instance.endLess = true;
        SaveLoadManager.instance.DeleteSaveFiles();
        sceneName = endlessSceneName;
        LoadScene(true);
    }
    //Onclick event for Quit Button
    public void QuitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    private void LoadScene(bool newGame)
    {
        SaveLoadManager.instance.newGame = newGame;
        StartCoroutine(LoadSceneCoroutine());
    }
    //Load main scene and display progress
    IEnumerator LoadSceneCoroutine()
    {
        float timeElapsed = 0f;
        float lerpTarget;
        float lerpTime;
        
        //operation is 0.9f if the scene is ready for display
        operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        canvas.transform.Find("LoadingScreen").gameObject.SetActive(true);
        
        //loop to update the progress bar
        while (loadBar.value < 1f)
        {
            //slow progress and pause at 90% while the scene is loading
            if (operation.progress < 0.9f)
            {
                lerpTarget = 0.9f;
                lerpTime = 5f;
            }
            else
            {
                //if scene is ready, lerp progress to 100%
                lerpTarget = 1.0f;
                lerpTime = 1f;
            }
            loadBar.value = Mathf.Lerp(0f, lerpTarget, timeElapsed/lerpTime);
            timeElapsed += Time.fixedDeltaTime;
            loadPercentage.text = loadBar.value.ToString("0.0%");
            if (loadBar.value == 1f)
            {
                //allow activation of the scene
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
