using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateWindow : MonoBehaviour
{
    public GameObject pauseWindow;
    //public GameObject buttonPrefab;
    public GameObject windowSacrificePrefab;
    private GameObject runtimeWindow;
    public Canvas canvas;  

    void Start()
    {
        GameObject tempObj = GameObject.FindWithTag("CanvasUI");
        canvas = tempObj.GetComponent<Canvas>();
    }
    public void CreatePauseWindow()
    {
        runtimeWindow = Instantiate(pauseWindow);
        runtimeWindow.transform.SetParent (canvas.transform, false);
        Time.timeScale = 0f;
        print("create pause window running..");
    }
    public void DestroyWindow()
    {
        Time.timeScale = 1.0f;
        DestroyImmediate(runtimeWindow);
        print("destroyWindow running..");
    }
}
