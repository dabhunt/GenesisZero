using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class CutsceneController : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayableDirector intro;
    private GameObject Primarycanvas;
    private GameObject CutsceneCanvas;
    private void Start()
    {
        Primarycanvas = GameObject.FindGameObjectWithTag("CanvasUI");
        CutsceneCanvas = GameObject.FindGameObjectWithTag("CutsceneCanvas");
    }
    public void IntroCutscene()
    {
        Cutscene();
        intro.Play();
        GameObject.FindGameObjectWithTag("GameManagers").transform.Find("TileManager").GetComponent<DeactivateDistant>().SetDist(80);
        Invoke("Reset", (float)intro.duration);
    }
    public void Cutscene()
    {
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
        GameObject.FindGameObjectWithTag("GameManagers").transform.Find("TileManager").GetComponent<DeactivateDistant>().ResetDist();
        CutsceneCanvas.SetActive(false);
        Primarycanvas.SetActive(true);
        GameInputManager.instance.EnablePlayerControls();
    }
}
