using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    public Text number;
    private float damage;
    public float time = .5f;
    private float starttime;
    public Color color = Color.black;
    private float multi;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        Destroy(this.gameObject, time);
        starttime = time;
    }

    public void Update()
    {
        multi = -1 + 2 * (time / starttime);
        time -= Time.deltaTime;
        number.color = new Color(color.r, color.g, color.b, time/starttime);
        transform.position += new Vector3(Mathf.Abs(multi) * Time.deltaTime, multi * Time.deltaTime, 0);
    }
    // Start is called before the first frame update
    public void SetNumber(float num)
    {
        number.text = num+"";
        damage = num;
        number.fontSize += Mathf.Clamp((int)(number.fontSize * damage / 100), 0, number.fontSize);
    }

    // Start is called before the first frame update
    public void SetColor(Color color)
    {
        this.color = color;
        number.color = color;
    }
}
