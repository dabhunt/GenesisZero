using System.Collections;
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
    private GameObject Primarycanvas;
    private GameObject CutsceneCanvas;
    private Camera cam;
    private float InspectorFov;
    private void Start()
    {
        UpdateCanvases();
        cam = Camera.main;
        InspectorFov = cam.fieldOfView;
        cutscene = transform.Find("MainCamera").gameObject.GetComponent<Animation>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CutsceneCanvas.activeSelf)
            Skip();
    }
    public void IntroCutscene()
    {
        cam.fieldOfView = 20;
        Cutscene();
        cutscene.Play(IntroName);
        GameObject.FindGameObjectWithTag("GameManagers").transform.Find("TileManager").GetComponent<DeactivateDistant>().SetDist(100);
        Invoke("Reset", (float)cutscene[IntroName].length); //reset the normal camera after it finishes
    }
    public void Cutscene()
    {
        UpdateCanvases();
        Player.instance.GetComponent<AimIK>().enabled = false;
        GetComponent<CinemachineBrain>().enabled = true;
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
        Player.instance.GetComponent<AimIK>().enabled = true;
        UpdateCanvases();
        StateManager.instance.Cursorvisible = false;
        if(SceneManager.GetActiveScene().name != "BossTest")
		{
			DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, 15, 3);
		}
        GameInputManager.instance.EnablePlayerControls();
        GameObject.FindGameObjectWithTag("GameManagers").transform.Find("TileManager").GetComponent<DeactivateDistant>().ResetDist();
        GetComponent<CinemachineBrain>().enabled = true;
        Primarycanvas.SetActive(true);
        CutsceneCanvas.SetActive(false);
        Invoke("AfterDelay", 1.6f);
    }
    public void AfterDelay() //after updating to 2020, sometimes input manager doesn't register that it should start taking inputs again, this is the backup
    {
        GameInputManager.instance.EnablePlayerControls();
    }
}
