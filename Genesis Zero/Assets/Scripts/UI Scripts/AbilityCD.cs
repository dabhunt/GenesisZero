using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCD : MonoBehaviour
{
    Player player;
    public Image ability1Overlay;
    public Image ability2Overlay;
    public Image ability1Background;
    public Image ability2Background;
    public Color defaultColor = new Color(0.04705f, 0.1098f, 0.1294f);
    public Color activeColor;
    private Color currentColor;
    private bool cooling;
    //this is done so that active items show the color/animation, and also regular abilities flash the color briefly before going on cool down
    public float activeCutoff= .98f;
    void Start()
    {
        player = FindObjectOfType<Player>();
        ability1Overlay.fillAmount = 0;
        ability2Overlay.fillAmount = 0;
        
    }
    void Update()
    {
        if (cooling)
        {
            AbilityCasting ac = player.GetComponent<AbilityCasting>();
            float cd1 = ac.GetAbilityCooldownRatio1();
            float cd2 = ac.GetAbilityCooldownRatio2();
            ability1Background.color = defaultColor;
            ability2Background.color = defaultColor;
            if (cd1 >= activeCutoff)
            {
                //change sprite to represent activated/ animated colors
               
                currentColor = Color.Lerp(defaultColor, activeColor, Mathf.PingPong(Time.time, 1));
                ability1Background.color = currentColor;
            }
            else { 
                ability1Overlay.fillAmount = cd1;
                ability1Overlay.color = Color.black;
             }
            if (cd2 >= activeCutoff)
            {
                //change sprite to represent activated/ animated colors
                currentColor = Color.Lerp(defaultColor, activeColor, Mathf.PingPong(Time.time, 1));
                ability2Background.color = currentColor;
            }
            else { ability2Overlay.fillAmount = cd2;
                ability2Overlay.color = Color.black;
            }
            if (ability1Overlay.fillAmount <= 0 && ability2Overlay.fillAmount <= 0)
            {
                cooling = false;
            }
        }
       
    }
    public void Cast()
    {
        cooling = true;
    }
}
