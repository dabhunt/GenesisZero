using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class Merchant : MonoBehaviour
{
    public float activeDistance = 5.0f;
    public int shopItemAmount = 7;
    public float ySeperation = -95f;
    private GameObject player;
    private GameInputActions inputActions;
    private float interactInput;
    private Vector2 moveInput;
    private float selectInput;

    private RectTransform merchantUI;
    //private RectTransform confirmUI;
    private GameObject shopItemsParent;
    private ShopObject selectedShopItem;
    private Canvas canvasRef;
    private SkillManager skillManager;
    private List<GameObject> gameObjList;
    private List<ShopObject> shopObjList;
    private bool isActive = true;
    private int canistersNeeded = 0;
    private bool firstInteraction = true;
    //private bool confirmationWindowOpen = false;
    //change to private later below this point
    private int itemSelectNum = -1;
    //public List<SkillObject> modSkills;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Interact();
    }

    private void Start()
    {
        gameObjList = new List<GameObject>();
        shopObjList = new List<ShopObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        inputActions = GameInputManager.instance.GetInputActions();
        inputActions.PlayerControls.Interact.performed += ctx => interactInput = ctx.ReadValue<float>();
        skillManager = player.GetComponent<Player>().GetSkillManager();
        canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
        //-1 means that no item has actually been selected yet
        itemSelectNum = -1;
        //confirmUI.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (!isActive)
            return;
        if (GameInputManager.instance.GetActiveControlMap() == "MenuControls")
            return;
        if (Vector3.Distance(player.transform.position, transform.position) <= activeDistance)
        {
            StateManager.instance.PauseGame();
            GameInputManager.instance.SwitchControlMap("MenuControls");
            isActive = true;
            if (firstInteraction)
            {
                DestroyPopUps();
                InitializeUI();
            }
            else 
            {
                DestroyPopUps();
                merchantUI.gameObject.SetActive(true);
            }
                
            UpdateSelect(0);
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
        itemSelectNum = num;
        //gets the shop object associated with this selected number
        selectedShopItem = shopObjList[num];
        //deselect all menu items setting deactive
        for (int i = 0; i < gameObjList.Count; i++)
            gameObjList[i].transform.Find("Select").gameObject.SetActive(false);
        //select the current item by activating the select
        gameObjList[num].transform.Find("Select").gameObject.SetActive(true);
        //update description, icon, and name in the display section on the left
        string name = selectedShopItem.VisibleName;
        string desc = selectedShopItem.Description;
        Sprite icon = selectedShopItem.Icon;
        string cost = "x" + selectedShopItem.Cost.ToString();
        // if it's a mod, get the mod info instead
        if (selectedShopItem.Type == 0)
        {
            name = gameObjList[num].transform.Find("Name").GetComponent<Text>().text.ToString();
            SkillObject mod = skillManager.GetModFromString(name);
            cost = "x" + (1 + mod.Rarity).ToString();
            icon = mod.Icon;
            desc = mod.Description;
        }
        canistersNeeded = selectedShopItem.Cost;
        merchantUI.transform.Find("Name").gameObject.GetComponent<Text>().text = name;
        merchantUI.transform.Find("Description").gameObject.GetComponent<Text>().text = desc;
        merchantUI.transform.Find("ShownItem").gameObject.GetComponent<Image>().sprite = icon;
        merchantUI.transform.Find("Cost").gameObject.GetComponent<Text>().text = cost;
        Button purchaseButton = merchantUI.transform.Find("Purchase").gameObject.GetComponent<Button>();
        if (canistersNeeded > player.GetComponent<Player>().GetFullCapsuleAmount())
        {
            purchaseButton.interactable = false;
            purchaseButton.GetComponentInChildren<Text>().text = "Not Enough Essence";
        }
        else 
        {
            purchaseButton.interactable = true;
            purchaseButton.GetComponentInChildren<Text>().text = "Purchase Item";
        }
    }
    private void DestroyPopUps()
    {
        GetComponent<InteractPopup>().DestroyPopUp();
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickups");
        for (int i = 0; i < pickups.Length; i++)
        {
            pickups[i].GetComponent<InteractPopup>().DestroyPopUp();
        }
    }
    private void InitializeUI()
    {
        firstInteraction = false;
        //Destroy the "Press F to interact popup"
        merchantUI = (RectTransform) canvasRef.transform.Find("MerchantUI");
        shopItemsParent = merchantUI.gameObject.transform.Find("ShopItemParent").gameObject;

        //load all shopitems from resources
        Object[] temp = Resources.LoadAll("ShopItems");
        for (int i = 0; i < temp.Length; i++)
        {
            //cast as shopObject type and add each to the list
            shopObjList.Add((ShopObject)temp[i]);
        }
        
        //using the already created template gameobject which is correctly positioned, make the rest
        int num = 0;
        //used to prevent duplicate mods from appearing in the shop
        SkillObject duplicatePrevention = skillManager.GetRandomModByChance();
        foreach (RectTransform child in shopItemsParent.transform)
        {
            //if the object is a modifier, instead inherit Icon and Name from the associated skillobject
            child.gameObject.SetActive(true);
            if (shopObjList[num].Type == 0)
            {
                SkillObject mod = skillManager.GetRandomModByChance();
                while (duplicatePrevention == mod)
                {
                    mod = skillManager.GetRandomModByChance();
                }
                duplicatePrevention = mod;
                child.transform.Find("Icon").GetComponent<Image>().sprite = mod.Icon;
                child.transform.Find("Cost").GetComponent<Text>().text = "x"+(1 + mod.Rarity).ToString();
                child.transform.Find("Name").GetComponent<Text>().text = mod.name;
            }
            //if it's anything but a modifier, use the shopObject data from the asset
            else
            {
                child.transform.Find("Icon").GetComponent<Image>().sprite = shopObjList[num].Icon;
                child.transform.Find("Name").GetComponent<Text>().text = shopObjList[num].VisibleName;
                child.transform.Find("Cost").GetComponent<Text>().text = "x" + shopObjList[num].Cost.ToString();
            }
;           gameObjList.Add(child.gameObject);
            num++;
        }
        merchantUI.gameObject.SetActive(true);
        //load all shop items from resources, put them in the array as gameobjects
        itemSelectNum = 0;
        //sets default selection to position 0
        UpdateSelect(0);
    }
    public void FinalConfirmSelection()
    {
        //shortcut to get player data
        Player pp = player.GetComponent<Player>();
        float prevMax;
        switch (itemSelectNum)
        {
            case 0:
                //If it is an instant heal item
                pp.Heal(40f);
                break;
            case 1:
                //if it is a health or shield charge (type 1)
                pp.GetShield().AddValue(30f);
                break;
            case 4:
                //increase the maximum amount of essence capsules the player can have by 1
                prevMax = pp.GetMaxCapsuleAmount();
                pp.SetMaxCapsuleAmount(prevMax+1);
                canvasRef.transform.Find("EssencePanel").Find("capsule (6)").gameObject.SetActive(true);
                canvasRef.transform.Find("EssencePanel").Find("bg capsule (6)").gameObject.SetActive(true);
                break;
            case 5:
                //increase the maximum amount of mods player can have at one time by 1
                prevMax = skillManager.GetModLimit();
                skillManager.SetModLimit(prevMax + 1);
                break;
            case 6:
                //if it is a Key type that unlocks things, add 1 key to player
                pp.AddKeys(1);
                break;
            default:
                //defaults to a "mod" type, since there are 2 or more of these in the shop
                string name = merchantUI.transform.Find("Name").GetComponent<Text>().text;
                SkillObject mod = skillManager.GetModFromString(name);
                skillManager.AddSkill(mod);
                break;
        }
        //calculate how many essence canisters to subtract from the player
        int essenceCost = player.GetComponent<Player>().GetEssencePerCapsule() * canistersNeeded * -1;
        player.GetComponent<Player>().AddEssence(essenceCost);
        CloseUI();
    }
    public void CloseUI()
    {
        merchantUI.gameObject.SetActive(false);
        StateManager.instance.UnpauseGame();
        itemSelectNum = -1;
        GameInputManager.instance.SwitchControlMap("PlayerControls");
    }
}
