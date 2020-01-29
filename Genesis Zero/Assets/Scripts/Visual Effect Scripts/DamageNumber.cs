using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    public Text number;
    public float time = .5f;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        Destroy(this.gameObject, time);
    }

    public void Update()
    {
        time -= Time.deltaTime;
        //number.color = new Color(1,1,1, Mathf.Lerp(number.color.a,0,time/.5f));
    }
    // Start is called before the first frame update
    public void SetNumber(float num)
    {
        number.text = num+"";
    }
}
