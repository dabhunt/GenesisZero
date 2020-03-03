using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;
    private float TimeScale;
    private float Timer;
    private Player player;
    private bool IsPaused;

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
    }

    private void Update()
    {
        if (Timer > 0)
        {
            Timer -= Time.unscaledDeltaTime;
            if (Timer <= 0)
            {
                ChangeTimeScale(1, 0);
                Timer = 0;
            }
        }
        //temporary input usage for demo tomorrow
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        //temporary input usage for demo tomorrow
        if (Input.GetKey(KeyCode.Backslash))
        {
            string skillStr = player.GetSkillManager().GetRandomSkill().name;
            player.GetSkillManager().SpawnMod(new Vector3(player.transform.position.x+2, player.transform.position.y+5, 0), skillStr);
        }

    }

    //This pauses game
    public void PauseGame()
    {
        //Pauses Game
        Debug.Log("Pausing Game");
        IsPaused = true;
        Time.timeScale = 0;
    }

    //This unpauses game
    public void UnpauseGame()
    {
        //UnPauses Game
        Debug.Log("UnPausing Game");
        IsPaused = false;
        Time.timeScale = TimeScale;
        Time.fixedDeltaTime = 0.02f * TimeScale;
    }
    
    public void ChangeTimeScale(float timescale, float time)
    {
        TimeScale = timescale;
        Timer = time;
        if (!IsPaused)
        {
            Time.timeScale = timescale;
            Time.fixedDeltaTime = 0.02f * TimeScale;
        }
    }
}
