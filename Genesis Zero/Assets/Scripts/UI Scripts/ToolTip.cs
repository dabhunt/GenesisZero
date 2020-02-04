using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
	[SerializeField]
	private Camera uiCamera;

  //   public GameObject toolTipPrefab;
  //   public Canvas canvas; 
  //   public Text tooltipText;
 	// public RectTransform backgroundRectTransform;

  //   public float toolTipDuration;
 	// public float delayBeforeShowing;
 	// private float timeLeft;

 	

    private void Awake()
    {
    	//gameObject.SetActive(false);
    	ShowToolTip("Random ToolTip text");
    }
    private void Update()
    {
    	// var screenPoint = Vector3(Input.mousePosition);
    	// screenPoint.z = 20.0f; //distance of plane in main camera is also 20
    	// transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
    	//  text.rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    	// Vector2 startPointPos = myCam.WorldToScreenPoint(startPoint.position);
    }

    private void ShowToolTip(string toolTipString)
    {
    	print("Show tool tip func running");
    	// gameObject.SetActive(true);
    	// float textPaddingSize = 4f;
    	// tooltipText.text = toolTipString;
    	// Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
    	// backgroundRectTransform.sizeDelta = backgroundSize;
    }
    public void DestroyToolTip()
    {
        // GameObject currentWindow = GameObject.Find("ToolTip(Clone)");
        print("destroyTooltip running..");
    }
  

}


    // void Start()
    // {
    //     GameObject tempObj = GameObject.Find("CanvasUI");
    //     canvas = tempObj.GetComponent<Canvas>();
    //     timeLeft = delayBeforeShowing;
    // }
    // void Update()
    // {
    // 	if (canvas.BlockedbyUI == true)
    // 	{
    // 		TimerUpdate();
    // 	}else
    // 	{
    // 		timeLeft = delayBeforeShowing;
    // 	}
    // 	TimerUpdate();
    // }
  // public void TimerUpdate()
  //   {
  //   	timeLeft -= Time.deltaTime;
 
	 // if (timeLeft <= 0.0f)
	 // {
	 //    CreateToolTip("This is my test tool tip description, lorem ipsum blan taco waffle cat.");
	 // }
  //   }