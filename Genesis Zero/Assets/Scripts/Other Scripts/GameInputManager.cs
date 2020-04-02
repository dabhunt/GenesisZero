﻿using System.Collections;
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
    public void SwitchControlMap(string name)
    {
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
    //determines what input map. returns true, given the string name of an input map 
    //still does not seem to work properly
    public bool IsControlMapEnabled(string name) 
    {
        foreach (var map in actionAsset.actionMaps)
        {
            if (map.name == name)
            {
                return actionAsset.FindActionMap(map.name).enabled;
            }
        }
        return false;
    }
}
