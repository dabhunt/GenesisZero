using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayableDirector intro;
    private GameObject Primarycanvas;
    private GameObject CutsceneCanvas;
    private Camera cam;
    private float InspectorFov;
    private void Start()
    {
        cam = Camera.main;
        InspectorFov = cam.fieldOfView;
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
        intro.Play();
        GameObject.FindGameObjectWithTag("GameManagers").transform.Find("TileManager").GetComponent<DeactivateDistant>().SetDist(100);
        Invoke("Reset", (float)intro.duration);
    }
    public void Cutscene()
    {
        GameInputManager.instance.DisablePlayerControls();
        StateManager.instance.Cursorvisible = true;
        CutsceneCanvas.SetActive(true);
        Primarycanvas.SetActive(false);
        GameInputManager.instance.DisablePlayerControls();
    }
    public void Skip()
    {
        intro.time = intro.duration;
        Reset();
    }
    public void Reset()
    {
        Primarycanvas = GameObject.FindGameObjectWithTag("CanvasUI");
        CutsceneCanvas = GameObject.FindGameObjectWithTag("CutsceneCanvas");
        StateManager.instance.Cursorvisible = false;
        if(SceneManager.GetActiveScene().name != "BossTest")
		{
			DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, 15, 3);
		}
        GameInputManager.instance.EnablePlayerControls();
        GameObject.FindGameObjectWithTag("GameManagers").transform.Find("TileManager").GetComponent<DeactivateDistant>().ResetDist();
        Primarycanvas.SetActive(true);
        CutsceneCanvas.SetActive(false);
    }
}
