using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;
    private Player player;
    private float TimeScale = 1;
    private float Timer = 0;
    private bool isPaused;
    public Restart restart;
    public GameObject canvas;
    private GameObject pauseMenu;
    private AsyncOperation operation;
    private GameObject optionsMenu;
    private GameObject pMenuButtons;
    private Vector2 BossRoomLocation = new Vector2(-346, 315f);
    public bool Cursorvisible = true;
    public bool GameOver = false;
    public bool InTutorial = true;
    private float tweenduration = 0;
    private Animation timeslowEffect;
    private bool timeSlowEffectActive = true;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Player");
        player = Player.instance;
        if (!SaveLoadManager.instance.newGame)
        {
            PlayerData pData = SaveLoadManager.instance.LoadPlayerData();
            //MapData mData = SaveLoadManager.instance.LoadMapData();
            print("loading player data");
            SaveLoadManager.instance.ApplyPlayerData(pData);
        }
        temp = StateManager.instance.gameObject;
        restart = temp.GetComponent<Restart>();
        canvas = GameObject.FindWithTag("CanvasUI");
        pauseMenu = canvas.transform.Find("PauseMenu").gameObject;
        optionsMenu = canvas.transform.Find("OptionsScreen").gameObject;
        timeslowEffect = Camera.main.transform.parent.Find("TimeSlow").GetComponent<Animation>();
        //timeslowEffect["TimeSlowOff"].wrapMode = WrapMode.Once;
        //timeslowEffect["TimeSlowOn"].wrapMode = WrapMode.Once;
    }
    private void Update()
    {
        if (Timer >= 0)
        {
            print("Timescale: "+TimeScale);
            Timer -= Time.unscaledDeltaTime;
            if (Timer < 0)
            {
                ChangeTimeScale(1, 0);
                if (timeSlowEffectActive)
                {
                    timeSlowEffectActive = false;
                    print("timeslowoff now playing");
                    timeslowEffect.Play("TimeSlowOff");
                }

            }
            //temporary input usage for demo tomorrow
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Time.realtimeSinceStartup > 10)
                {
                    if (pauseMenu.activeSelf)
                    {
                        pauseMenu.SetActive(false);
                        ToggleOptionsMenu(false);
                        UnpauseGame();
                    }
                    else
                    {
                        pauseMenu.SetActive(true);
                        PauseGame();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
                restart.RestartScene();
            if (isPaused == true || Cursorvisible)
                Cursor.visible = true;
            else
                Cursor.visible = false;
        }
    }
    public Vector2 GetBossRoomLocation()
    {
        return BossRoomLocation;
    }
    //remove all interaction popups
    public void DestroyPopUpsWithTag(string Tag)
    {
        GameObject[] pickups = GameObject.FindGameObjectsWithTag(Tag);
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i].GetComponent<InteractPopup>() != null)
                pickups[i].GetComponent<InteractPopup>().DestroyPopUp();
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeTimeScale(1, 0);
    }
    //This pauses game
    public void PauseGame()
    {
        //Pauses Game
        AudioManager.instance.StopAllSounds();
        StateManager.instance.DestroyPopUpsWithTag("Pickups");
        StateManager.instance.DestroyPopUpsWithTag("Interactable");
        isPaused = true;
        Time.timeScale = 0f;
        canvas.transform.Find("BlackUnderUI").GetComponent<Image>().enabled = true;
    }
    //pass true to pause the game or false to unpause
    public void Pause(bool truthy)
    {
        if (truthy)
            PauseGame();
        else
            UnpauseGame();
    }
    //This unpauses game
    public void UnpauseGame()
    {
        if (Player.instance != null) //if there is a player
        { 
            if (Player.instance.IsInteracting() == true)
                return; //don't unpause if the player is interacting
        }
        //UnPauses Game
        if (pauseMenu != null)
		{
            print("unpausing");
			if (isPaused == true)
			{
				canvas.transform.Find("BlackUnderUI").GetComponent<Image>().enabled = false;
			}
			pauseMenu.SetActive(false);
		}

		isPaused = false;
        Time.timeScale = TimeScale;
        Time.fixedDeltaTime = 0.02f * TimeScale;
    }
    public bool IsPaused()
    {
        return isPaused;
    }
    public void ChangeTimeScale(float timescale, float time)
    {
        TimeScale = timescale;
        Timer = time;
        print(isPaused);//this better be false
        if (!isPaused && TimeScale > .9f) //only set timescale if you're not already slowed
        {
            Time.timeScale = TimeScale;
            Time.fixedDeltaTime = 0.02f * TimeScale;
        }
        if (TimeScale < .6f) 
        {
            timeslowEffect.Play("TimeSlowOn");
            timeSlowEffectActive = true;
        }
    }
    public void LoadMenu()
    {
        
        StartCoroutine(LoadSceneCoroutine());
        //SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

	public void LoadCredits()
	{
		StartCoroutine(LoadCreditsSceneCoroutine());
	}

	/* when given a transform, this recursively disables all objects below it, excluding one
     * This is currently used to turn off the canvas without ruining other things / needing to make a new canvas
     */
	public void SetActiveExcludingChild(Transform t, string excludedName, bool enabled)
    {
        foreach (Transform child in t)
        {
            if (child.name != excludedName)
                child.gameObject.SetActive(enabled);
        }
    }
    public void TintScreenForDuration(Color color, float duration, float tweenduration, float endAlpha)
    {
        print("tint func runs");
        Image overlay = canvas.transform.Find("BlackUnderUI").GetComponent<Image>();
        overlay.color = new Color(color.r, color.g, color.b, 0);
        this.tweenduration = tweenduration;
        overlay.enabled = true;
        overlay.DOFade(endAlpha, tweenduration);
        Invoke("FadeOut", duration-tweenduration);
    }
    private void FadeOut()
    {
        Image overlay = canvas.transform.Find("BlackUnderUI").GetComponent<Image>();
        overlay.DOFade(0, tweenduration);
    }
    public void ToggleOptionsMenu(bool toggle)
    {
        //pauseMenu.SetActive(!toggle);
        optionsMenu.SetActive(toggle);
    }
    //returns the area the player is in: 0 for scrap yard, 1 for refinery, 2 for city, and 3 for boss room
    public int GetCurrentPlayerLevel()
    {
        if (Player.instance.transform.position.x < -200)
            return 3; // player is in boss room
        int level = Mathf.FloorToInt(Player.instance.transform.position.x / TileManager.instance.levelSpacing);
        Mathf.Clamp(level, 0, 3);
        return level;
    }
    public void Teleport(Vector2 destination, bool bossRoomOverride, bool hasPair , string name)
    {
        canvas.transform.Find("BlackOverlay").GetComponent<Animation>().Play();
        if (MayhemTimer.instance != null && TileManager.instance.MayhemMode)
            MayhemTimer.instance.LevelCleared();
        Camera cam = Camera.main;
        cam.GetComponentInParent<CinemachineBrain>().enabled = false;
        cam.transform.position = new Vector3(player.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        //GameObject.FindGameObjectWithTag("CMcam").GetComponent<CinemachineVirtualCamera>().enabled = false;
        TileManager.instance.GetComponent<TileManager>().mayhemLevelUp();
        if (name.Contains("Skip"))
            SkipTutorial();
        AudioManager.instance.PlaySound("SFX_Teleport");
        if (!hasPair) //if the teleporter doesn't already have a pair
        {
            GameObject newTele = Instantiate(TileManager.instance.interactablePrefabs[4]) as GameObject;//spawn a teporter where ever the teleporter takes you
            newTele.transform.position = destination + new Vector2(-4, 4);
            newTele.GetComponent<Teleporter>().SetDestination(player.transform.position + new Vector3(4, 0, 0));
            newTele.GetComponent<Teleporter>().hasPair = true;
        }
        if (TileManager.instance.MayhemMode) //if it doesn't have pair, and you are in mayhem mode
        {
            MayhemTimer.instance.IncrementClearedLevels();
            EnemyManager.instance.ModifyDifficultyMulti(.4f); //mayhem mode increase
        }
        else //if it doesnt have pair and you are in normal mode
        {
            EnemyManager.instance.ModifyDifficultyMulti(.4f); //regular mode
        }
        //Destroy(fakeTele.GetComponent<Teleporter>());
        StateManager.instance.InTutorial = false;
        Player.instance.transform.position = new Vector3(destination.x, destination.y, 0);
        int level = StateManager.instance.GetCurrentPlayerLevel();
        if (bossRoomOverride == true)
        {
            Player.instance.transform.position = GetBossRoomLocation();
            player.GetComponent<UniqueEffects>().CombatChangesMusic = false;
            level = 3;
        }
        Player.instance.Heal(50);
        Camera.main.GetComponentInParent<CinemachineBrain>().enabled = true;
        BUGE.instance.transform.position = player.transform.position;
        BUGE.instance.FollowingPlayer(true);
        BUGE.instance.GetComponent<InteractPopup>().SetText("Right Click to Interact");
        BUGE.instance.GetComponent<InteractPopup>().DestroyPopUp();
        AudioManager.instance.PlaySongsForLevel(level);
        TileManager.instance.playerOnlevel++;
    }
    public void SkipTutorial()
    {
        SkillManager sk = Player.instance.GetSkillManager();
        sk.AddSkill(sk.GetRandomAbility());
        while (sk.GetRandomGoldsFromPlayer(1).Count < 1 && sk.GetPlayerMods().Count < 5) // if the player has a legendary OR 4 mods, stop rolling
            sk.AddSkill(sk.GetRandomModsByChance(1)[0]);
        DialogueManager.instance.TriggerDialogue("BUG-E_SkipTut");
        Destroy(gameObject.GetComponent<FindingBuge>());
        BUGE buge = BUGE.instance;
        buge.GetComponent<InteractPopup>().SetInteractable(false);
        buge.GetComponent<InteractPopup>().DestroyPopUp();
        buge.GetComponent<InteractPopup>().SetText("Right Click to Interact");
    }
    public void RecursiveLayerChange(Transform t, string layerName)
    {
        foreach (Transform child in t)
        {
            this.gameObject.layer = LayerMask.NameToLayer(layerName);
            if (child.childCount > 0)
            {
                RecursiveLayerChange(child, layerName);
            }
        }
    }

    IEnumerator LoadSceneCoroutine()
    {
        operation = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

	IEnumerator LoadCreditsSceneCoroutine()
	{
		operation = SceneManager.LoadSceneAsync("Credits", LoadSceneMode.Single); // Change this later to the credits
		operation.allowSceneActivation = false;
		while (!operation.isDone)
		{
			if (operation.progress >= 0.9f)
			{
				operation.allowSceneActivation = true;
			}
			yield return null;
		}
	}
}
