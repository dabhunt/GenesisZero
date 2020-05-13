using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI number;
    private float damage;
    public float time = .5f;
    private float starttime;
    public Color color;
    private float multi;
	private Vector3 scale;

    private void Start() {
        GetComponent<Canvas>().worldCamera = Camera.main;
        Destroy(this.gameObject, time);
        starttime = time;
		scale = GetComponent<RectTransform>().localScale;
	}

    public void Update()
    {
        multi = -1 + 2 * (time / starttime);
        time -= Time.deltaTime;
        number.color = new Color(color.r, color.g, color.b, time / starttime);
        //number.color = new Color(1, 1, 1, time / starttime);
        transform.position += new Vector3(Mathf.Abs(multi) * Time.deltaTime, multi * Time.deltaTime, 0);
		GetComponent<RectTransform>().localScale = scale * Mathf.Clamp(((time + (starttime / 2)) / starttime), 1, 99);

	}

    // Sets the display of the text given the number
    public void SetNumber(float num)
    {
        damage = (int)num;
        number.text = damage + "";
        number.fontSize += Mathf.Clamp((int)(number.fontSize * damage / 100), 0, number.fontSize);
    }

    // Sets the display of the text given the number, if critcal, gives a ! sign
    public void SetNumber(float num, bool critical)
    {
        SetNumber(num);
        if (critical)
        {
            number.text += "!";
        }
    }

    // Start is called before the first frame update
    public void SetColor(Color color)
    {
        this.color = color;
        //number.material.SetColor("_OutlineColor", color);
        number.outlineColor = color;
    }
}
