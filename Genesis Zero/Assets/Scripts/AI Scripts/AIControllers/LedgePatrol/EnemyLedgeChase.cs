using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLedgeChase : StateMachineBehaviour
{
    Vector3 startpos;
    Enemy owner;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
        owner = animator.GetComponent<Enemy>();
        startpos = (Vector3)owner.transform.position;
        Debug.Log(owner.name + "Entering Chase State");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.distancefromTarget() > owner.targetdistance)
        {
            animator.SetTrigger("Patrol");
          //  animator.SetTrigger("Chase");
        }
        
        var target = (Vector3)owner.GetTarget().position;
        var currentpos = (Vector3)owner.transform.position;

        Debug.DrawLine(currentpos, target, Color.red);
        // Vector2 targetposition= Vector2.MoveTowards(currentpos, target, 1 * Time.deltaTime);
        Vector3 dirFromAtoB = (target - currentpos).normalized;

        Debug.Log(owner.name+" speed="+owner.GetSpeed().GetValue());

        float isfacing = Vector3.Dot(dirFromAtoB, owner.transform.right);

        Vector3 targetposition = currentpos + ((dirFromAtoB * .1f));// *Time.deltaTime);
        targetposition.y = startpos.y;
        Debug.Log(owner.name + ":is facing val= " + isfacing);
        if (isfacing < 0)
        {
            owner.transform.Rotate(0, 180, 0);
        }

        targetposition.z = currentpos.z;
        owner.transform.position = targetposition;

       
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(owner.name + "Exiting Chase State");
        animator.ResetTrigger("Chase");
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
