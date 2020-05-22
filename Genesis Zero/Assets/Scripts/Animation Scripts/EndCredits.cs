using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class EndCredits : MonoBehaviour
{
    // Start is called before the first frame update

    public Color DefaultTextColor = Color.white;
    public Color DefaultHighlightTextColor = Color.red;
    public float DefaultShowDuration = 4f;
    public float DefaultFadeDuration = .7f;
    public float DefaultTextInactiveDuration = .7f;

    private Queue<CreditCard> cardQueue;
    private Color overlay;
    private float fadeDuration;
    private TextMeshProUGUI tmp;
    private TextMeshProUGUI tmp2;
    private Image image;

    private void Start()
    {
        cardQueue = new Queue<CreditCard>();
        CreditCard[] objs = Resources.LoadAll<CreditCard>("CreditCards");
        for (int i = 0; i < objs.Length; i++)
        {
            cardQueue.Enqueue(objs[i]);
        }
        tmp = transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        tmp2 = transform.Find("RoleTxt").gameObject.GetComponent<TextMeshProUGUI>();
        transform.Find("Overlay").GetComponent<Image>().enabled = true;
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
            tmp.text = cardQueue.Peek().Name;
            tmp.color = cardQueue.Peek().MainTextColor;
            tmp2.text = cardQueue.Peek().Role;
            tmp2.color = new Color(0.0f, 0.8f, 1.0f, 1.0f);
            fadeDuration = cardQueue.Peek().FadeDuration;
            float inactiveDuration = cardQueue.Peek().TextInactiveDuration;
            float durationOnCard = cardQueue.Peek().ShowDuration;
            if (fadeDuration == 0)
                fadeDuration = DefaultFadeDuration;
            if (inactiveDuration == 0)
                inactiveDuration = DefaultTextInactiveDuration;
            if (durationOnCard == 0)
                durationOnCard = DefaultShowDuration;
            cardQueue.Dequeue();
            FadeIn();
            Invoke("FadeOut", fadeDuration + durationOnCard);
            Invoke("NextCard", fadeDuration + durationOnCard + fadeDuration + inactiveDuration);

        }
    }
    void Update()
    {
        transform.Find("Overlay").gameObject.GetComponent<Image>().color = overlay;
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitCredits();
    }
    //public string SplitText()
    //fade in refers to the text becoming visible, even though the overlay is technically fading out
    public void FadeIn()
    {
        DOTween.To(() => overlay.a, x => overlay.a = x, 0, fadeDuration);
    }
    //fade out refers to the text becoming covered by the overlay
    public void FadeOut()
    {
        DOTween.To(() => overlay.a, x => overlay.a = x, 1, fadeDuration);
    }
    public void ExitCredits()
    {
        CancelInvoke();
        cardQueue.Clear();
        gameObject.SetActive(false);
        //gameObject.transform.parent.Find("BlackOverlay").GetComponent<SpriteFade>().FadeOut(4f);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

    }
}
