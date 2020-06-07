using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MayhemTimer : MonoBehaviour
{
    public static MayhemTimer instance;
    public float curTimeLeft;
    public float resetTime;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI levelsClearedText;
    public bool Active = false;
    public bool ActiveOnStart = false;
    public float decreasePercent = .9f;//how much time between difficulty bumps decreases each difficulty bump
    private string Strformat = "0.#";
    private float levelsCleared = 0;
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
        if (ActiveOnStart)
            Active = true;
        else
        {
            Active = false;
            this.gameObject.SetActive(false);//turn it off if active on start is false
        }
    }
    void Start()
    {
        curTimeLeft = resetTime;
        levelsClearedText.text = "Levels Cleared: " + levelsCleared.ToString();
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
        levelsClearedText.text = "Levels Cleared: "+ levelsCleared.ToString();
    }
    public void BumpDifficulty()
    {
        EnemyManager.ModifyDifficultyMulti(1.2f);
        curTimeLeft = resetTime * decreasePercent;
        countdownText.text = curTimeLeft.ToString(Strformat);
    }
}
