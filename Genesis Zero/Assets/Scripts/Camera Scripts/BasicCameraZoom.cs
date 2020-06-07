using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BasicCameraZoom : MonoBehaviour
{
    public float zoomSpeed;
    public float fovMin;
    public float fovMax;
    private Camera myCamera;
    private float target;
    private bool spanding = false;
    private float savedFOV;
    private float time;
	[HideInInspector]
	public bool inboss;
    // Use this for initialization
    void Start()
    {
        myCamera = GetComponent<Camera>();
        target = myCamera.fieldOfView;
        savedFOV = myCamera.fieldOfView;
    }
    // Update is called once per frame
    void Update()
    {
        if (GameInputManager.instance.isEnabled() && !spanding)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && time <= 0)
            {
                myCamera.DOFieldOfView(myCamera.fieldOfView+3, .5f);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && time <= 0)
            {
                myCamera.DOFieldOfView(myCamera.fieldOfView+3, .5f);
            }
        }
    }
    public void ChangeFieldOfView(float newFOV, float tweenTime)
    {
        if (spanding == false)
        {
            savedFOV = myCamera.fieldOfView; //save current FOV
            myCamera.DOFieldOfView(newFOV, tweenTime);
            StartCoroutine(Reset(false, tweenTime));
        }
    }
    public void ChangeFieldOfViewTemporary(float field, float duration, float tweenTime)
    {
        if (spanding == false) //only change FOV if not already doing it
        {
            savedFOV = myCamera.fieldOfView;
            Tween t = myCamera.DOFieldOfView(field, tweenTime);
            t.SetUpdate(UpdateType.Fixed);
            StartCoroutine(Reset(true,duration));
            spanding = true;
        }
    }
    //resets the spanding value and FOV if it was a temporary change
    // pass in whether or not this reset is for a temp FOV change, or permanent one, and how long to wait before doing the rest
    IEnumerator Reset(bool temp, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        spanding = false;
        if (!temp)
            yield return 0;
        while (Player.instance.GetComponent<UniqueEffects>().IsInCombat() == true) //continue checking while in combat
        {
            yield return new WaitForSeconds(.5f); //wait .5 seconds then check again to see if player is still in combat
        }
        if (!inboss)
		{
            Tween t = myCamera.DOFieldOfView(savedFOV, 1.2f); //tween back to original FOV
            t.SetUpdate(UpdateType.Fixed);
		}
		spanding = false;
    }
    public void ZoomOnCombat(bool flag)
    {
        inboss = flag;
        if (!flag)
        {
            StopAllCoroutines();
        }
    }
}
