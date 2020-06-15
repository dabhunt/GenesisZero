using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MayhemTimer : MonoBehaviour
{
    public static MayhemTimer instance;
    public float curTimeLeft;
    public float resetTime = 105;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI difficultytxt;
    public TextMeshProUGUI curDifficultytxt;
    public bool Active = false;
    public bool ActiveOnStart = false;
    public float decreasePercent = .9f;//how much time between difficulty bumps decreases each difficulty bump
    private string Strformat = "0.#";
    private int killCount = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (!SaveLoadManager.instance.newGame && SaveLoadManager.instance.endLess)
        {
            ApplyData();
        }
        else
        {
            curTimeLeft = resetTime;
        }
        killCountText.text = "Enemies Killed: " + killCount.ToString();
        curDifficultytxt.text = "Current Difficulty: " + EnemyManager.Difficulty.ToString(Strformat);
        if (ActiveOnStart)
        {
            Active = true;

        }
        else
        {
            Active = false;
            Invoke("DelayedStart", 1f);
            return;
        }
    }
    void DelayedStart()
    {
        Active = false;
        this.gameObject.SetActive(false);//turn it off if active on start is false
    }
    void Update()
    {
        if (Active)
        {
            curTimeLeft = Mathf.Clamp(curTimeLeft -= Time.deltaTime, 0, resetTime);
            countdownText.text = curTimeLeft.ToString(Strformat);
            if (curTimeLeft <= 0)
            {
                BumpDifficulty(); //increase difficulty and reset countdown
            }
        }
    }
    public void EnemyKilled()
    {
        killCount++;
        string display = "Enemies Killed: " + killCount.ToString();
        killCountText.text = display;
        curDifficultytxt.text = "Current Difficulty: " + EnemyManager.Difficulty.ToString(Strformat);
        //difficultytxt.text = "Difficulty: " + difficulty.ToString();
    }

    public float[] GetData()
    {
        float[] data = new float[4];
        data[0] = EnemyManager.Difficulty;//Difficulty
        data[1] = killCount;//Enemies Killed
        data[2] = curTimeLeft;//Time left
        data[3] = TileManager.tileID;
        return data;
    }

    public void ApplyData()
    {
        Debug.Log("Applying Endless Data");
        EndlessData eData = SaveLoadManager.instance.LoadEndlessData();
        EnemyManager.instance.SetDifficulty(eData.data[0]);
        //Debug.Log("Difficulty: " + EnemyManager.Difficulty);
        killCount = (int) eData.data[1];
        curTimeLeft = eData.data[2];
        TileManager.tileID = (int) eData.data[3];
    }
    public int GetkillCount() 
    { return killCount; }
    public void BumpDifficulty()
    {
        EnemyManager.instance.AddDifficulty(.3f);
        //EnemyManager.instance.ModifyDifficultyMulti(.6f);
        TileManager.instance.tempTextDisplay.ShowText("Difficulty Increased!", 1 , .75f);
        curDifficultytxt.text = "Current Difficulty: " + EnemyManager.Difficulty.ToString(Strformat);
        curTimeLeft = resetTime * decreasePercent;
        curTimeLeft = Mathf.Clamp(curTimeLeft, 30, 60);
        countdownText.text = curTimeLeft.ToString(Strformat);
    }
}
