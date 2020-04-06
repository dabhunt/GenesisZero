using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    public GameObject loadingScreen;
    public Slider loadBar;
    public Text loadPercentage;
    private AsyncOperation operation;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public void QuitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);
        operation.allowSceneActivation = false;
        float timeElapsed = 0f;
        float lerpTarget;
        float lerpTime;
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
