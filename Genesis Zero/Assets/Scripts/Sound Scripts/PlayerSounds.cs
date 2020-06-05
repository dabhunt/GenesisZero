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
		PC = Player.instance.gameObject.GetComponent<PlayerController>();
		aManager = AudioManager.instance;
	}
	public void Jump()
	{
		aManager.PlayRandomSFXType("Jump", null, .3f);
	}
	public void GunShot()
	{
		aManager.PlayRandomSFXType("Gunshot", playerObj, 1, 1.5f, .4f);
	}
	public void Roll()
	{
		aManager.PlayRandomSFXType("PhaseRoll", playerObj, .9f, 1.2f, .9f);
	}
	public void Land()
	{
		if (GameInputManager.instance.isEnabled())
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
