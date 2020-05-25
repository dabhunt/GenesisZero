﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Diagnostics;
using System.ComponentModel;

public class EndCredits : MonoBehaviour
{
    public float DefaultShowDuration = 4f;
    public float DefaultFadeDuration = .7f;
    public float DefaultTextInactiveDuration = .7f;
    public List<GameObject> CreditList;
    public List<float> CreditDurations;
    public List<float> CreditFades;
    public List<float> CreditDelays;
    public List<bool> ShowCreditBG;

    private Queue<GameObject> cardQueue;
    private int count = 0;
    private UnityEngine.Component[] allCredits;
    private Image bg;

    // Start is called before the first frame update
    private void Start()
    {
        // reference to blackbackground
        bg = GameObject.Find("BlackOverlay").GetComponent<Image>();

        // Queue up credits
        cardQueue = new Queue<GameObject>();
        foreach (GameObject s in CreditList)
        {
            cardQueue.Enqueue(s);
        }

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

        // Set default bg settings
        while (ShowCreditBG.Count < CreditList.Count)
        {
            ShowCreditBG.Add(false);
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
            // make credit active and determine presence of colored background
            CreditList[count].SetActive(true);
            ShowBG();

            // find all TextMeshPro components in a credit's children
            allCredits = CreditList[count].GetComponentsInChildren(typeof(TextMeshProUGUI));
            foreach (TextMeshProUGUI txt in allCredits)
            {
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0f);
            }

            // fading and queueing sections
            FadeIn();
            Invoke("FadeOut", CreditFades[count] + CreditDurations[count]);
            Invoke("NextCard", CreditFades[count] + CreditDurations[count] + CreditFades[count] + CreditDelays[count]);
        }
    }
    void Update()
    {
        //transform.Find("Overlay").gameObject.GetComponent<Image>().color = overlay;
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitCredits();
    }

    // to be invoked once credit done fading out
    private void DisableCredit()
    {
        cardQueue.Dequeue().SetActive(false);
        count++;
    }
    private void ShowBG()
    {
        // shows background if checked
        if (ShowCreditBG[count])
        {
            if (bg.color.a == 0)
                bg.DOFade(1f, CreditFades[count]);
        }
        else
            if (bg.color.a > 0)
            bg.DOFade(0f, CreditFades[count]);
    }

    private void FadeIn()
    {
        //DOTween.To(() => overlay.a, x => overlay.a = x, 0, CreditFades[count]);

        foreach (TextMeshProUGUI txt in allCredits)
        {
            txt.DOFade(1f, CreditFades[count]);
        }
    }
    private void FadeOut()
    {
        //DOTween.To(() => overlay.a, x => overlay.a = x, 1, CreditFades[count]);

        foreach (TextMeshProUGUI txt in allCredits)
        {
            txt.DOFade(0f, CreditFades[count]);
        }
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
