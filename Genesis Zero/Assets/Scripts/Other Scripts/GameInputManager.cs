using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager instance;
    private GameInputActions inputActions;
    private InputActionAsset actionAsset;
    private string activeControlMap;
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
        actionAsset = inputActions.GetInputActionAsset();
        //inputActions.MenuControls.Disable();
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
    public string GetActiveControlMap()
    {
        return activeControlMap;
    }

    //This switches the control map to the one with "name" as its name
    //this function was not working as intended, I disabled it - David
    public void SwitchControlMap(string name)
    {
        Debug.Log("SwitchingControlMap to " + name);

        foreach (var map in actionAsset.actionMaps)
        {
            if (map.name == name)
            {
                actionAsset.FindActionMap(map.name).Enable();
                activeControlMap = map.name;
            }
            else
            {
                actionAsset.FindActionMap(map.name).Disable();
            }
        }
    }
}
