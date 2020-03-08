using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLedgePatrol : StateMachineBehaviour
{
    Enemy owner;
    int axis = 1;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        owner = animator.GetComponent<Enemy>();
        Debug.Log(owner.name + "Entering Patrol State");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
   override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (axis > 0){
            owner.transform.eulerAngles = new Vector3(0, -90, 0);
        }
        else
        {
            owner.transform.eulerAngles = new Vector3(0, 90, 0);
        }

        owner.Translate((new Vector3(.1f * axis, 0) * Time.deltaTime) * 20f);
        if (!owner.IsGrounded())
        {
          //  owner.transform.Rotate(new Vector3(0, 180, 0));
            axis *= -1;
        }
        if (owner.inRange(owner.targetdistance))
        {
           animator.SetTrigger("Chase");
           // animator.SetBool("Patrol", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(owner.name + "Exiting Patrol State");
        animator.ResetTrigger("Patrol");
        //animator.ResetTrigger("Patrol");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

   
}
