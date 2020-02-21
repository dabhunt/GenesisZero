using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplosiveShot : MonoBehaviour
{
    public bool quitting;
    
    public float baseblastRadius = 5.0f;
    public float lerpMultiplier=1;
    //how fast the sphere grows in size
    public string vfxName;
    public GameObject explosionPrefab;
    private float lerpScale=0;
   	private GameObject runtimeExplosion;
   	private Restart restartScript;
   	private float startScale = .1f;
   	private float blastRadius;
    void Start()
    {
        quitting = false;
        GameObject temp = GameObject.FindWithTag("EventSystem");
        restartScript = temp.GetComponent<Restart>();
        //blastRadius = baseblastRadius;
    }
    private void OnDestroy()
    {
    	//if the scene is being restarted or the player quits
        if (restartScript.ExitingScene() || quitting){
            return;
        } // otherwise Do the Explosive Effect
        else
        {
        	Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y,0);
        	//this is for collision, not VFX
 			runtimeExplosion = Instantiate(explosionPrefab,spawnPoint, Quaternion.identity);
 			AOE AOEscript = runtimeExplosion.GetComponent<AOE>();
 			AOEscript.setScaleTarget(startScale, blastRadius, lerpMultiplier);
 			print("blastRadius = " + blastRadius + "during the on destroy call last section");
 			GameObject emit = VFXManager.instance.PlayEffect(vfxName, new Vector3(transform.position.x, transform.position.y, transform.position.z), 0f, blastRadius/baseblastRadius);
        }
       
    }
    public void ModifyBlastRadius(float adjustment)
    {
    	blastRadius = baseblastRadius;
    	blastRadius += adjustment;
    	print("blastRadius in explosiveshot = " + blastRadius);
    }
    //prevent spawning things upon game window being exited
    void OnApplicationQuit()
    {
        quitting = true;
    }
    public void isQuitting(){
        quitting = true;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //quitting = true;
    }
}
//Alternative way to get enemy detection

        	       	//get array of all enemies
        	 // GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
          //    foreach (GameObject enemy in enemyArray){
          //      if(blastRadius >= Vector3.Distance(spawnPoint, enemy.transform.position)) {
          //      		//this doesn't fully utilize Kennies damage system but I'm not sure how to utilize it properly
          //                //Add Code for affected enemies here, example :
          //                //enemy.GetComponent(HealthScript).addDamage(100);
        	// 			}
          //      }