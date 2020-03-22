﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerController PC;
    private AudioManager aManager;
    public string walk;
    public string[] gunshot = new string[0];
    public string[] jump = new string[0];
    public string[] land = new string[0];
    //bool applicationIsQuitting;
    // Start is called before the first frame update
    // Update is called once per frame
    void Awake()
    {
        
        GameObject temp = GameObject.FindWithTag("Player");
        PC = temp.GetComponent<PlayerController>();
        aManager = FindObjectOfType<AudioManager>();
    }
	public void Jump()
	{
        int rng = Random.Range(1, jump.Length);
        aManager.PlaySoundOneShot(jump[rng-1]);
	}
	public void GunShot()
	{
		int rng = Random.Range(1, gunshot.Length);
        aManager.PlaySoundOneShot(gunshot[rng-1]);
	}
	public void Land()
	{
		int rng = Random.Range(1, land.Length);
        aManager.PlaySoundOneShot(land[rng-1]);
	}
	public void Walk()
	{
        aManager.PlaySound(walk);
	}
	public void StopWalk(){
		aManager.StopSound(walk);
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
