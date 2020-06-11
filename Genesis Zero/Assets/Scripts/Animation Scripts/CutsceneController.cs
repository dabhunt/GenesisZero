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
    private Animation camAnim;
    private Animation cinematicAnim;
    public string IntroName = "IntroCutscene";
    public string bugeIntro = "MeetingBuge";
    public static CutsceneController instance;
    public Vector3 playerCutscenePos;
    private GameObject Primarycanvas;
    private GameObject CutsceneCanvas;
    private Camera cam;
    private float InspectorFov;
    private bool inCutscene;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        UpdateCanvases();
        cam = Camera.main;
        InspectorFov = cam.fieldOfView;
        camAnim = cam.GetComponent<Animation>();
        cinematicAnim = GetComponent<Animation>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CutsceneCanvas != null && CutsceneCanvas.activeSelf)
            Skip();
    }
    public void IntroCutscene()
    {
        cam.fieldOfView = 20;
        Cutscene();
        camAnim.Play(IntroName);
        TileManager.instance.GetComponent<DeactivateDistant>().SetDist(100);
        Invoke("Reset", (float)camAnim[IntroName].length); //reset the normal camera after it finishes
    }
    public void BugeCutscene()
    {
        Cutscene();
        print("buge cutscene playing");
        cinematicAnim.Play(bugeIntro);
        CutsceneCanvas.SetActive(false);
        Invoke("Reset", (float)cinematicAnim[bugeIntro].length); //reset the normal camera after it finishes
    }
    public void Cutscene()
    {
        UpdateCanvases();
        inCutscene = true;
        Player.instance.GetComponent<AimIK>().enabled = false;
        GetComponentInChildren<CinemachineBrain>().enabled = true;
        GameInputManager.instance.DisablePlayerControls();
        StateManager.instance.Cursorvisible = true;
        CutsceneCanvas.SetActive(true);
        Primarycanvas.SetActive(false);
        GameInputManager.instance.DisablePlayerControls();
    }
    public void Skip()
    {
        //cutscene.fin
        camAnim[IntroName].enabled = false;
        camAnim.Play("EndIntro");
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
        CancelInvoke("Reset");
        Player.instance.GetComponent<AimIK>().enabled = true;
        UpdateCanvases();
        StateManager.instance.Cursorvisible = false;
        if(SceneManager.GetActiveScene().name != "BossTest")
		{
			DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, 15, 3);
		}
        GameInputManager.instance.EnablePlayerControls();
        TileManager.instance.GetComponent<DeactivateDistant>().ResetDist();
        GetComponentInChildren<CinemachineBrain>().enabled = true;
        Primarycanvas.SetActive(true);
        CutsceneCanvas.SetActive(false);
        Invoke("AfterDelay", 1.6f);
    }
    public void AfterDelay() //after updating to 2020, sometimes input manager doesn't register that it should start taking inputs again, this is the backup
    {
        GameInputManager.instance.EnablePlayerControls();
    }
}
