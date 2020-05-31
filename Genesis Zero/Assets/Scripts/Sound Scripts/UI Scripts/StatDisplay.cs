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
       
    }
    public void UpdateStats()
    {
        float _bd = Player.instance.GetDamage().GetValue() * 10;
        float _ap = Player.instance.GetAbilityPower().GetValue() * 10;
        float _as = Player.instance.GetAttackSpeed().GetValue();
        float _cc = Player.instance.GetCritChance().GetValue() * 100;
        AD.SetText("Bullet Damage: " + (int)_bd );
        AP.SetText("Ability Power: " + (int)_ap);
        AS.SetText("Attack Speed: " + (Mathf.Round(_as * 100)) / 100.0);
        CC.SetText("Crit Chance: " + _cc + "%");
    }
}
