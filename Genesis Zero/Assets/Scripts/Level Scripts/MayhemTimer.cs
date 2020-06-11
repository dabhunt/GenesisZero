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
    public TextMeshProUGUI levelsClearedText;
    public TextMeshProUGUI difficultytxt;
    public bool Active = false;
    public bool ActiveOnStart = false;
    public float decreasePercent = .9f;//how much time between difficulty bumps decreases each difficulty bump
    private string Strformat = "0.#";
    private int levelsCleared = 0;
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
        curTimeLeft = resetTime;
        levelsClearedText.text = "Levels Cleared: " + levelsCleared.ToString();
        if (ActiveOnStart)
            Active = true;
        else
        {
            Invoke("DelayedStart", 1f);
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
    public void LevelCleared()
    {
        levelsCleared++;
        string display;
        if (levelsCleared == 1)
            display = "1 Level Cleared";
        else 
        {
            display = levelsCleared.ToString() + " Levels Cleared ";
        }
        levelsClearedText.text = display;
        //difficultytxt.text = "Difficulty: " + difficulty.ToString();
        TileManager.instance.tempTextDisplay.ShowText(display , 1, .75f);
    }

    public float[] GetData()
    {
        float[] data = new float[3];
        data[0] = EnemyManager.Difficulty;//Difficulty
        data[1] = levelsCleared;//Levels cleared
        data[2] = curTimeLeft;//Time left
        return data;
    }

    public void ApplyData()
    {
        Debug.Log("Applying Endless Data");
        EndlessData eData = SaveLoadManager.instance.LoadEndlessData();
        EnemyManager.instance.SetDifficulty(eData.data[0]);
        levelsCleared = (int) eData.data[1];
        curTimeLeft = eData.data[3];
    }
    public int GetLevelsCleared() 
    { return levelsCleared; }
    public void IncrementClearedLevels()
    {
        levelsCleared++;
    }
    public void BumpDifficulty()
    {
        EnemyManager.instance.AddDifficulty(.6f);
        //EnemyManager.instance.ModifyDifficultyMulti(.6f);
        TileManager.instance.tempTextDisplay.ShowText("Difficulty Increased!", 1 , .75f);
        curTimeLeft = resetTime * decreasePercent;
        curTimeLeft = Mathf.Clamp(curTimeLeft, 30, 60);
        countdownText.text = curTimeLeft.ToString(Strformat);
    }
}
