using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    public Text number;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        Destroy(this.gameObject, 1);
    }
    // Start is called before the first frame update
    public void SetNumber(float num)
    {
        number.text = num+"";
    }
}
