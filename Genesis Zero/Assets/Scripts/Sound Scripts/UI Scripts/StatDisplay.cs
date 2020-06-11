using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public static StatDisplay instance;
    private GameObject canvas;
    //public Color adHover = Color.red;
    //public Color apHover = Color.blue;
    //public Color asHover = Color.white;
    //public Color ccHover = Color.yellow;
    public TextMeshProUGUI AD;
    public TextMeshProUGUI AP;
    public TextMeshProUGUI AS;
    public TextMeshProUGUI CC;
    private bool Abbreviations = false;
    private bool Hidden = true;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
       ToggleHideStats();
    }
    public void ToggleHideStats()
    {
        Hidden = !Hidden;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!Hidden);
        }
        transform.Find("Display").gameObject.SetActive(Hidden);
        if (Hidden)
        {
            this.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        else
        {
            this.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
    public void UpdateStats()
    {
        if (Hidden)
            return;
        float _bd = Player.instance.GetDamage().GetValue() * 10;
        float _ap = Player.instance.GetAbilityPower().GetValue() * 10;
        float _as = Player.instance.GetAttackSpeed().GetValue();
        float _cc = Player.instance.GetCritChance().GetValue() * 100;
        if (Abbreviations)
        {
            AD.SetText("B.D.: " + (int)_bd);
            AP.SetText("A.P.: " + (int)_ap);
            AS.SetText("A.S.: " + (Mathf.Round(_as * 100)) / 100.0);
            CC.SetText("Crit: " + (Mathf.Round(_cc * 100)) / 100.0 + "%");
        }
        else
        {
            AD.SetText("Bullet Damage: " + (int)_bd);
            AP.SetText("Ability Power: " + (int)_ap);
            AS.SetText("Attack Speed: " + (Mathf.Round(_as * 100)) / 100.0);
            CC.SetText("Crit Chance: " + (Mathf.Round(_cc * 10)) / 10+"%");
        }
    }

}
