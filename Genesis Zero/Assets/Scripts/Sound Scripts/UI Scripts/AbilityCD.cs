using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCD : MonoBehaviour
{
    Player player;
    public Image[] abilityOverlay;
    public Image[] abilityBackground;
    public Color defaultColor = new Color(0.04705f, 0.1098f, 0.1294f);
    public Color activeColor;
    public Color coolingColor;
    private Color currentColor;
    private bool[] cooling = new bool[2];
    private float[] cd = new float[2];
    //this is done so that active items show the color/animation, and also regular abilities flash the color briefly before going on cool down
    public float activeCutoff= .98f;
    void Start()
    {
        player = FindObjectOfType<Player>();
        for (int i = 0; i < 2; i++)
        {
            abilityOverlay[i].fillAmount = 0f;
            abilityOverlay[i].color = defaultColor;
            abilityBackground[i].color = defaultColor;
            cooling[i] = false;
            cd[i] = 0;
        }
    }
    void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            if (player != null)
            {
                AbilityCasting ac = player.GetComponent<AbilityCasting>();
                cd[i] = ac.GetAbilityCooldownRatio(i);
                if (cooling[i] == true)
                {
                    abilityBackground[i].color = defaultColor;
                    if (cd[i] >= activeCutoff)
                    {
                        //change sprite to represent activated/ animated colors
                        currentColor = Color.Lerp(defaultColor, activeColor, Mathf.PingPong(Time.time, 1));
                        abilityBackground[i].color = currentColor;
                    }
                    else
                    {
                        abilityOverlay[i].fillAmount = cd[i];
                        abilityOverlay[i].color = coolingColor;
                    }
                }
                else
                {
                    abilityOverlay[i].color = defaultColor;
                }
                if (cd[i] <= 0)
                {
                    cooling[i] = false;
                }
            }
        }
    }
    public void Cast(int num)
    {
            cooling[num] = true;
    }
}
