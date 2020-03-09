using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager instance;
    private GameInputActions inputActions;
    private InputActionAsset actionMaps;
    private void Awake() 
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        inputActions = new GameInputActions();
        actionMaps = inputActions.GetInputActionAsset();
    }

    private void OnDisable() 
    {
        inputActions.Disable();
    }

    private void OnEnable() 
    {
        inputActions.Enable();
    }

    public GameInputActions GetInputActions()
    {
        return inputActions;
    }

    //This switches the control map to the one with "name" as its name
    //this function was not working as intended, I disabled it - David
    public void SwitchControlMap(string name)
    {
        Debug.Log("SwitchingControlMap to " + actionMaps.name);

        actionMaps.FindActionMap(name);
        //print("actionMap.name=" +actionMap.name);
        //if (actionMap.name == name)
        //{
        //    actionMap.Enable();
        //}
        //else
        //{
        //    actionMap.Disable();
        //}
    }
}
