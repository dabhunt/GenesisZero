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
    public float acceleration = .2f;
    public float speedvar = 4f;
    private GameObject target;
    private Player player;
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
        //InvokeRepeating("CheckPopup", 0, .2f);

    }
    private void Update()
    {   //if this skill is an ability, and the player has no room for new abilities change the text
        if (player.GetSkillManager().GetAbilityAmount() > 1 && skill.IsAbility)
            GetComponent<InteractPopup>().SetText("Press [F] to Replace Ability 1");
        if (Input.GetKeyDown(KeyCode.F))
        {
            //if the player presses F within range, it will be pulled towards them
            if (Vector3.Distance(player.transform.position, transform.position) <= pickupDist)
            {
                pressed = true;
                //if the nearby skill is an Ability 
                if (skill.IsAbility)
                {   //if the player doesn't have that ability
                    if (player.GetSkillManager().HasSkill(skill.name) == true)
                    {
                        GetComponent<InteractPopup>().SetText("Duplicate Abilities cannot be picked up");
                        pressed = false;
                    }
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (target != null && pressed){
            // Force pull for pickup
            if (GetComponentInChildren<Floating>() != null)
                Destroy(GetComponentInChildren<Floating>());
            speedvar *= 1.1f;
            Vector3 tVec = new Vector3(target.transform.position.x, target.transform.position.y + .9f, 0);
            transform.LookAt(tVec);
            this.transform.position = Vector3.MoveTowards(this.transform.position, tVec, speedvar * Time.deltaTime);
        }
        if (added == true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
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
                    //Check if it's an ability, cant have more than one
                    if (skill.IsAbility && p.GetSkillManager().HasSkill(skill.name))
                    {
                        GetComponent<InteractPopup>().SetText("Duplicate Abilities cannot be picked up");
                    }
                    else
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
        }
    }

}
