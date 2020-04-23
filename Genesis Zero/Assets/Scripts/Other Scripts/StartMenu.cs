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

    //Onclick Event for BackButton(options menu)
    public void OptionsBackButton()
    {
        optionsScreen.SetActive(false);
        mainMenuScreen.transform.Find("Buttons").gameObject.SetActive(true);
        mainMenuScreen.SetActive(true);
    }

    //Event to set master volume
    public void SetMasterVolume(float value)
    {
        if (value >= -40f)
            AudioManager.instance.mixer.SetFloat("masterVolume", value);
        else
            AudioManager.instance.mixer.SetFloat("masterVolume", -80f);
    }

    //Event to set sfx volume
    public void SetSFXVolume(float value)
    {
        if (value >= -40f)
            AudioManager.instance.mixer.SetFloat("sfxVolume", value);
        else
            AudioManager.instance.mixer.SetFloat("sfxVolume", -80f);
    }

    //Event to set music volume
    public void SetMusicVolume(float value)
    {
        if (value >= -40f)
            AudioManager.instance.mixer.SetFloat("musicVolume", value);
        else
            AudioManager.instance.mixer.SetFloat("musicVolume", -80f);
    }

    //Event for toggle mute(master)
    public void MuteMaster(bool value)
    {
        if (value)
        {
            SetInteractable("Master", false);
            SetMasterVolume(-80f);
        }
        else
        {
            SetInteractable("Master", true);
            SetMasterVolume(GetSliderValue("Master"));
        }
    }

    //Event for toggle mute(sfx)
    public void MuteSFX(bool value)
    {
        if (value)
        {
            SetInteractable("SFX", false);
            SetSFXVolume(-80f);
        }
        else
        {
            SetInteractable("SFX", true);
            SetSFXVolume(GetSliderValue("SFX"));
        }
    }

    //Event for toggle mute(music)
    public void MuteMusic(bool value)
    {
        if (value)
        {
            SetInteractable("Music", false);
            SetMusicVolume(-80f);
        }
        else
        {
            SetInteractable("Music", true);
            SetMusicVolume(GetSliderValue("Music"));
        }
    }

    private float GetSliderValue(string name)
    {
        float volume = optionsScreen.transform.Find("AudioSettings").Find(name).gameObject.GetComponent<Slider>().value;
        return volume;
    }

    private void SetInteractable(string name, bool value)
    {
        Slider slider = optionsScreen.transform.Find("AudioSettings").Find(name).gameObject.GetComponent<Slider>();
        slider.interactable = value;
    }

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }
    //Event to set Resolution
    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen, resolutions[index].refreshRate);
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
