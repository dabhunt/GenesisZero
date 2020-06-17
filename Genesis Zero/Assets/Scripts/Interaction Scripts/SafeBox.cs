using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeBox : MonoBehaviour
{
    private List<SkillObject> modList;
    private SkillManager sk;
    private GameObject player;
    private Animator animator;
    Player playerScript;
    /* This script controls what happens when the player interacts with the mod converter, and the math behind what mod you get in return
     * Essentially, you cannot get back a modifier of lower quality than you put in, and putting in more modifiers increases the likelyhood
     * of a higher tier mod coming out
     * ScrapConverter
     */
    private void Start()
    {
        modList = new List<SkillObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        sk = playerScript.GetSkillManager();
        animator = transform.Find("Top").gameObject.GetComponent<Animator>();
    }
    //When you press the Interact button, the Merchant/Machine determines what random Mod to give you
    private void Update()
    {
        if (GetComponent<InteractPopup>() == null)
            return;
        if (playerScript.GetKeysAmount() < 1)
        {
            GetComponent<InteractPopup>().SetText("You need a Keycode to open this");
        }
        else
        {
            GetComponent<InteractPopup>().SetText("Press [F] to Open the Lock Box");
        }
    }
    public void Interact()
    {
        print("interacting");
        if (player == null)
            return;
        if (Vector2.Distance(player.transform.position, gameObject.transform.position) <= 5)
        {
            InteractPopup iPop = GetComponent<InteractPopup>();
            if (iPop == null) //if the player has already
                transform.Find("Top").gameObject.transform.Find("LightEffects").gameObject.SetActive(false);
            if (playerScript.GetKeysAmount() >= 1)
            {
                animator.SetTrigger("OpenTrigger");
                transform.Find("Top").gameObject.transform.Find("LightEffects").gameObject.SetActive(true);
                if (iPop != null)
                {
                    iPop.DestroyPopUp();
                    Destroy(iPop);
                }
                //marks this object as no longer able to be interacted with
                gameObject.AddComponent<InactiveFlag>();
                playerScript.GetKeys().AddValue(-1);
            }
        }
    }
    //returns a SkillObject, takes an int as the number of times that the mod should reroll to get best rarity value
    public SkillObject GetNewMod(int rerolls)
    {
        //the amount of mods you put in increases the guaranteed lowest rarity you can receive, and increases how likely you are to get a better mod
        SkillObject newMod = sk.GetRandomModByChance();
        //start at 1, so that this only runs at values > 1
        for (int i = 1; i < rerolls; i++)
        {
            SkillObject oldMod = newMod;
            //for each reroll, we roll a new mod by chance, if that mod exceeds the value of the stored mod, replace it
            newMod = sk.GetRandomModByChance();
            if (newMod.Rarity < oldMod.Rarity)
                newMod = oldMod;
        }
        modList.Clear();
        //deactivate Lock box
        return newMod;
    }
    public void AfterAnimation()
    {
        sk.SpawnMod(transform.position + new Vector3(0.1f, 3, 0), sk.GetRandomGreens(1)[0].name); //one rare guaranteed
        sk.SpawnMod(transform.position + new Vector3(0, 3, 0), GetNewMod(4).name); //one mod gets the best of 3 rolls
        if (Random.value < .5f)
            sk.SpawnMod(player.transform.position + new Vector3(-.1f, 3, 0), GetNewMod(1).name);
    }
}

