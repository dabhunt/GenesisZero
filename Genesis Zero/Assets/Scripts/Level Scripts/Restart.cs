using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    //private Player player;
    private GameObject temp;
    private bool restartingScene;

    void Start()
    {
    	temp = GameObject.FindWithTag("Player");
    	restartingScene = false;
        //player = temp.GetComponent<Player>();
    }
    void Update(){
    	//if player is dead, restart the scene
    	if (temp == null){
    		restartingScene = true;
    		RestartScene();
    	} else{
    		restartingScene = false;
    	}
    }

    // Update is called once per frame
    void RestartScene()
    {
    	string scene = SceneManager.GetActiveScene().name;
		//Load it
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    public bool isRestartingScene (){
    	return restartingScene;
    }
}
