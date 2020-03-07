using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class disapear : MonoBehaviour
{
    // Start is called before the first frame update
    public float fadeTime = 3f;
    public Text instructions;
    void Start()
    {
        //Cursor.visible = false;
        Invoke("Showtext", 4);
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void Showtext()
    {
        Color fixedColor = instructions.color;
        fixedColor.a = 1;
        instructions.color = fixedColor;
        instructions.CrossFadeAlpha(0f, 0f, true);
        instructions.CrossFadeAlpha(1, fadeTime, false);
        instructions.CrossFadeAlpha(0.0f, 0.01f, false);
    }
}
