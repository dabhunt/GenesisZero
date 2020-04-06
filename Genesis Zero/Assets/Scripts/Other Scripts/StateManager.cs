using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        if (Input.GetKeyDown(KeyCode.Home))
        {
            player.SetEssence(player.GetMaxEssenceAmount());
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            Merchant closestMerchant = GetComponent<InteractInterface>().ClosestInteractable().GetComponent<Merchant>();
            if (closestMerchant != null && closestMerchant.GetWindowOpen())
                GetComponent<InteractInterface>().ClosestInteractable().GetComponent<Merchant>().InitializeUI();
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
    }

    //This unpauses game
    public void UnpauseGame()
    {
        //UnPauses Game
        isPaused = false;
        Time.timeScale = TimeScale;
        Time.fixedDeltaTime = 0.02f * TimeScale;
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
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
}
