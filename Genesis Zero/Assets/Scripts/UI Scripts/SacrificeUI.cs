using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeUI : MonoBehaviour
{
    public RectTransform selectionObj;

    private Player player;
    private PlayerController playerController;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private float selectInput;
    private SkillObject skill;
    GameObject[] ModUI;
    GameObject selectedMod;

    private void Wake()
    {
        //inputActions.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions = new PlayerInputActions();
    }
    private void Start()
    {
        
    }
    //if player presses on a hardware mod in the bar above, it highlights
    private void changeSelection()
    {
        
    }
    private void SetPosition(float xValue)
    {
        selectionObj.transform.position = new Vector3(xValue,1,1);
    }
    private void openSacrificeWindow()
    {
        ModUI = GameObject.FindGameObjectsWithTag("ModUI");
        selectedMod = ModUI[1];
        SkillObject skill = selectedMod.GetComponent<SkillUIElement>().GetComponent<SkillObject>();
    }
}
