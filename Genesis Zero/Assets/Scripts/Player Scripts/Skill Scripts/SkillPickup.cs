using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPickup : MonoBehaviour
{
    [Header("Mod or Ability")]
    public SkillObject skill;
    private bool added;
    private bool isMod;
    private bool pressed;
    private float pickupDist = 4f;
    private float speedvar = 4f;
    private GameObject target;
    private Player player;
    private bool dropped = false;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        player = target.GetComponent<Player>();
        if (skill == null)
        {
            skill = player.GetSkillManager().GetRandomModByChance();
            VFXManager.instance.ChangeColor(this.gameObject, player.GetSkillManager().GetColor(skill));
        }
        else if (GetComponent<SimpleTooltip>() != null)
        {
            GetComponent<SimpleTooltip>().infoLeft = skill.SimpleDescription;
        }
        isMod = skill.IsAbility ? false : true;

        if (skill != null)
        {
            if (skill.IsAbility)
                VFXManager.instance.ChangeColor(this.gameObject, new Color(0, .11f, .18f, .66f));
            else
                VFXManager.instance.ChangeColor(this.gameObject, player.GetSkillManager().GetColor(skill));
        }

    }
    private void Update()
    {
        //if the player dropped the skill using right click
        if (dropped)
        {
            target = GetComponent<InteractInterface>().ClosestInteractable();
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            //if the player presses F within range, it will be pulled towards them
            if (Vector3.Distance(player.transform.position, transform.position) <= pickupDist )
            {
                pressed = true;
            }
        }
        //if the player has no more room for new modifiers, tell them
        if (!skill.IsAbility)
        {
            if (player.GetSkillManager().GetModAmount() >= player.GetSkillManager().GetModLimit())
            {
                GetComponent<InteractPopup>().SetText("Mod Limit Reached. Drop Unwanted Modifiers w/ Right Click");
                pressed = false;
            }
        }
        else
        {
            //if this skill is an ability, and the player has no room for new abilities change the text
            if (player.GetSkillManager().GetAbilityAmount() > 1)
                GetComponent<InteractPopup>().SetText("Press [F] to Replace Ability 1");
            if (player.GetSkillManager().HasSkill(skill.name) == true)
            { //if the ability is a duplicate, change the text to say you can't pick it up
                GetComponent<InteractPopup>().SetText("Duplicate Abilities cannot be picked up");
                pressed = false;
            }
        }
    }
    private void FixedUpdate()
    {
        if (target != null)
        {
            //if player is the target, pressed must also be true, if the target is ScrapConverter, it flies there regardless
            if ((target.GetComponent<Player>() != null && pressed ) || (target.GetComponent<Player>() == null && target.GetComponent<ModConverter>().isActive))
            {  // Force pull for pickup
                if (GetComponentInChildren<Floating>() != null)
                    Destroy(GetComponentInChildren<Floating>());
                speedvar *= 1.09f;
                Vector3 tVec = new Vector3(target.transform.position.x, target.transform.position.y + .9f, 0);
                transform.LookAt(tVec);
                this.transform.position = Vector3.MoveTowards(this.transform.position, tVec, speedvar * Time.deltaTime);
            }
        }
        if (added == true)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (added)
            return;
        if (pressed)
        {
            if (other.GetComponentInParent<Player>() || other.GetComponent<Player>())
            {
                Player p = other.GetComponent<Player>();
                if (other.GetComponentInParent<Player>())
                {
                    p = other.GetComponentInParent<Player>();
                }

                if (skill != null && added == false)
                {
                    // There are more than two abilities and a new ability is here
                    if (skill.IsAbility && !p.GetSkillManager().HasSkill(skill.name) && p.GetSkillManager().GetAbilityAmount() >= 2)
                    {
                        // Prompt choice to swap out with current abilities
                        SkillObject otherskill = p.GetSkillManager().GetAbility1();
                        p.GetSkillManager().RemoveSkill(otherskill);
                        GameObject ability = p.GetSkillManager().SpawnAbility(other.transform.position, otherskill.name);
                        p.GetSkillManager().AddSkill(skill);
                        ability.GetComponent<InteractPopup>().SetText("Press [F] to Replace Ability 1");
                        //GameObject.FindObjectOfType<SkillDisplay>().CheckUpdate();
                        added = true;
                    }
                    else
                    {
                        p.GetSkillManager().AddSkill(skill);
                        added = true;
                    }
                }

            }

        }
        //this section of code deals with what the mod should do once it reaches a ScrapConverter
        if (other.GetComponent<ModConverter>())
        {
            ModConverter mc = other.GetComponent<ModConverter>();
            mc.AddMod(skill);
            added = true;
        }
    }
    public void SetDropped(bool boo)
    {
        dropped = boo;
    }

}
