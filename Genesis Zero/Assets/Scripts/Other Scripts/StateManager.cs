using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public GameObject pauseMenu;
    private AsyncOperation operation;
    private GameObject optionsMenu;
    private GameObject pMenuButtons;
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
        GameObject temp = GameObject.FindWithTag("Player");
        player = temp.GetComponent<Player>();
        temp = GameObject.FindWithTag("StateManager");
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
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                UnpauseGame();
            }
            else
            {
                pauseMenu.SetActive(true);
                PauseGame();
            }
        }
        //temporary cheat codes to get mods
        if (Input.GetKey(KeyCode.Backslash))
        {
            string skillStr = player.GetSkillManager().GetRandomMod().name;
            player.GetSkillManager().SpawnMod(new Vector3(player.transform.position.x+2, player.transform.position.y+5, 0), skillStr);
        }
        if (Input.GetKey(KeyCode.End))
        {
            player.GetHealth().SetValue(0);
        }
        if (Input.GetKeyDown(KeyCode.Home))
        {
            player.SetEssence(player.GetMaxEssenceAmount());
            player.SetKeys(3);
            player.GetHealth().SetMaxValue(2000);
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            Merchant closestMerchant = InteractInterface.instance.ClosestInteractable().GetComponent<Merchant>();
            if (closestMerchant != null && closestMerchant.GetWindowOpen())
                InteractInterface.instance.ClosestInteractable().GetComponent<Merchant>().InitializeUI();
        }
        //get to next level instantly
        if (Input.GetKey(KeyCode.ScrollLock))
        {
            int levelNum = TileManager.instance.curlevel;
            float levelSpacing = TileManager.instance.levelSpacing;
            float rng = Random.Range(1.0f, 4.0f) / 10;
            Vector2 destination = new Vector2((levelNum+1) * levelSpacing + levelSpacing * rng, 70f);
            Vector2 colliderPos = GameObject.FindWithTag("CamCollider").transform.position;
            GameObject.FindWithTag("CamCollider").transform.position = new Vector2(colliderPos.x + levelSpacing+100, colliderPos.y);
            GameObject.FindWithTag("Player").transform.position = destination;
            TileManager.instance.curlevel++;

        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            List<SkillObject> list = player.GetSkillManager().GetAllAbilities();
            for (int i = 0; i < list.Count; i++)
            {
                Vector3 newVec = new Vector3(player.transform.position.x + i * 3f, player.transform.position.y + 1, 0); 
                player.GetSkillManager().SpawnAbility(newVec, list[i].name);
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) 
        {
            restart.RestartScene();
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
        FindObjectOfType<AudioManager>().StopAllSounds();
        isPaused = true;
        Time.timeScale = 0f;
        canvas.transform.Find("BlackUnderUI").GetComponent<Image>().enabled = true;
    }

    //This unpauses game
    public void UnpauseGame()
    {
        //UnPauses Game
        isPaused = false;
        Time.timeScale = TimeScale;
        Time.fixedDeltaTime = 0.02f * TimeScale;
        if (pauseMenu != null)
        {
            canvas.transform.Find("BlackUnderUI").GetComponent<Image>().enabled = false;
            pauseMenu.SetActive(false);
        }
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

    public void ToggleOptionsMenu(bool toggle)
    {
        pauseMenu.SetActive(!toggle);
        optionsMenu.SetActive(toggle);
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
}
