using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flamethrower : MonoBehaviour
{
    public VisualEffect flamethrower;
    public Animator anim;

    void Start()
    {
        flamethrower.Stop();
        anim.SetBool("Firing", false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            flamethrower.Play();
            anim.SetBool("Firing", true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            flamethrower.Stop();
            anim.SetBool("Firing", false);
        }
    }
}
