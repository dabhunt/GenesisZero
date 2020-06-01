﻿using System.Collections;
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
	private float updatetime;
    // Use this for initialization
    void Start()
    {
        myCamera = GetComponent<Camera>();
        target = myCamera.fieldOfView;
		savedpov = target;
		savedmax = fovMax;
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
				target = savedpov;
                ChangeFieldOfView(savedpov);
            }
        }

		if (updatetime > 0)
		{
			updatetime -= Time.deltaTime;
			if (updatetime <= 0)
			{
				fovMax = savedmax;
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
			target = field;
			StartCoroutine(Spandout(.5f, myCamera.fieldOfView, field, spanding));
        }
    }

    public void ChangeFieldOfViewTemporary(float field, float time, float duration)
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