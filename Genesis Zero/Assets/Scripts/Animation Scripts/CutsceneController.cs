﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cinemachine;
using RootMotion.FinalIK;
public class CutsceneController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animation cutscene;
    public string IntroName = "IntroCutscene";
    public string bugeIntro = "MeetingBuge";

    public Vector3 playerCutscenePos;
    public Vector3 bugePos;
    public Vector3 bugeRotation;
    public Transform buge = BUGE.instance.transform;
    private Vector3 playerPreCutscene;
    private GameObject Primarycanvas;
    private GameObject CutsceneCanvas;
    private Camera cam;
    private float InspectorFov;
    private bool inCutscene;
    private void Start()
    {
        UpdateCanvases();
        cam = Camera.main;
        InspectorFov = cam.fieldOfView;
        cutscene = GetComponent<Animation>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CutsceneCanvas.activeSelf)
            Skip();
        if (inCutscene)
        {
            BUGE.instance.transform.position = bugePos;
            BUGE.instance.transform.rotation = Quaternion.Euler(bugeRotation);
        }
    }
    public void IntroCutscene()
    {
        playerPreCutscene = Player.instance.transform.position; 
        cam.fieldOfView = 20;
        Cutscene();
        cutscene.Play(IntroName);
        TileManager.instance.GetComponent<DeactivateDistant>().SetDist(100);
        Invoke("Reset", (float)cutscene[IntroName].length); //reset the normal camera after it finishes
    }
    public void BugeCutscene()
    {
        Cutscene();
        cutscene.Play(IntroName);
        CutsceneCanvas.SetActive(false);
        Invoke("Reset", (float)cutscene[bugeIntro].length); //reset the normal camera after it finishes
    }
    public void Cutscene()
    {
        UpdateCanvases();
        inCutscene = true;
        Player.instance.GetComponent<AimIK>().enabled = false;
        GetComponentInParent<CinemachineBrain>().enabled = true;
        GameInputManager.instance.DisablePlayerControls();
        StateManager.instance.Cursorvisible = true;
        CutsceneCanvas.SetActive(true);
        Primarycanvas.SetActive(false);
        GameInputManager.instance.DisablePlayerControls();
    }
    public void Skip()
    {
        //cutscene.fin
        cutscene[IntroName].enabled = false;
        cutscene.Play("EndIntro");
        Reset();
    }
    public void UpdateCanvases()
    {
        if (Primarycanvas == null)
            Primarycanvas = GameObject.FindGameObjectWithTag("CanvasUI");
        if (CutsceneCanvas == null)
            CutsceneCanvas = GameObject.FindGameObjectWithTag("CutsceneCanvas");
    }
    public void Reset()
    {
        inCutscene = false;
        Player.instance.transform.position = playerPreCutscene;
        Player.instance.GetComponent<AimIK>().enabled = true;
        UpdateCanvases();
        StateManager.instance.Cursorvisible = false;
        if(SceneManager.GetActiveScene().name != "BossTest")
		{
			DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, 15, 3);
		}
        GameInputManager.instance.EnablePlayerControls();
        TileManager.instance.GetComponent<DeactivateDistant>().ResetDist();
        GetComponentInParent<CinemachineBrain>().enabled = true;
        Primarycanvas.SetActive(true);
        CutsceneCanvas.SetActive(false);
        Invoke("AfterDelay", 1.6f);
    }
    public void AfterDelay() //after updating to 2020, sometimes input manager doesn't register that it should start taking inputs again, this is the backup
    {
        GameInputManager.instance.EnablePlayerControls();
    }
}
