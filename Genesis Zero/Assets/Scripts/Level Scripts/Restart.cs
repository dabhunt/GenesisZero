using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
public class Restart : MonoBehaviour
{
    //private Player player;
    private GameObject player;
    public bool exitingScene;

    private GameObject canvas;
    private GameObject pauseMenu;
    private GameObject resume;
    private GameObject overlay;
    private GameObject gameovertext;
    private bool dead = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    	exitingScene = false;
        //player = temp.GetComponent<Player>();
        canvas = GameObject.FindWithTag("CanvasUI");
        pauseMenu = canvas.transform.Find("PauseMenu").gameObject;
        gameovertext = canvas.transform.Find("Gameover").gameObject;
        resume = pauseMenu.transform.Find("Buttons").transform.Find("Continue").gameObject;
        overlay = canvas.transform.Find("BlackOverlay").gameObject;
    }
    void Update()
    {
    	//if player is dead, restart the scene
    	if (player == null && dead == false)
        {
            // Play gameover music/fade out any playing tracks
            // AudioManager.instance.PlayTrack(1, "Music", "GameOver", false, true);
            dead = true;
            // Fade to black
            overlay.SetActive(true);
            GameObject buge = GameObject.FindWithTag("BUG-E");
            AudioManager.instance.PlaySound("SFX_GameOver", 3, 1, false, buge.transform.position);
            overlay.GetComponent<SpriteFade>().FadeIn(2f);
            gameovertext.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
            gameovertext.GetComponent<TextMeshProUGUI>().DOFade(1, 1.5F);
            // Show game over menu after fading in
            StartCoroutine("ExecuteAfterTime", 2.5f);
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
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickups");
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i].GetComponent<InteractPopup>() != null)
                pickups[i].GetComponent<InteractPopup>().DestroyPopUp();
        }
        pauseMenu.GetComponent<Image>().enabled = false;
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
