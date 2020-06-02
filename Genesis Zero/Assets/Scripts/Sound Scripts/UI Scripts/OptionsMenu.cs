using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    private Canvas canvas;
    private AsyncOperation operation;
    private Slider loadBar;
    private Text loadPercentage;

    private GameObject mainMenuScreen;
    private GameObject optionsScreen;
    private GameObject loadingScreen;
    private TMPro.TMP_Dropdown resDropdown;
    private List<Resolution> resolutions;
    private SettingsData settingsData;

    public void Start()
    {
        Debug.Log("Options Start");
        List<string> values;
        canvas = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            mainMenuScreen = canvas.transform.Find("MainMenuScreen").gameObject;
        }
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
            if (resolutions[i].height == Screen.height && resolutions[i].width == Screen.width)
            {
                index = i;
            }
        }
        resDropdown.ClearOptions();
        resDropdown.AddOptions(values);
        resDropdown.value = index;
        resDropdown.RefreshShownValue();
        if (SaveLoadManager.instance.SettingsSaveExists())
        {
            settingsData = SaveLoadManager.instance.LoadSettings();
            Debug.Log("Applying Settings");
            //Applying saved settings
            SetMasterVolume(settingsData.masterVolume);
            SetSFXVolume(settingsData.sfxVolume);
            SetMusicVolume(settingsData.musicVolume);
            MuteMaster(settingsData.muteMaster);
            MuteSFX(settingsData.muteSFX);
            MuteMusic(settingsData.muteMusic);
            SetFullScreen(settingsData.fullScreen);
            SetResolution(settingsData.resIndex);
            SetSliderValue("Master", settingsData.masterVolume);
            SetSliderValue("Music", settingsData.musicVolume);
            SetSliderValue("SFX", settingsData.sfxVolume);
            SetToggleValue("Master", settingsData.muteMaster);
            SetToggleValue("Music", settingsData.muteMusic);
            SetToggleValue("SFX", settingsData.muteSFX);
            resDropdown.value = settingsData.resIndex;
            resDropdown.RefreshShownValue();
            optionsScreen.transform.Find("Resolution").Find("FullScreen").gameObject.GetComponent<Toggle>().isOn = settingsData.fullScreen;
        }
        optionsScreen.SetActive(false);
    }

    public void OptionsButton()
    {
        canvas.transform.Find("OptionsScreen").gameObject.SetActive(true);
    }
    //Onclick Event for BackButton(options menu)
    public void OptionsBackButton()
    {
        settingsData = new SettingsData();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            mainMenuScreen.transform.Find("Buttons").gameObject.SetActive(true);
            mainMenuScreen.SetActive(true);
        }
        else 
        {
            //if in main scene do this
        }
        AudioManager.instance.mixer.GetFloat("masterVolume", out settingsData.masterVolume);
        AudioManager.instance.mixer.GetFloat("sfxVolume", out settingsData.sfxVolume);
        AudioManager.instance.mixer.GetFloat("musicVolume", out settingsData.musicVolume);
        settingsData.fullScreen = optionsScreen.transform.Find("Resolution").Find("FullScreen").gameObject.GetComponent<Toggle>().isOn;
        settingsData.resIndex = resDropdown.value;
        settingsData.muteMaster = GetToggleValue("Master");
        settingsData.muteSFX = GetToggleValue("SFX");
        settingsData.muteMusic = GetToggleValue("Music");
        //Saves user preferences
        SaveLoadManager.instance.SaveSettings(settingsData);
        optionsScreen.SetActive(false);
    }

    //Event to set master volume
    public void SetMasterVolume(float value)
    {
        //Debug.Log("MasterVolume: " + value);
        if (value > -40f)
        {
            AudioManager.instance.mixer.SetFloat("masterVolume", value);
        }
        else
        {
            AudioManager.instance.mixer.SetFloat("masterVolume", -80f);
        }
    }

    //Event to set sfx volume
    public void SetSFXVolume(float value)
    {
        //Debug.Log("SFXVolume: " + value);
        if (value > -40f)
        {
            AudioManager.instance.mixer.SetFloat("sfxVolume", value);
        }
        else
        {
            AudioManager.instance.mixer.SetFloat("sfxVolume", -80f);
        }
    }

    //Event to set music volume
    public void SetMusicVolume(float value)
    {
        //Debug.Log("MusicVolume: " + value);
        if (value > -40f)
        {
            AudioManager.instance.mixer.SetFloat("musicVolume", value * .7f);
        }
        else
        {
            AudioManager.instance.mixer.SetFloat("musicVolume", -80f);
        }
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
        return optionsScreen.transform.Find("AudioSettings").Find(name).gameObject.GetComponent<Slider>().value;
    }

    private void SetInteractable(string name, bool value)
    {
        Slider slider = optionsScreen.transform.Find("AudioSettings").Find(name).gameObject.GetComponent<Slider>();
        slider.interactable = value;
    }
    private void SetSliderValue(string name, float value)
    {
        float val = Mathf.Clamp(value, -40f, 0f);
        optionsScreen.transform.Find("AudioSettings").Find(name).gameObject.GetComponent<Slider>().value = val;
    }

    private void SetToggleValue(string name, bool value)
    {
        optionsScreen.transform.Find("AudioSettings").Find(name).Find("Toggle").GetComponent<Toggle>().isOn = value;
    }
    private bool GetToggleValue(string name)
    {
        return optionsScreen.transform.Find("AudioSettings").Find(name).Find("Toggle").GetComponent<Toggle>().isOn;
    }
    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
        //Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen, Screen.currentResolution.refreshRate);
    }
    //Event to set Resolution
    public void SetResolution(int index)
    {
        if (index > resolutions.Count || index < 0)
            index = 0;
        Debug.Log("Changing res to: " + resolutions[index].width + " x " + resolutions[index].height);
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen, resolutions[index].refreshRate);
    }
}