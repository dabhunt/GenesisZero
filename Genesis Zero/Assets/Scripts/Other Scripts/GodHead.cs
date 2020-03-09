using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodHead : MonoBehaviour
{
    public float activeDistance = 5.0f;
    public int modsToChooseFrom = 3;
    private GameObject player;
    private GameInputActions inputActions;
    private Vector2 moveInput;
    private float selectInput;
    private float interactInput;
    private RectTransform sacUI;
    //private RectTransform confirmUI;
    private GameObject sacModObjs;
    private GameObject selection;
    private Canvas canvasRef;
    private SkillManager skillManager;
    public List<GameObject> modObjUI;
    //private SkillUIElement skillUIElement;
    private bool isActive = false;
    private bool confirmationWindowOpen = false;
    //change to private later below this point
    public int modSelectNum;
    //public List<SkillObject> modSkills;

    private void Update()
    {
        UpdateUI();
        InputUpdate();
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inputActions = GameInputManager.instance.GetInputActions();
        inputActions.MenuControls.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.Interact.performed += ctx => interactInput = ctx.ReadValue<float>();
        skillManager = player.GetComponent<Player>().GetSkillManager();
        canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
        //confirmUI.gameObject.SetActive(false);
    }
    public void InputInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Interact();
        }
    }
    public void Interact()
    {
        print("interact running");
        if (!inputActions.PlayerControls.enabled == true) return;
        if (Vector3.Distance(player.transform.position, transform.position) <= activeDistance)
        {
            //StateManager.instance.PauseGame();
            print("switching");
            GameInputManager.instance.SwitchControlMap("MenuControls");
            InitializeUI();
            isActive = true;
        }
    }
    public void Select(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            UpdateSelect(modSelectNum);
        }
    }
    public void UpdateSelect(int num)
    {
        //will add a confirmation window later
        if (modSelectNum == num)
        {
            FinalConfirmSelection();
        }
        else
        {
            selection.transform.position = new Vector3(157f * modSelectNum, selection.transform.position.y, selection.transform.position.z);
        }

        //confirmUI.gameObject.SetActive(true);
    }
    public void FinalConfirmSelection()
    {
        print("Finalconfirmselction");
        SkillObject skill = modObjUI[modSelectNum].GetComponent<SkillUIElement>().Skill;
        skillManager.RemoveSkill(skill);
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 2, 0);
        skillManager.SpawnAbility(spawnPoint, skillManager.GetRandomAbility().name);
        CloseUI();
    }
    private void InputUpdate()
    {
        if (!confirmationWindowOpen)
        {
            if (moveInput.x > 0)
            {
                modSelectNum++;
            }
            if (moveInput.y > 0)
            {
                if (modSelectNum != -1)
                {
                    modSelectNum = 1;
                }
            }
            if (moveInput.x < 0)
            {
                modSelectNum--;
            }
            if (moveInput.y < 0)
            {
                modSelectNum = -1;
            }
            Mathf.Clamp(modSelectNum, 0, 2);
        }
    }

    private void UpdateUI()
    {
        if (!isActive) return;

    }

    private void InitializeUI()
    {
        sacUI = (RectTransform) canvasRef.transform.Find("SacrificeUI");
        sacModObjs = sacUI.gameObject.transform.Find("SacMods").gameObject;
        selection = sacModObjs.gameObject.transform.Find("Selection").gameObject;
        List<SkillObject> modSkills = skillManager.GetRandomModsFromPlayer(3);
        Vector2 screenPos;
        Vector2 headScreenPos;
        if (modSkills.Count > 0)
        {
            int num = 0;
            foreach (RectTransform child in sacModObjs.transform)
            {
                if (child.gameObject.GetComponent<SkillUIElement>() != null && num < modsToChooseFrom && num < modSkills.Count)
                {
                    print("skillManager.GetColor(modSkills[num])" + skillManager.GetColor(modSkills[num]));
                    child.gameObject.GetComponent<SkillUIElement>().SetIcon(modSkills[num].Icon);
                    child.gameObject.GetComponent<SkillUIElement>().SetColor(skillManager.GetColor(modSkills[num]));
                    child.gameObject.SetActive(true);
                    modObjUI.Add(child.gameObject);
                    num++;
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
        headScreenPos = canvasRef.worldCamera.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y, 0));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRef.transform as RectTransform, headScreenPos, canvasRef.worldCamera, out screenPos);
        sacUI.anchoredPosition = screenPos;
        //confirmUI.anchoredPosition = screenPos;
        sacUI.gameObject.SetActive(true);
    }
    private void CloseUI()
    {
        isActive = false;
        foreach (RectTransform child in sacModObjs.transform)
        {
            child.gameObject.SetActive(false);
        }
        sacUI.gameObject.SetActive(false);
        //StateManager.instance.UnpauseGame();
        GameInputManager.instance.SwitchControlMap("PlayerControls");
    }
}
