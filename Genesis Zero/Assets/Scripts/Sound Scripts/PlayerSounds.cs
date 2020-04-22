using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerController PC;
    private AudioManager aManager;
    //bool applicationIsQuitting;
    // Start is called before the first frame update
    // Update is called once per frame
	private void Start()
	{
		GameObject temp = GameObject.FindWithTag("Player");
		PC = temp.GetComponent<PlayerController>();
		aManager = FindObjectOfType<AudioManager>();
	}
	public void Jump()
	{
		aManager.PlayRandomSFXType("Jump");
	}
	public void GunShot()
	{
		aManager.PlayRandomSFXType("Gunshot");
	}
	public void Land()
	{
		aManager.PlayRandomSFXType("Jump");
	}
	public void Walk()
	{
		aManager.PlaySound("SFX_Running");
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
