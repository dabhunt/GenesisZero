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
    private bool spanding;
    private float time;
    // Use this for initialization
    void Start()
    {
        myCamera = GetComponent<Camera>();
        target = myCamera.fieldOfView;
    }
    // Update is called once per frame
    void Update()
    {
        if (GameInputManager.instance.isEnabled())
        {
			/**
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && time <= 0)
            {
                target += zoomSpeed;
                target = Mathf.Clamp(target, fovMin, fovMax);
                myCamera.fieldOfView = target;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && time <= 0)
            {
                target -= zoomSpeed;
                target = Mathf.Clamp(target, fovMin, fovMax);
                myCamera.fieldOfView = target;
            }
			*/
        }
    }

    public void ChangeFieldOfView(float field, float tweenTime)
    {
        if (spanding == false)
        {
            myCamera.DOFieldOfView(field, tweenTime);
        }
    }
    public void ChangeFieldOfViewTemporary(float field, float duration, float tweenTime)
    {
        if (spanding == false) //only change FOV if not already doing it
        {
            float savedpov = myCamera.fieldOfView;
            myCamera.DOFieldOfView(field, tweenTime);
            StartCoroutine(Reset(duration, savedpov));
            spanding = true;
        }
    }
    IEnumerator Reset(float waitTime, float FOV)
    {
        yield return new WaitForSeconds(waitTime);
        while (Player.instance.GetComponent<UniqueEffects>().IsInCombat() == true) //continue checking while in combat
        {
            yield return new WaitForSeconds(.5f); //wait .5 seconds then check again to see if player is still in combat
        }
        myCamera.DOFieldOfView(FOV, 1.2f); //tween back to original FOV
        spanding = false;
    }
}
/*public void ChangeFieldOfViewTemporary(float field, float time, float duration)
{
    if (spanding == false)
    {
        savedmax = fovMax;
        if (field > fovMax)
        {
            fovMax = field;
            field = fovMax;
        }
        target = field;
        this.time = time;
        savedpov = myCamera.fieldOfView;
        StartCoroutine(Spandout(duration, myCamera.fieldOfView, field, spanding));
    }
}

public void StopTempFieldOfViewChange()
{
    if (time > 0 && spanding == false)
    {
        time = 0;
        target = savedpov;
        ChangeFieldOfView(savedpov);
        updatetime = .5f;
    }
}

IEnumerator Spandout(float time, float start, float target, bool spanding)
{
    spanding = true;
    for (float f = 0; f <= time; f += Time.fixedDeltaTime)
    {
        if (spanding == true && reset == true)
        {
            reset = false;
            break;
        }
        myCamera.fieldOfView = Mathf.Clamp(start + ((target - start) * f / time), fovMin, fovMax);
        yield return new WaitForSeconds(Time.fixedDeltaTime);
    }
    spanding = false;
}
}
*/
