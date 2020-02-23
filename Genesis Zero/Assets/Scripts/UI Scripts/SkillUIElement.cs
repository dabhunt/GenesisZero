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
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Image>())
        {
            GetComponent<Image>().sprite = Icon.sprite;
        }
    }
    public void UpdateSkillUIElement(Sprite icon, int stacknumber, SkillObject skill)
    {
        Icon.sprite = icon;
        GetComponent<Image>().sprite = icon;
        StackNumber = stacknumber;
        if (stacknumber > 1)
        {
            GetComponentInChildren<Text>().text = "x" + stacknumber;
        }
        else
        {
            GetComponentInChildren<Text>().text = "";
        }
        Skill = skill;
        
    }
    public void SetStack(int num)
    {
        UpdateSkillUIElement(Icon.sprite, num, Skill);
    }

    public void AddStack(int num)
    {
        UpdateSkillUIElement(Icon.sprite, StackNumber + num, Skill);
    }

    public void SetIcon(Sprite icon)
    {
        UpdateSkillUIElement(icon, StackNumber, Skill);
    }
    public void SetSkill(SkillObject skill)
    {
        UpdateSkillUIElement(Icon.sprite, StackNumber, Skill);
    }
}
