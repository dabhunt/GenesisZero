using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class disapear : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = .01f;
    public Text instructions;
    void Start()
    {
        Cursor.visible = false;
        Invoke("Showtext", 4);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void Showtext()
    {
    	instructions.CrossFadeAlpha(0.0f, 0.01f, false);
    }
}
