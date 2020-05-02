using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public Canvas canvas;
    public string sceneName;
    private AsyncOperation operation;
    private Slider loadBar;
    private Text loadPercentage;

    private GameObject mainMenuScreen;
    private GameObject optionsScreen;
    private GameObject loadingScreen;
    private TMPro.TMP_Dropdown resDropdown;
    private List<Resolution> resolutions;

    public void Start()
    {
        List<string> values;
        //Caching some canvas groups
        mainMenuScreen = canvas.transform.Find("MainMenuScreen").gameObject;
        loadingScreen = canvas.transform.Find("LoadingScreen").gameObject;
        optionsScreen = canvas.transform.Find("OptionsScreen").gameObject;
        resDropdown = optionsScreen.transform.Find("Resolution").GetComponent<TMPro.TMP_Dropdown>();
        Cursor.visible = true;
        //Populating Resolution list
        Resolution[] options = Screen.resolutions;
        resolutions = new List<Resolution>(options);
        values = new List<string>();
        resolutions.Reverse();

        int index = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].refreshRate < 60)
                continue;
            values.Add(resolutions[i].width + "x" + resolutions[i].height + " (" + resolutions[i].refreshRate + " hz)");
            if (resolutions[i].height == Screen.currentResolution.height && resolutions[i].width == Screen.currentResolution.width)
            {
                index = i;
            }
        }
        resDropdown.ClearOptions();
        resDropdown.AddOptions(values);
        resDropdown.value = index;
        resDropdown.RefreshShownValue();
    }

    //Onclick Event for Start Button
    public void StartButton()
    {
        //hiding MainMenuScreen.
        mainMenuScreen.SetActive(false);
        loadingScreen.SetActive(true);
        //grabbing some component to show loading screen
        loadBar = loadingScreen.transform.Find("LoadBar").gameObject.GetComponent<Slider>();
        loadPercentage = loadBar.transform.Find("LoadPercentage").gameObject.GetComponent<Text>();
        LoadScene();
    }
    //Onclick Event for Options Button
    public void OptionsButton()
    {
        mainMenuScreen.transform.Find("Buttons").gameObject.SetActive(false);
        canvas.transform.Find("OptionsScreen").gameObject.SetActive(true);
    }
    //Onclick event for Quit Button
    public void QuitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    private void LoadScene()
    {
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
