using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    //private Player player;
    private GameObject temp;
    public bool exitingScene;

    void Start()
    {
    	temp = GameObject.FindWithTag("Player");
    	exitingScene = false;
        //player = temp.GetComponent<Player>();
    }
    void Update()
    {
    	//if player is dead, restart the scene
    	if (temp == null){
    		exitingScene = true;
    		RestartScene();
    	} else{
    		exitingScene = false;
    	}
    }

    // Update is called once per frame
    void RestartScene()
    {
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
}
