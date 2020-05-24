using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Diagnostics;

public class EndCredits : MonoBehaviour
{
    // Start is called before the first frame update
    public float DefaultShowDuration = 4f;
    public float DefaultFadeDuration = .7f;
    public float DefaultTextInactiveDuration = .7f;
    public List<GameObject> CreditList;
    public List<float> CreditDurations;
    public List<float> CreditFades;
    public List<float> CreditDelays;

    private Queue<GameObject> cardQueue;
    private Color overlay;
    private int count = 0;

    private void Start()
    {
        // Queue up credits
        cardQueue = new Queue<GameObject>();
        foreach (GameObject s in CreditList)
        {
            cardQueue.Enqueue(s);
        }
        transform.Find("Overlay").GetComponent<Image>().enabled = true;

        // Set default duration credit is shown
        for (int i = 0; i < CreditDurations.Count; i++)
        {
            if(CreditDurations[i] <= 0)
                CreditDurations[i] = DefaultShowDuration;
        }
        while (CreditDurations.Count < CreditList.Count)
        {
            CreditDurations.Add(DefaultShowDuration);
        }

        // Set default fade time
        for (int i = 0; i < CreditFades.Count; i++)
        {
            if (CreditFades[i] <= 0)
                CreditFades[i] = DefaultFadeDuration;
        }
        while (CreditFades.Count < CreditList.Count)
        {
            CreditFades.Add(DefaultFadeDuration);
        }

        // Set default delay between credits
        for (int i = 0; i < CreditDelays.Count; i++)
        {
            if (CreditDelays[i] <= 0)
                CreditDelays[i] = DefaultTextInactiveDuration;
        }
        while (CreditDelays.Count < CreditList.Count)
        {
            CreditDelays.Add(DefaultTextInactiveDuration);
        }

        NextCard();
        if(GameInputManager.instance != null)
            GameInputManager.instance.DisablePlayerControls();
    }
    public void NextCard()
    {
        if (cardQueue.Count < 1)
        {
            ExitCredits();
            return;
        }
        else
        {
            CreditList[count].SetActive(true);
            FadeIn();
            Invoke("FadeOut", CreditFades[count] + CreditDurations[count]);
            Invoke("NextCard", CreditFades[count] + CreditDurations[count] + CreditFades[count] + CreditDelays[count]);
        }
    }
    void Update()
    {
        transform.Find("Overlay").gameObject.GetComponent<Image>().color = overlay;
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitCredits();
    }

    private void DisableCredit()
    {
        cardQueue.Dequeue().SetActive(false);
        count++;
    }

    //fade in refers to the text becoming visible, even though the overlay is technically fading out
    public void FadeIn()
    {
        DOTween.To(() => overlay.a, x => overlay.a = x, 0, CreditFades[count]);
    }
    //fade out refers to the text becoming covered by the overlay
    public void FadeOut()
    {
        DOTween.To(() => overlay.a, x => overlay.a = x, 1, CreditFades[count]);
        Invoke("DisableCredit", CreditFades[count]);
    }

    public void ExitCredits()
    {
        CancelInvoke();
        cardQueue.Clear();
        gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
