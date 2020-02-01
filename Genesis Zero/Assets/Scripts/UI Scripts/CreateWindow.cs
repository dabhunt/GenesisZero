using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateWindow : MonoBehaviour
{
    public GameObject windowPrefab;
    public GameObject buttonPrefab;
    public GameObject runtimeWindow;
    public Canvas canvas;  

    void Start()
    {
        GameObject tempObj = GameObject.Find("CanvasUI");
        canvas = tempObj.GetComponent<Canvas>();
    }
    void Update()
    {
        
    }
    public void CreateNewWindow()
    {
    	runtimeWindow = Instantiate(windowPrefab);
        runtimeWindow.transform.SetParent (canvas.transform, false);
        Time.timeScale = 0f;
    }
    public void DestroyWindow()
    {
        Time.timeScale = 1.0f;
        GameObject currentWindow = GameObject.Find("FullscreenWindow(Clone)");
        DestroyImmediate(currentWindow);
        print("destroyWindow running..");
    }
}
