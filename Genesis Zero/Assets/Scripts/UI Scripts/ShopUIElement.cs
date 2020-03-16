using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIElement : MonoBehaviour
{
    public Image Icon;
    public string Name;
    public int Cost;
    public ShopObject ItemObj;
    public SkillObject Skill;
    void Start()
    {
        if (GetComponent<Image>() && Icon != null)
        {
            GetComponent<Image>().sprite = Icon.sprite;
        }

    }
    public void UpdateShopUIElement(string name, Sprite icon, SkillObject skill, int cost)
    {
        Name = name;
        Icon.sprite = icon;
        Cost = cost;
        transform.Find("Icon").gameObject.GetComponent<Image>().sprite = icon;
        transform.Find("Name").gameObject.GetComponent<Text>().text = name;
        transform.Find("Cost").gameObject.GetComponent<Text>().text = cost.ToString();
        Skill = skill;
    }
    public void SetName(string name)
    {
        UpdateShopUIElement(name,Icon.sprite,Skill, Cost);
    }
    public void SetIcon(Sprite icon)
    {
        UpdateShopUIElement(Name,icon, Skill, Cost);
    }
    public void SetSkill(SkillObject skill)
    {
        UpdateShopUIElement(Name,Icon.sprite, skill, Cost);
    }
    public void SetCost(int cost)
    {
        UpdateShopUIElement(Name,Icon.sprite,Skill,cost);
    }
    public void PurchaseItem()
    {
        transform.Find("PurchasedOverlay").gameObject.SetActive(true);
    }
    public void SelectItem()
    {
        transform.Find("Select").gameObject.SetActive(true);
    }
}

