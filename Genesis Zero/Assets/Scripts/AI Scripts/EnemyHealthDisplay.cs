using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    private Pawn pawn;
    private Hurtbox hb;
    public GameObject healthBar;
    //public Image healthBar;
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
            healthBar.SetActive(true);
            healthBar.transform.localScale = new Vector3((hp/maxHealth)*2,.5f, 1);
        }
    }
}
