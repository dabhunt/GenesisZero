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
    public void LoadFromMenu(string sceneName)
    {
        StartCoroutine(LoadFromMenuCoRoutine(sceneName));
    }

    IEnumerator LoadFromMenuCoRoutine (string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadBar.value = progress;
            loadPercentage.text = progress * 100 + " %";
            Debug.Log(progress);
            yield return null;
        }
    }
}
