using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class Intro : MonoBehaviour
{
    // Start is called before the first frame update

    public Color DefaultTextColor = Color.white;
    public Color DefaultHighlightTextColor = Color.red;
    public float DefaultShowDuration = 4f;
    public float DefaultFadeDuration = .7f;
    public float DefaultTextInactiveDuration = .7f;

    private Queue<IntroCard> cardQueue;
    private Color overlay;
    private float fadeDuration;
    private TextMeshProUGUI tmp;
    private Image image;
    private void Start()
    {
        cardQueue = new Queue<IntroCard>();
        if (SaveLoadManager.instance.newGame == true)
        {

            IntroCard[] objs = Resources.LoadAll<IntroCard>("IntroCards");
            for (int i = 0; i < objs.Length; i++)
            {
                cardQueue.Enqueue(objs[i]);
            }
            tmp = transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
            transform.Find("Overlay").GetComponent<Image>().enabled = true;
            NextCard();
            GameInputManager.instance.DisablePlayerControls();
        }
        else { EndIntro();}
    }
    public void NextCard()
    {
        if (cardQueue.Count < 1)
        {
            EndIntro();
            return;
        }
        else
        {
            tmp.text = cardQueue.Peek().Text;
            tmp.color = cardQueue.Peek().MainTextColor;
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
            EndIntro();
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
    public void EndIntro()
    {
        CancelInvoke();
        cardQueue.Clear();
        gameObject.SetActive(false);
        gameObject.transform.parent.Find("BlackOverlay").GetComponent<SpriteFade>().FadeOut(4f);
        if (Camera.main.GetComponentInParent<CutsceneController>())
        {
            if (SaveLoadManager.instance.newGame == true)
                Camera.main.GetComponentInParent<CutsceneController>().IntroCutscene();
            else
                Camera.main.GetComponentInParent<CutsceneController>().Reset();
        }

    }
}
