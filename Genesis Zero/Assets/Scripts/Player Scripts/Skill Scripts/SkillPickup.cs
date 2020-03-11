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
    private float attractDist = 4f;
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
    {
        if (Input.GetKey(KeyCode.F))
        {
            //if the player presses F within range, it will be pulled towards them
            if (Vector3.Distance(player.transform.position, transform.position) <= attractDist)
                pressed = true;
        }
    }
    private void FixedUpdate()
    {
        if (target != null && pressed){
            // Force pull for pickup
            if (GetComponent<Floating>() != null)
                Destroy(GetComponent<Floating>());
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= attractDist)
            {
                Vector3 direction = target.transform.position - transform.position;
                direction.y += 1;
                GetComponent<Rigidbody>().AddForce((direction) * (1 - (distance / 4)) / 2, ForceMode.Impulse);
            }
            else if (GetComponent<Rigidbody>().velocity.magnitude > 12)
            {
                float mag = GetComponent<Rigidbody>().velocity.magnitude;
                GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * (mag *.9f);
                //GetComponent<Rigidbody>().velocity *= .3f;
            }
        }

        if (added == true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
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
                    // Pop-up prompt UI stuff for when there are dupicates
                }
                else
                {
                    // There are more than two abilities and a new ability is here
                    if (skill.IsAbility && !p.GetSkillManager().HasSkill(skill.name) && p.GetSkillManager().GetAbilityAmount() >= 2)
                    {
                        // Prompt choice to swap out with current abilities
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            SkillObject otherskill = p.GetSkillManager().GetAbility1();                          
                            p.GetSkillManager().RemoveSkill(otherskill);
                            p.GetSkillManager().SpawnAbility(other.transform.position, otherskill.name);
                            p.GetSkillManager().AddSkill(skill);
                            //GameObject.FindObjectOfType<SkillDisplay>().CheckUpdate();
                            added = true;
                        }
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
