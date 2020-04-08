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

    public void StartButton()
    {
        //hiding the buttons.
        canvas.transform.Find("Title").gameObject.SetActive(false);
        canvas.transform.Find("Play").gameObject.SetActive(false);
        canvas.transform.Find("Quit").gameObject.SetActive(false);
        canvas.transform.Find("Options").gameObject.SetActive(false);
        loadBar = canvas.transform.Find("LoadingScreen").Find("LoadBar").gameObject.GetComponent<Slider>();
        loadPercentage = loadBar.transform.Find("LoadPercentage").gameObject.GetComponent<Text>();
        LoadScene();
    }

    public void OptionsButton()
    {

    }

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

    IEnumerator LoadSceneCoroutine()
    {
        float timeElapsed = 0f;
        float lerpTarget;
        float lerpTime;
        operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        canvas.transform.Find("LoadingScreen").gameObject.SetActive(true);
        while (loadBar.value < 1f)
        {
            if (operation.progress < 0.9f)
            {
                lerpTarget = 0.9f;
                lerpTime = 5f;
            }
            else
            {
                lerpTarget = 1.0f;
                lerpTime = 1f;
            }
            loadBar.value = Mathf.Lerp(0f, lerpTarget, timeElapsed/lerpTime);
            timeElapsed += Time.fixedDeltaTime;
            //Debug.Log(operation.progress);
            loadPercentage.text = loadBar.value.ToString("0.0%");
            if (loadBar.value == 1f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
