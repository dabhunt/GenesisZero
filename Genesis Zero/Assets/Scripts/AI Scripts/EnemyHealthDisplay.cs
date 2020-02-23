using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    private Pawn pawn;
    private Hurtbox hb;
    public GameObject healthObj;
    public Image healthBar;
    private float maxHealth;
    void Start()
    {
        pawn = GetComponent<Pawn>();
        hb = GetComponent<Hurtbox>();
        maxHealth = pawn.GetHealth().GetValue();
    }

    // Update is called once per frame
    void Update()
    {
        float hp = pawn.GetHealth().GetValue();
        if (hp < maxHealth)
        {
            healthObj.SetActive(true);
            healthBar.fillAmount = hp / maxHealth;
        }
    }
}
