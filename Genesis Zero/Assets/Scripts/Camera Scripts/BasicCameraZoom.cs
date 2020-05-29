using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraZoom : MonoBehaviour
{
    public float zoomSpeed;
    public float fovMin;
    public float fovMax;
    private Camera myCamera;
    private float target;
    private bool spanding;
    private bool reset;
    private float time;
    private float savedpov;
    private float savedmax;
    // Use this for initialization
    void Start()
    {
        myCamera = GetComponent<Camera>();
        target = myCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
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

        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                fovMax = savedmax;
                ChangeFieldOfView(savedpov);
            }
        }

    }

    public void ChangeFieldOfView(float field)
    {
        if (spanding == false)
        {
            if (field > fovMax)
            {
                fovMax = field;
            }
            StartCoroutine(Spandout(.5f, myCamera.fieldOfView, field, spanding));
        }
    }

    public void ChangeFieldOfViewTemporary(float field, float time)
    {
        if (spanding == false)
        {
            savedmax = fovMax;
            if (field > fovMax)
            {
                fovMax = field;
            }
            this.time = time;
            savedpov = myCamera.fieldOfView;
            StartCoroutine(Spandout(.25f, myCamera.fieldOfView, field, spanding));
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
            myCamera.fieldOfView = start + ((target - start) * f / time);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        spanding = false;
    }
}