using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterAnimatorCode : MonoBehaviour
{
    static Animator anim;
    bool IsFacingRight;
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        IsFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
           anim.SetTrigger("isJumping");
        }

        anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        if(Input.GetAxis("Horizontal") < 0)
        {
           IsFacingRight = false;
           gameObject.transform.rotation = Quaternion.EulerRotation(new Vector3(0, -90, 0));
        }else
        {
           IsFacingRight = true;
           gameObject.transform.rotation = Quaternion.EulerRotation(new Vector3(0, 90, 0));
        }
        
        //How to have a quick "running to stop" code here.
    }
}
