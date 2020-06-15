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
    private int interactionCount = 0;
    private SkillObject[] savedMods;
    //private bool confirmationWindowOpen = false;
    //change to private later below this point
    private int itemSelectNum = -1;
    //public List<SkillObject> modSkills;
    private void Start()
    {
        gameObjList = new List<GameObject>();
        shopObjList = new List<ShopObject>();
        savedMods = new SkillObject[2];
        player = Player.instance.gameObject;
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
        //prevent player from activating multiple times
        if (StateManager.instance.IsPaused())
            return;
        if (Vector2.Distance(player.transform.position, transform.position) <= activeDistance)
        {
            int interactions = DialogueManager.instance.GetInteractAmount(0);
            StateManager.instance.DestroyPopUpsWithTag("Pickups");
            StateManager.instance.DestroyPopUpsWithTag("Interactable");
            //if player has interacted with a merchant less than twice, show extended dialogue
            if (interactions <= 1)
            {
                //reads the 0_ and 1_Merchant files
                DialogueManager.instance.TriggerDialogue(interactions + "_Merchant");
            }
            else // otherwise, play only one line of dialogue
            {
                AfterDialogue();
            }
            DialogueManager.instance.SetInteractionAfterDialogue(0);
            //incrementInteract takes an int representing type, which is 0 for merchant and adds 1.
            DialogueManager.instance.IncrementInteract(0);
            AudioManager.instance.StopAllSounds();
            StateManager.instance.PauseGame();
        }
    }
    public void AfterDialogue()
    {
        AudioManager.instance.StopAllSounds();
        GameInputManager.instance.SwitchControlMap("MenuControls");
        isActive = true;
        if (firstInteraction)
        {
            interactionCount++;
            InitializeUI();
            UpdateSelect(0);
        }
        else
        {
            InitializeUI();
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
        string rarity = "";
        Color color = Color.white;
        Sprite icon = selectedShopItem.Icon;
        string cost = "x" + selectedShopItem.Cost.ToString();
        canistersNeeded = selectedShopItem.Cost;
        // if it's a mod, get the mod info instead
        if (selectedShopItem.Type == 0)
        {
            name = gameObjList[num].transform.Find("Name").GetComponent<Text>().text.ToString();
            SkillObject mod = skillManager.GetSkillFromString(name);
            rarity = " ( " + skillManager.GetRarityString(mod) + " )";
            color = skillManager.GetColor(mod);
            cost = "x" + (mod.Rarity).ToString();
            canistersNeeded = mod.Rarity;
            icon = mod.Icon;
            desc = mod.Description;
        }
        merchantUI.transform.Find("Name").gameObject.GetComponent<Text>().text = name;
        merchantUI.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = desc;
        merchantUI.transform.Find("Rarity").gameObject.GetComponent<TextMeshProUGUI>().text = rarity;
        merchantUI.transform.Find("Rarity").gameObject.GetComponent<TextMeshProUGUI>().color = color;
        merchantUI.transform.Find("ShownItem").gameObject.GetComponent<Image>().sprite = icon;
        merchantUI.transform.Find("Cost").gameObject.GetComponent<Text>().text = cost;
        Button purchaseButton = merchantUI.transform.Find("Purchase").gameObject.GetComponent<Button>();
        Player pScript = player.GetComponent<Player>();
        int modAmount = pScript.GetSkillManager().GetUniqueModAmount();
        int modLimit = pScript.GetSkillManager().GetModSlotLimit();
        //if player doesn't have enough essence to make purchase
        if (canistersNeeded > pScript.GetFullCapsuleAmount())
        {
            purchaseButton.interactable = false;
            purchaseButton.GetComponentInChildren<Text>().text = "Not Enough Essence";
        }
        //if the player has selected a mod they don't already have, and don't have room for it
        else if (selectedShopItem.Type == 0 && (skillManager.GetUniqueModAmount() >= skillManager.GetModSlotLimit() && !skillManager.HasSkill(name)))
        {
                purchaseButton.interactable = false;
                purchaseButton.GetComponentInChildren<Text>().text = "Mod Limit Reached";
        }
        else
        {//if none of the above are true, the player is allowed to purchase the item
            purchaseButton.interactable = true;
            purchaseButton.GetComponentInChildren<Text>().text = "Purchase Item";
        }
    }
    public SkillObject[] GetSavedMods()
    {
        if (savedMods[0] && savedMods[1]) //if savedMods already has data in it
        {
            return savedMods; //if the mods have already been rolled
        }
        savedMods[0] = skillManager.GetRandomModByChance();
        //used to prevent duplicate mods from appearing in the shop
        savedMods[1] = skillManager.GetRandomModByChance();
        while (savedMods[1].name == savedMods[0].name)
        {
            savedMods[1] = skillManager.GetRandomModByChance();
        }
        //savedMods[1] = savedMods[0];
        return savedMods;
    }
    public void InitializeUI()
    {
        player.GetComponent<Player>().SetInteracting(true);
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
        int z = 0;
        foreach (RectTransform child in shopItemsParent.transform)
        {
            //if the object is a modifier, instead inherit Icon and Name from the associated skillobject
            child.gameObject.SetActive(true);
            if (shopObjList[num].Type == 0)
            {
                GetSavedMods();//refresh mod list if needed
                child.transform.Find("Icon").GetComponent<Image>().sprite = savedMods[z].Icon;
                child.transform.Find("Cost").GetComponent<Text>().text = "x"+(savedMods[z].Rarity).ToString();
                child.transform.Find("Name").GetComponent<Text>().text = savedMods[z].name;
                z++;
            }
            //if it's anything but a modifier, use the shopObject data from the asset
            else
            {
                child.transform.Find("Icon").GetComponent<Image>().sprite = shopObjList[num].Icon;
                child.transform.Find("Name").GetComponent<Text>().text = shopObjList[num].VisibleName;
                child.transform.Find("Cost").GetComponent<Text>().text = "x" + shopObjList[num].Cost.ToString();
            }
            if (child.gameObject.GetComponent<Button>())
            {
                child.gameObject.GetComponent<Button>().interactable = true;
                child.transform.Find("PurchasedOverlay").gameObject.SetActive(false);
            } 
            gameObjList.Add(child.gameObject);
            num++;
        }
        SkillManager sk = Player.instance.GetSkillManager();
        if (sk.GetModSlotLimit() >= sk.GetModHardCap())
        {
            disableShopItem(5, "Out of stock");
        }
        if (Player.instance.GetMaxCapsuleAmount() >= 6)
        {
            disableShopItem(4, "Out of stock");
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
                pp.HealPercent(.5f);
                break;
            case 1:
                //if it is a health or shield charge (type 1)
                pp.GetShield().AddMaxValue(30);
                pp.GetShield().SetValue(30f);
                break;
            case 4:
                //increase the maximum amount of essence capsules the player can have by 1
                prevMax = pp.GetMaxCapsuleAmount();
                pp.SetMaxCapsuleAmount(prevMax+1);
                break;
            case 5:
                //increase the maximum amount of mods player can have at one time by 1
                prevMax = skillManager.GetModSlotLimit();
                skillManager.SetModLimit(prevMax + 1);
                break;
            case 6:
                //if it is a Key type that unlocks things, add 1 key to player
                pp.AddKeys(1);
                break;
            default:
                //defaults to a "mod" type, since there are 2 or more of these in the shop
                string name = merchantUI.transform.Find("Name").GetComponent<Text>().text;
                SkillObject mod = skillManager.GetSkillFromString(name);
                skillManager.AddSkill(mod);
                break;
        }
        //calculate how many essence canisters to subtract from the player
        int essenceCost = player.GetComponent<Player>().GetEssencePerCapsule() * canistersNeeded * -1;
        player.GetComponent<Player>().AddEssence(essenceCost);
        //set the purchased overlay to active so player is not able to select Item
        disableShopItem(itemSelectNum, "Purchased");
        //update UI, since game is paused must be manually done
        canvasRef.transform.Find("EssencePanel").gameObject.GetComponent<EssenceFill>().CalculateEssenceUI();
    }
    public void CloseUI()
    {
        player.GetComponent<Player>().SetInteracting(false);
        merchantUI.gameObject.SetActive(false);
        StateManager.instance.UnpauseGame();
        itemSelectNum = -1;
        GameInputManager.instance.SwitchControlMap("PlayerControls");
    }
    public void disableShopItem(int num, string text)
    {
        text = text.ToUpper();
        GameObject purchasedOverlay = gameObjList[num].transform.Find("PurchasedOverlay").gameObject;
        purchasedOverlay.SetActive(true);
        Button purchaseButton = merchantUI.transform.Find("Purchase").gameObject.GetComponent<Button>();
        purchaseButton.interactable = false;
        purchasedOverlay.GetComponentInChildren<Text>().text = text;
        gameObjList[num].GetComponent<Button>().interactable = false;
    }
    public bool GetWindowOpen()
    {
        return merchantUI.gameObject.activeSelf;
    }
}
