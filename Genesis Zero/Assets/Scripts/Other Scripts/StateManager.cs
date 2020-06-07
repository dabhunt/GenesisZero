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
        GameObject temp = Player.instance.gameObject;
        player = Player.instance;
        if (!SaveLoadManager.instance.newGame)
        {
            PlayerData pData = SaveLoadManager.instance.LoadPlayerData();
            //MapData mData = SaveLoadManager.instance.LoadMapData();
            SaveLoadManager.instance.ApplyPlayerData(pData, temp);
        }

        temp = StateManager.instance.gameObject;
        restart = temp.GetComponent<Restart>();
        canvas = GameObject.FindWithTag("CanvasUI");
        pauseMenu = canvas.transform.Find("PauseMenu").gameObject;
        optionsMenu = canvas.transform.Find("OptionsScreen").gameObject;
    }
    private void Update()
    {
        if (Timer >= 0)
        {
            Timer -= Time.unscaledDeltaTime;
            if (Timer < 0)
            {
                ChangeTimeScale(1, 0);
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
        if (isPaused == true || Cursorvisible)
            Cursor.visible = true;
        else
            Cursor.visible = false;
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
            if (Player.instance.IsInteracting == true)
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
        if (!isPaused)
        {
            Time.timeScale = timescale;
            Time.fixedDeltaTime = 0.02f * TimeScale;
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
    public void Teleport(Vector2 destination, bool bossRoomOverride)
    {
        GameObject fakeTele = Instantiate(TileManager.instance.interactablePrefabs[4]) as GameObject;//spawn a fake teleporter where ever the teleporter takes you
        fakeTele.transform.position = destination;
        Destroy(fakeTele.GetComponent<Teleporter>());
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
        EnemyManager.ModifyDifficultyMulti(1.7f);
        Camera.main.GetComponentInParent<CinemachineBrain>().enabled = true;
        BUGE.instance.transform.position = player.transform.position;
        BUGE.instance.FollowingPlayer(true);
        AudioManager.instance.PlaySongsForLevel(level);
        TileManager.instance.playerOnlevel++;
    }
    public void RecursiveLayerChange(Transform t, string layerName)
    {
        foreach (Transform child in t)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Dead");
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
