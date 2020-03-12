using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractMaster : MonoBehaviour
{

    // This script will hold all "interact" button related inputs, Godhead, and all pickups will be sent through here
    // any object that we want to be interactable, should have a Interactable script attached
    // currently this script is not active
    public float MaxCheckDist = 5f;
    private GameInputActions inputActions;
    private float interactInput;
    private Player player;
    void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Player");
        player = temp.GetComponent<Player>();
        inputActions = GameInputManager.instance.GetInputActions();
        inputActions.PlayerControls.Interact.performed += ctx => interactInput = ctx.ReadValue<float>();
    }
    void FindClosest()
    {

    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            FindClosest();
        }
    }


}
