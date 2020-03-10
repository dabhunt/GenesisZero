using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GodHead : MonoBehaviour
{
    public float activeDistance = 5.0f;
    public int modsToChooseFrom = 3;
    public float selectionOffset = -157f;
    private GameObject player;
    private GameInputActions inputActions;
    private Vector2 moveInput;
    private float selectInput;
    private float interactInput;
    private RectTransform sacUI;
    private GameObject selection;
    //private RectTransform confirmUI;
    private GameObject sacModObjs;
    private Canvas canvasRef;
    private SkillManager skillManager;
    private List<GameObject> modObjUI;
    //private SkillUIElement skillUIElement;
    private bool isActive = true;
    //private bool confirmationWindowOpen = false;
    //change to private later below this point
    private int modSelectNum = -1;
    //public List<SkillObject> modSkills;

    private void Update()
    {
        UpdateUI();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inputActions = GameInputManager.instance.GetInputActions();
        inputActions.MenuControls.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.Interact.performed += ctx => interactInput = ctx.ReadValue<float>();
        skillManager = player.GetComponent<Player>().GetSkillManager();
        canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
        modSelectNum = -1;
        modObjUI = new List<GameObject>();
        //confirmUI.gameObject.SetActive(false);
    }
    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!isActive)
                return;

            //if(inputActions.MenuControls.enabled) return;
            if (Vector3.Distance(player.transform.position, transform.position) <= activeDistance)
            {
                //StateManager.instance.PauseGame();
                GameInputManager.instance.SwitchControlMap("MenuControls");
                isActive = true;
                InitializeUI();
            }
        }
    }

    public void Select(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            
        }
    }
    public void UpdateSelect(int num)
    {
        //will add a confirmation window later
        if (modSelectNum == num)
        {
            //FinalConfirmSelection();
        }
        else
        {
            modSelectNum = num;
            selection = GameObject.FindGameObjectWithTag("Selection");
            //move the selection sprite to the right
            selection.transform.localPosition = new Vector3((157f * num)-selectionOffset, selection.transform.localPosition.y, selection.transform.localPosition.z);
            //name = selection.gameObject.transform.Find("Name").gameObject;
            string name = modObjUI[num].GetComponent<SkillUIElement>().Skill.name;
            string desc = modObjUI[num].GetComponent<SkillUIElement>().Skill.Description;
            selection.transform.Find("Name").gameObject.GetComponent<Text>().text = name;
            selection.transform.Find("Description").gameObject.GetComponent<Text>().text = desc;
        }

        //confirmUI.gameObject.SetActive(true);
    }
    public void FinalConfirmSelection()
    {
        SkillObject skill = modObjUI[modSelectNum].GetComponent<SkillUIElement>().Skill;
        skillManager.RemoveSkill(skill);
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 2, 0);
        skillManager.SpawnAbility(spawnPoint, skillManager.GetRandomAbility().name);
        CloseUI();
    }

    private void UpdateUI()
    {
        if (!isActive) return;

        

    }

    private void InitializeUI()
    {
        sacUI = (RectTransform) canvasRef.transform.Find("SacrificeUI");
        sacModObjs = sacUI.gameObject.transform.Find("SacMods").gameObject;
        sacUI.gameObject.SetActive(true);
        
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
                    child.gameObject.SetActive(true);

                    child.gameObject.GetComponent<SkillUIElement>().SetIcon(modSkills[num].Icon);
                    child.gameObject.GetComponent<SkillUIElement>().SetSkill(modSkills[num]);
                    child.gameObject.GetComponent<SkillUIElement>().SetColor(skillManager.GetColor(modSkills[num]));
                    modObjUI.Add(child.gameObject);
                    num++;
                }
            }
        }
        else
        {
            //Maybe have a message saying that you got no skills right now
        }
        headScreenPos = canvasRef.worldCamera.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y+1.5f, 0));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRef.transform as RectTransform, headScreenPos, canvasRef.worldCamera, out screenPos);
        sacUI.anchoredPosition = screenPos;
        
        //confirmUI.anchoredPosition = screenPos;
        //sets default selection to position 0
        UpdateSelect(0);
    }
    public void CloseUI()
    {
        isActive = false;
        foreach (RectTransform child in sacModObjs.transform) 
            child.gameObject.SetActive(false);
        sacUI.gameObject.SetActive(false);
        //StateManager.instance.UnpauseGame();
        modSelectNum = -1;
        GameInputManager.instance.SwitchControlMap("PlayerControls");
    }
}
