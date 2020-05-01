using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    private GameObject mainMenuScreen;
    private GameObject optionsScreen;
    private TMPro.TMP_Dropdown resDropdown;
    private List<Resolution> resolutions;

    private GameObject canvas;

    private void Start() 
    {
        List<string> values;
        //Caching some canvas groups
        canvas = GameObject.FindGameObjectWithTag("CanvasUI");
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
}
