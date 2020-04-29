using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class Restart : MonoBehaviour
{
    //private Player player;
    private GameObject player;
    public bool exitingScene;

    public GameObject canvas;
    public GameObject pauseMenu;
    public GameObject resume;
    public GameObject overlay;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    	exitingScene = false;
        //player = temp.GetComponent<Player>();
        canvas = GameObject.FindWithTag("CanvasUI");
        pauseMenu = canvas.transform.Find("PauseMenu").gameObject;
        resume = pauseMenu.transform.Find("Decline (1)").gameObject;
        overlay = canvas.transform.Find("BlackOverlay").gameObject;
    }
    void Update()
    {
    	//if player is dead, restart the scene
    	if (player == null)
        {
            // Play gameover music/fade out any playing tracks
            // AudioManager.instance.PlayTrack(1, "Music", "GameOver", false, true);

            // Fade to black
            overlay.SetActive(true);
            overlay.GetComponent<SpriteFade>().FadeIn(2f);

            // Show game over menu after fading in
            StartCoroutine("ExecuteAfterTime", 4f);
        } else{
    		exitingScene = false;
    	}

    }

    // Update is called once per frame
    public void RestartScene()
    {
        exitingScene = true;
        string scene = SceneManager.GetActiveScene().name;
		//Load it
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    void OnApplicationQuit()
    {
        exitingScene = true;
    }
    public bool ExitingScene()
    {
    	return exitingScene;
    }

    private void GameOverMenu()
    {
        resume.SetActive(false);
        pauseMenu.SetActive(true);
    }

    // function to delay code written after it
    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        // Code to execute after the delay
        GameOverMenu();
    }
}
