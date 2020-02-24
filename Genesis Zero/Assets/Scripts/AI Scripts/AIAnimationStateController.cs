using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIController))]
/**
 * Justin Couch
 * The AIAnimationStateController class connects AI states to animator states
 */
public class AIAnimationStateController : MonoBehaviour
{
    private AIController ai;
    private Animator anim;
    float fallLayerBlend = 0.0f;
    public float FallLayerBlendRate = 1.0f;

    private void Awake()
    {
        ai = GetComponent<AIController>();
        anim = GetComponentInChildren<Animator>();

        /*if (ai != null)
        {
            ai.StateChangeEvent.AddListener(ChangeState);
        }*/
    }

    private void Update()
    {
        if (ai == null || anim == null) { return; }

        anim.SetBool("IsIdling", ai.GetState() == AIController.AIState.Idle);
        anim.SetBool("IsPatrolling", ai.GetState() == AIController.AIState.Patrol);
        anim.SetBool("IsFollowing", ai.GetState() == AIController.AIState.Follow);
        anim.SetBool("IsCharging", ai.GetState() == AIController.AIState.Charge);
        anim.SetBool("IsAttacking", ai.GetState() == AIController.AIState.Attack);
        anim.SetBool("IsCoolingDown", ai.GetState() == AIController.AIState.Cooldown);
        anim.SetBool("IsFalling", !ai.IsGrounded());

        if (!ai.IsGrounded() && ai.GetState() != AIController.AIState.Charge && ai.GetState() != AIController.AIState.Attack && ai.GetState() != AIController.AIState.Cooldown)
        {
            fallLayerBlend = Mathf.Clamp01(fallLayerBlend + FallLayerBlendRate * Time.deltaTime);
        }
        else
        {
            fallLayerBlend = Mathf.Clamp01(fallLayerBlend - FallLayerBlendRate * Time.deltaTime);
        }
        anim.SetLayerWeight(1, fallLayerBlend);
    }

    /**
     * Updates the animator state based on the AI state
     */
    private void ChangeState(AIController.AIState newState)
    {
        if (anim == null) { return; }

        switch (newState)
        {
            case AIController.AIState.Idle:
                anim.SetTrigger("Idling");
                break;
            case AIController.AIState.Patrol:
                anim.SetTrigger("Patrolling");
                break;
            case AIController.AIState.Follow:
                anim.SetTrigger("Following");
                break;
            case AIController.AIState.Charge:
                anim.SetTrigger("Charging");
                break;
            case AIController.AIState.Attack:
                anim.SetTrigger("Attacking");
                break;
            case AIController.AIState.Cooldown:
                anim.SetTrigger("Cooling Down");
                break;
        }
    }
}
