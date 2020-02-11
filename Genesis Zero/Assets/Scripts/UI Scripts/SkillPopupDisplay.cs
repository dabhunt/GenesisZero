using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPopupDisplay : MonoBehaviour
{
    public Image Icon;
    public Text Name;
    public Text Text;
    public Image BG;
    public float PopupTime;
    private float currpoptime;
    public float fadetime;
    private float totalfadetime;
    public float delay;
    private float totaldelay;

    void Start()
    {
        totalfadetime = fadetime;
        totaldelay = delay;
        currpoptime = 0;
        fadetime = 0;
        SetAlpha(0);
    }

    void FixedUpdate()
    {
        if (delay > 0)
        {
            SetAlpha(1 - (delay / totaldelay));
            delay -= Time.fixedDeltaTime;
        }
        else if (currpoptime > 0)
        {
            SetAlpha(1);
            currpoptime -= Time.fixedDeltaTime;
        }
        else
        {
            currpoptime = 0;
            if (fadetime > 0)
            {
                SetAlpha(fadetime / totalfadetime);

                fadetime -= Time.fixedDeltaTime;
            }
            else
            {
                SetAlpha(0);
            }
        }
    }

    public void SetPopup(Sprite icon, string name, string text)
    {
        Icon.sprite = icon;
        Name.text = name;
        Text.text = text;
    }

    public void Popup()
    {
        delay = totaldelay;
        currpoptime = PopupTime;
        fadetime = totalfadetime;
    }

    public void SetAlpha(float alpha)
    {
        Mathf.Clamp(alpha, 0, 1);
        
        Color iconcolor = Icon.color;
        iconcolor.a = alpha;
        Icon.color = iconcolor;

        Color namecolor = Name.color;
        namecolor.a = alpha;
        Name.color = namecolor;

        Color textcolor = Text.color;
        textcolor.a = alpha;
        Text.color = textcolor;

        Color BGcolor = BG.color;
        BGcolor.a = alpha;
        BG.color = BGcolor;
    }
}
