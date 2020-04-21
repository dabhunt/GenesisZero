using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script was found at the website:
// http://theflyingkeyboard.net/unity/unity-c-zoom-in-and-out-mouse-wheel-input/
// ? 2017 TheFlyingKeyboard and released under MIT License
// theflyingkeyboard.net

//KENNY PLEASE LET ME KNOW WHAT WE SHOULD KEEP/MOVE/TAKE OUT OF THIS.
//IF THIS COMMENT IS HERE REFER TO THE LINE ABOVE AND GO BUG WILL ABOUT THIS.
public class BasicCameraZoom : MonoBehaviour
{
    public float zoomSpeed;
    public float fovMin;
    public float fovMax;
    private Camera myCamera;
    // Use this for initialization
    void Start()
    {
        myCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                myCamera.fieldOfView += zoomSpeed;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                myCamera.fieldOfView -= zoomSpeed;
            }
            myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, fovMin, fovMax);
    }
}