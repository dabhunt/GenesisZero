using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AIController))]
/**
 * Justin Couch
 * AIStateEffect is used to connect the current state of an AI to other things like hitboxes and effects.
 */
public class AIStateEffect : MonoBehaviour
{
    private AIController ai;
    public AIController.AIState EffectState; // Relevant state to track
    public UnityEvent StateEnterEvent; // Invoked when the state is entered
    public UnityEvent StateExitEvent; // Invoked when the state is exited

    private void Awake()
    {
        ai = GetComponent<AIController>();
        ai.StateChangeEvent.AddListener(OnStateChange);
    }

    private void OnStateChange(AIController.AIState newState)
    {
        if (newState == EffectState)
        {
            StateEnterEvent.Invoke();
        }
        else
        {
            StateExitEvent.Invoke();
        }
    }
}
