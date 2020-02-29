using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GodHead : MonoBehaviour
{   
    public float activeDistance = 5.0f;
    private GameObject player;
    private GameInputActions inputActions;
    private Vector2 moveInput;
    private float selectInput;
    private RectTransform sacUI;
    private GameObject sacMods;
    private Canvas canvasRef;
    private SkillManager skillManager;
    private bool isActive = false;


    private void Update()
    {
        UpdateUI();
    }

    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inputActions = GameInputManager.instance.GetInputActions();
        inputActions.MenuControls.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        skillManager = player.GetComponent<Player>().GetSkillManager();
        canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= activeDistance)
            {
                //StateManager.instance.PauseGame();
                GameInputManager.instance.SwitchControlMap("MenuControls");
                InitializeUI();
                isActive = true;
            }
        }
    }
    private void UpdateUI()
    {
        if (!isActive) return;
        //closes UI when either of the button is pressed
    }

    private void InitializeUI()
    {
        sacUI = (RectTransform) canvasRef.transform.Find("SacrificeUI");
        sacMods = sacUI.gameObject.transform.Find("SacMods").gameObject;
        List<SkillObject> skills = skillManager.GetSkillObjects();
        Stack<SkillObject> rSkills = new Stack<SkillObject>();
        List<int> nums = new List<int>();
        Vector2 screenPos;
        Vector2 headScreenPos;
        int num;
        if (skills.Count > 0)
        {
            foreach (var skill in skills)
            {
                do { num = Random.Range(0, skills.Count); } while(nums.Contains(num));
                nums.Add(num);
                if (!skills[num].IsAbility)
                    rSkills.Push(skills[num]);
                if (rSkills.Count == 3)
                    break;
            }

            foreach (RectTransform child in sacMods.transform)
            {
                if (rSkills.Count > 0)
                {
                    child.gameObject.GetComponent<Image>().sprite = rSkills.Pop().Icon;
                    child.gameObject.SetActive(true);
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            //Maybe have a message saying that you got no skills right now
        }
        headScreenPos = canvasRef.worldCamera.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, 0));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRef.transform as RectTransform, headScreenPos, canvasRef.worldCamera, out screenPos);
        sacUI.anchoredPosition = screenPos;
        sacUI.gameObject.SetActive(true);
    }

    private void CloseUI()
    {
        foreach (RectTransform child in sacMods.transform)
        {
            child.gameObject.SetActive(false);
        }
        sacUI.gameObject.SetActive(false);
        StateManager.instance.UnpauseGame();
        GameInputManager.instance.SwitchControlMap("PlayerControls");
    }
}
