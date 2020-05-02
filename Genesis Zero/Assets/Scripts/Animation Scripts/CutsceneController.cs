using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
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
        Primarycanvas = GameObject.FindGameObjectWithTag("CanvasUI");
        cam = Camera.main;
        InspectorFov = cam.fieldOfView;
        CutsceneCanvas = GameObject.FindGameObjectWithTag("CutsceneCanvas");
        Invoke("Delayed", 2f);
    }
    private void Delayed()
    {
        Cursor.visible = true;
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
        Cursor.visible = true;
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
        DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, InspectorFov, 3);
        Cursor.visible = false;
        GameInputManager.instance.EnablePlayerControls();
        GameObject.FindGameObjectWithTag("GameManagers").transform.Find("TileManager").GetComponent<DeactivateDistant>().ResetDist();
        CutsceneCanvas.SetActive(false);
        Primarycanvas.SetActive(true);
        GameInputManager.instance.EnablePlayerControls();
    }
}
