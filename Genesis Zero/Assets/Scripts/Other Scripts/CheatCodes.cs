using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CheatCodes : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CheatPanel;
    public Button NextLevelButton;
    public Button TPBossButton;
    public Button SpawnModButton;
    public Button GodModeButton;
    public Button SpawnAbilitiesButton;
    public Button RandomBuildButton;
    public Button LowGravityButton;
    public Button NoCooldownsButton;
    private bool noCooldowns = false;
    private bool godmode = false;
    private bool lowgravity = false;
    private Player p;
    private GameObject pObj;

    public float newGravity = 8;
    void Start()
    {
        p = Player.instance;
        pObj = p.gameObject;
        CheatPanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            ToggleCheats();
        }
    }
    public void ToggleCheats()
    {
        CheatPanel.SetActive(!CheatPanel.activeSelf);
    }
    public void NextLevel()
    {
        GameObject[] teles = GameObject.FindGameObjectsWithTag("Teleporter");
        foreach (GameObject t in teles)
        {
            float dist = Vector2.Distance(p.transform.position, t.transform.position);
            if (t.name.Contains("Easter") == false && dist < 750)
            {
                t.GetComponent<Teleporter>().TeleportWithAnim();
                break;
            }
        }
    }
    public void BossRoom()
    {
        GameObject tele = GameObject.FindGameObjectWithTag("Teleporter");
        tele.GetComponent<Teleporter>().BossRoomOverride = true;
        tele.GetComponent<Teleporter>().TeleportWithAnim();
    }
    public void SpawnMod()
    {
        GameObject.FindWithTag("BUG-E").GetComponent<BUGE>().FollowingPlayer(true);
        string skillStr = p.GetSkillManager().GetRandomMod().name;
        p.GetSkillManager().SpawnMod(new Vector3(pObj.transform.position.x + 2, pObj.transform.position.y + 5, 0), skillStr);
    }
    public void SpawnAbilities()
    {
        List<SkillObject> list = p.GetSkillManager().GetAllAbilities();
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 newVec = new Vector3(p.transform.position.x + i * 3f, p.transform.position.y + 1, 0);
            p.GetSkillManager().SpawnAbility(newVec, list[i].name);
        }
    }
    public void RandomBuild()
    {
        //give the p 1 random Ability for the first slot
        SkillManager skillManager = p.GetSkillManager();
        skillManager.clearSkills();
        p.GetSkillManager().AddSkill(p.GetSkillManager().GetRandomAbility());
        SkillObject secondAbility = skillManager.GetRandomAbility();
        //give the p a second ability, that isn't the same as the first
        while (skillManager.GetAbilityAmount() < 2)
        {
            p.GetSkillManager().AddSkill(p.GetSkillManager().GetRandomAbility());
        }
        int i = 0;
        while (p.GetSkillManager().GetAmount() < 19 || i > 250)
        {
            if (i == 0)
            { //guarantee you get 2 legendary's
                skillManager.AddSkill(skillManager.GetRandomGolds(1)[0]);
                skillManager.AddSkill(skillManager.GetRandomGolds(1)[0]);
            }
            skillManager.AddSkill(skillManager.GetRandomModByChance());
            i++;
        }
    }
    // the functions below are toggle based cheat codes
    /// *************************************************
    public void NoCooldowns()
    {
        Player.instance.gameObject.GetComponent<AbilityCasting>().cooldownCheatOn = !noCooldowns;
    }
    public void GodMode()
    {
        if (godmode)
        {
            p.GetHealth().SetMaxValue(100);
        }
        else
        {
            p.SetEssence(p.GetMaxEssenceAmount());
            p.SetKeys(3);
            p.GetHealth().SetMaxValue(9999);
        }
        godmode = !godmode;
    }
    public void LowGravity()
    {
        if (lowgravity)
            Player.instance.GetComponent<PlayerController>().gravity = Player.instance.GetComponent<PlayerController>().GetNormalGravity();
        else
            Player.instance.GetComponent<PlayerController>().gravity = newGravity;
    }
}
