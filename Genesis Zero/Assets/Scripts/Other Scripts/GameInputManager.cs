﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager instance;
    private GameInputActions inputActions;
    private void Awake() 
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        inputActions = new GameInputActions();
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
    public void SwitchControlMap(string name)
    {
        var actionMaps = inputActions.GetInputActionAsset();
        foreach (var actionMap in actionMaps)
        {
            if (actionMap.name == name)
            {
                actionMap.Enable();
            }
            else
            {
                actionMap.Disable();
            }
        }
    }
}