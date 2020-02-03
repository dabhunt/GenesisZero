/**
 * THIS CODE SHOULD BE REPLACED INTO IT'S PROPER SCRIPT STRUCTURE.
 * THIS FILE SHOULD NOT BE PERMANENT
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterAnimatorCode : MonoBehaviour
{
    static Animator anim;
    bool IsFacingRight;
    public CharacterController characterController;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        IsFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {

        //bool grounded = characterController.IsGrounded();
        if (Input.GetButtonDown("Jump") && characterController.IsGrounded() == true)
        {
           anim.SetTrigger("isJumping");
        }
        
        anim.SetBool("isGrounded", characterController.IsGrounded());
        if (characterController.IsGrounded())
        {
            anim.ResetTrigger("isJumping");
        }

        anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        if(Input.GetAxis("Horizontal") < 0)
        {
           IsFacingRight = false;
           gameObject.transform.rotation = Quaternion.EulerRotation(new Vector3(0, -90, 0));
        }if(Input.GetAxis("Horizontal") > 0)
        {
           IsFacingRight = true;
           gameObject.transform.rotation = Quaternion.EulerRotation(new Vector3(0, 90, 0));
        }
        
        //How to have a quick "running to stop" code here.
    }
}
