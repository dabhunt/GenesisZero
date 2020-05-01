using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerController PC;
    private AudioManager aManager;
	private GameObject playerObj;
    //bool applicationIsQuitting;
    // Start is called before the first frame update
    // Update is called once per frame
	private void Start()
	{
		playerObj = GameObject.FindWithTag("Player");
		PC = playerObj.GetComponent<PlayerController>();
		aManager = FindObjectOfType<AudioManager>();
	}
	public void Jump()
	{
		aManager.PlayRandomSFXType("Jump", null, .3f);
	}
	public void GunShot()
	{
		aManager.PlayRandomSFXType("Gunshot", null, 1, 1.5f, .7f);
	}
	public void Land()
	{
		aManager.PlayRandomSFXType("Jump", null, .3f);
	}
	public void Walk()
	{
		aManager.PlayAttachedSound("SFX_Running", null, .3f, 1, false, 0);
	}
	public void StopWalk(){
		aManager.StopSound("SFX_Running");
	}
	public void DoubleJump()
	{

	}
	// public void PlaySound(string sound)
	// {
	// 	string[] soundArray = Type.GetType(sound);
	// 	if (soundArray != null)
	// 	{
	// 		int rng = Random.Range(1, soundArray.Length);
	// 	}
	// }

}
