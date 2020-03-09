using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Kenny Doan
 * SkillUIElement is a script that handles the display of the skill it should have
 */
public class SkillUIElement : MonoBehaviour
{
    public Image Icon;
    public SkillObject Skill;
    public int StackNumber;
    public Color iColor = Color.white;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Image>() && Icon != null)
        {
            GetComponent<Image>().sprite = Icon.sprite;
        }
    }
    public void UpdateSkillUIElement(Sprite icon, int stacknumber, SkillObject skill, Color color)
    {
        Icon.sprite = icon;
        iColor = color;
        GetComponent<Image>().sprite = icon;
        GetComponent<Image>().color = color;
        StackNumber = stacknumber;
        if (stacknumber > 1)
        {
            GetComponentInChildren<Text>().text = "x" + stacknumber;
        }
        else
        {
           if (GetComponentInChildren<Text>() != null)
                GetComponentInChildren<Text>().text = "";
        }
        Skill = skill;
    }
    public void SetStack(int num)
    {
        UpdateSkillUIElement(Icon.sprite, num, Skill, iColor);
    }

    public void AddStack(int num)
    {
        UpdateSkillUIElement(Icon.sprite, StackNumber + num, Skill, iColor);
    }
    public void SetColor(Color color)
    {
        UpdateSkillUIElement(Icon.sprite, StackNumber, Skill, color);
    }
    public void SetIcon(Sprite icon)
    {
        UpdateSkillUIElement(icon, StackNumber, Skill, iColor);
    }
    public void SetSkill(SkillObject skill)
    {
        UpdateSkillUIElement(Icon.sprite, StackNumber,skill, iColor);
    }
}
