using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
public class Teleporter : MonoBehaviour
{
	public float destinationX = -120;
	public float destinationY = 16;
	public float destinationZ = 0;
	public bool BossRoomOverride = false;
	//Variables for animator
	public float toWhite = .5f;
	public float stayWhite = .6f;
	public float fadeWhite = 1f;
	private Transform player;
	private GameObject buge;
	public float portalactivedistance = 9.5f;
	private GameObject canvas;
	public bool hasPair = false;
	Animator ani;

	private void Start()
	{
		buge = GameObject.FindWithTag("BUG-E");
		player = GameObject.FindWithTag("Player").transform;    //Grabs the player transform position to use in the rest of the script.
		ani = GetComponent<Animator>();
		canvas = GameObject.FindGameObjectWithTag("CanvasUI");
		InvokeRepeating("portalAnimation", 1f, .3f);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Player>())
		{
			//thePlayer.transform.position = destination.position;

			//thePlayer.transform.position = new Vector3(-120, 16, 0);
			//	if(GetAnimationStateTime()>=1){
			//d();

			//ani.Play("TeleportAnimation");
			TeleportWithAnim();
		}
	}


	private void Update()
	{
		//portalAnimation();
	}
	public void TeleportWithAnim()
	{
		canvas.transform.Find("BlackOverlay").GetComponent<SpriteFade>().color = Color.white;
		canvas.transform.Find("BlackOverlay").GetComponent<SpriteFade>().FadeIn(toWhite);
		Camera cam = Camera.main;
		cam.GetComponentInParent<CinemachineBrain>().enabled = false;
		cam.transform.position = new Vector3(player.transform.position.x, cam.transform.position.y, cam.transform.position.z);
		//GameObject.FindGameObjectWithTag("CMcam").GetComponent<CinemachineVirtualCamera>().enabled = false;
		Invoke("CallTeleportFromAnimation", toWhite);
		TileManager.instance.GetComponent<TileManager>().mayhemLevelUp();
	}
	public void SkipTutorial()
	{
		SkillManager sk = Player.instance.GetSkillManager();
		sk.AddSkill(sk.GetRandomAbility());
		while (sk.GetRandomGoldsFromPlayer(1).Count < 1 && sk.GetPlayerMods().Count < 5) // if the player has a legendary OR 4 mods, stop rolling
			sk.AddSkill(sk.GetRandomModsByChance(1)[0]);
		DialogueManager.instance.TriggerDialogue("BUG-E_SkipTut");
		Destroy(gameObject.GetComponent<FindingBuge>());
		buge.GetComponent<InteractPopup>().SetInteractable(false);
		buge.GetComponent<InteractPopup>().DestroyPopUp();
		buge.GetComponent<InteractPopup>().SetText("Right Click to Interact");
	}
	private void CallTeleportFromAnimation()
	{
		Invoke("AfterTele",stayWhite);
		Teleport();
		if (this.name.Contains("Skip"))
			SkipTutorial();
	}
	public void AfterTele()
	{
		canvas.transform.Find("BlackOverlay").GetComponent<SpriteFade>().FadeOut(fadeWhite);
	}
	private void Teleport()
	{
		if (TileManager.instance.MayhemMode)
			MayhemTimer.instance.LevelCleared();
		StateManager.instance.Teleport(new Vector2(destinationX, destinationY), BossRoomOverride, hasPair);
		hasPair = true;
	}
	public void SetDestination(Vector2 destination)
	{
		destinationX = destination.x;
		destinationY = destination.y;
		destinationZ = 0;
	}
	public float GetAnimationStateTime(){
		return Mathf.Repeat(ani.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f)+.1f;
	}

	public void portalAnimation()
    {
			if(distanceFromPortal()<portalactivedistance){
				ani.SetBool("Open",true);
				AudioManager.instance.PlayAttachedSound("SFX_TeleporterAmbient", this.gameObject, .5f, 1, true, 0);
				ani.SetBool("Close",false);
			}
			if(distanceFromPortal()>portalactivedistance){
				ani.SetBool("Close",true);
				ani.SetBool("Open",false);
			}
	}
    protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, 3);
	}

	public float distanceFromPortal()
    {
		float distance;

		if (player == null) 
			return -1;
		distance=Vector3.Distance(transform.position,player.position); //keeps everything a positive value and returns the distance
		return distance;
    }
}
