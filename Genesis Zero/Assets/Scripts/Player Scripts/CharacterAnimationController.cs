/**
 * THIS CODE SHOULD BE REPLACED INTO IT'S PROPER SCRIPT STRUCTURE.
 * THIS FILE SHOULD NOT BE PERMANENT
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    static Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim.GetBool("isGrounded"))
        {
            
        }
        else
        {
            
        }

        if(anim.GetFloat("xSpeed") > 0)
        {
           
        }
        else
        {
           
        }
    }
}
