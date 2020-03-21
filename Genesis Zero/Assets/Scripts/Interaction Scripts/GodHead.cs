using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class GodHead : MonoBehaviour
{
    public float activeDistance = 5.0f;
    public int modsToChooseFrom = 3;
    public float selectionOffset = 166f;
    private GameObject player;
    private GameInputActions inputActions;
    private float interactInput;
    private Vector2 moveInput;
    private float selectInput;

    private RectTransform sacUI;
    private GameObject selection;
    //private RectTransform confirmUI;
    private GameObject sacModObjs;
    private Canvas canvasRef;
    private SkillManager skillManager;
    private List<GameObject> modObjUI;
    //private SkillUIElement skillUIElement;
    private bool isActive = true;
    private int canistersNeeded = 0;
    //private bool confirmationWindowOpen = false;
    //change to private later below this point
    private int modSelectNum = -1;
    //public List<SkillObject> modSkills;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inputActions = GameInputManager.instance.GetInputActions();
        inputActions.PlayerControls.Interact.performed += ctx => interactInput = ctx.ReadValue<float>();
        inputActions.MenuControls.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        
        skillManager = player.GetComponent<Player>().GetSkillManager();
        canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
        modSelectNum = -1;
        modObjUI = new List<GameObject>();
        //confirmUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Interact();
    }
    public void Interact()
    {
        if (!isActive)
        {
            return;
        }
        if (GameInputManager.instance.GetActiveControlMap() == "MenuControls")
            return;
        if (Vector3.Distance(player.transform.position, transform.position) <= activeDistance && skillManager.GetModAmount() > 0)
        {
            StateManager.instance.PauseGame();
            GameInputManager.instance.SwitchControlMap("MenuControls");
            isActive = true;
            InitializeUI();
        }
        else
        {
            GetComponent<InteractPopup>().SetText("This Interaction requires at least one Modifier.");
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
            selection.transform.localPosition = new Vector3((selectionOffset * num)-selectionOffset, selection.transform.localPosition.y, selection.transform.localPosition.z);
            //name = selection.gameObject.transform.Find("Name").gameObject;
            SkillObject skill = modObjUI[num].GetComponent<SkillUIElement>().Skill;
            string name = skill.name;
            string desc = skill.Description;
            selection.transform.Find("Name").gameObject.GetComponent<Text>().text = name;
            selection.transform.Find("Description").gameObject.GetComponent<Text>().text = desc;
            GameObject extra = selection.transform.Find("ExtraCanister").gameObject;
            string botText = "";
            //get rarity color, convert to html so the color can be shown in text
            Color sColor = skillManager.GetColor(skill);
            string hex = ColorUtility.ToHtmlStringRGB(sColor);
            extra.transform.Find("TopText").gameObject.GetComponent<TextMeshProUGUI>().text = "Sacrificing a Modifier of this <color=#" + hex + ">Rarity</color> ALSO requires:";
            canistersNeeded = 0;
            extra.SetActive(true);
            Button sacButton = sacUI.transform.Find("Sacrifice").GetComponent<Button>();
            switch (skill.Rarity)
            {
                case 1:
                    botText = "Canisters of Essence";
                    canistersNeeded = 3;
                    break;
                case 2:
                    botText = "Canister of Essence";
                    canistersNeeded = 1;
                    break;
                case 3:
                    extra.SetActive(false);
                    break;

            }
            //if the player doesn't have enough canisters of essence to make that deal, make the button not interactable and change button text
            if (canistersNeeded > player.GetComponent<Player>().GetFullCapsuleAmount())
            {
                sacButton.interactable = false;
                sacButton.GetComponentInChildren<Text>().text = "Not Enough Essence";
            }
            else 
            {
                sacButton.interactable = true;
                sacButton.GetComponentInChildren<Text>().text = "Sacrifice Modifer";
            }
            extra.transform.Find("BottomText").gameObject.GetComponent<TextMeshProUGUI>().text = botText;
            extra.transform.Find("x1").gameObject.GetComponent<TextMeshProUGUI>().text = "x"+canistersNeeded.ToString();
            
        }

        //confirmUI.gameObject.SetActive(true);
    }
    public void FinalConfirmSelection()
    {
        SkillObject skill = modObjUI[modSelectNum].GetComponent<SkillUIElement>().Skill;
        skillManager.RemoveSkill(skill);
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 2, 0);
        SkillObject ability = skillManager.GetRandomAbility();
        //Guarantee that the player gets an ability they don't currently have
        //Also, this may at some point even keep track of all abilities the player has ever gotten, to ensure maximum variety for the demo
        while (skillManager.GetAbility1() == ability || skillManager.GetAbility2() == ability)
        {
           ability = skillManager.GetRandomAbility();
        }
        skillManager.SpawnAbility(spawnPoint, skillManager.GetRandomAbility().name);
        //calculate how many essence canisters to subtract from the player
        int essenceCost = player.GetComponent<Player>().GetEssencePerCapsule() * canistersNeeded * -1;
        player.GetComponent<Player>().AddEssence(essenceCost);
        CloseUI();
    }
    private void InitializeUI()
    {
        //destroy all popups when entering the interface
        GetComponent<InteractPopup>().DestroyPopUp();
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickups");
        for (int i = 0; i < pickups.Length; i++)
        {
            pickups[i].GetComponent<InteractPopup>().DestroyPopUp();
        }
        Destroy(GetComponent<InteractPopup>());
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
        sacUI.gameObject.SetActive(false);
        StateManager.instance.UnpauseGame();
        modSelectNum = -1;
        GameInputManager.instance.SwitchControlMap("PlayerControls");
    }
}
