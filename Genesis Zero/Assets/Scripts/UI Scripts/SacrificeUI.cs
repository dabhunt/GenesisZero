using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SacrificeUI : MonoBehaviour
{
    [Header("SacrificeUI")]
    private GameInputActions inputActions;
    private Vector2 moveInput;
    private float selectInput;
    private GameObject sacUI;

    private void Awake()
    {
        inputActions = GameInputManager.instance.GetInputActions();
        inputActions.MenuControls.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    }
    private void Update()
    {
        //UI behavior goes here. I.e: Selectting stuff, 
    }

    private void CloseUI()
    {
        //This line will just call OnDisable() to switch back to regular game state
        this.enabled = false;
    }

    private void OnEnable() 
    {
        //Pauses game, switch control map
        StateManager.instance.PauseGame();
        GameInputManager.instance.SwitchControlMap("MenuControls");

        //Bring up the UI
        sacUI = GameObject.FindGameObjectWithTag("CanvasUI").transform.Find("SacrificeUI").gameObject;
        //To do:
        // Populate the actual SacUI, and move it to where the GodHead is
        sacUI.SetActive(true);
    }

    private void OnDisable() 
    {
        //Put UI away
        sacUI.SetActive(false);
        //Switch control map, unpause game
        GameInputManager.instance.SwitchControlMap("PlayerControls");
        StateManager.instance.UnpauseGame();
    }
}
