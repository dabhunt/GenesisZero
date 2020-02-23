using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeUI : MonoBehaviour
{
    private Player player;
    private PlayerController playerController;
    public RectTransform selectionObj;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private float selectInput;
    private SkillObject skill;
    GameObject[] ModUI;
    GameObject selectedMod;

    private void Wake()
    {
        //inputActions.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.MenuControls.MoveSelect.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.MenuControls.Select.performed += ctx => selectInput = ctx.ReadValue<float>();
        inputActions = new PlayerInputActions();
        
    
    }
    void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Player");
        player = temp.GetComponent<Player>();
        playerController = player.GetComponent<PlayerController>();
        SetPosition(5f);
    }
    //
    //if player presses on a hardware mod in the bar above, it highlights
    void changeSelection()
    {
        if (moveInput.x > 0)
        {
            selectionObj.transform.position += Vector3.right * (0.765f+75f);
        }
        if (moveInput.x < 0)
        {
            selectionObj.transform.position -= Vector3.right * (0.765f+75f);   
        }
    }
    void SetPosition(float xValue)
    {
        selectionObj.transform.position = new Vector3(xValue,1,1);
    }
    void openSacrificeWindow()
    {
       ModUI = GameObject.FindGameObjectsWithTag("ModUI");
        selectedMod = ModUI[1];
        SkillObject skill = selectedMod.GetComponent<SkillUIElement>().GetComponent<SkillObject>();
        
    }
    void ScrapMod()
    {
        player.GetSkillManager().RemoveSkill(skill);
    }
    //remove skill function
    //if we get a click from the player
}
