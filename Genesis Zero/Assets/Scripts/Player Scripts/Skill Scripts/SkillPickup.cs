using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPickup : MonoBehaviour
{
    [Header("Mod or Ability")]
    public SkillObject skill;
    // Start is called before the first frame update
    void Start()
    {
        if (skill == null)
        {
            // Add default skill
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>() || other.GetComponent<Player>())
        {
            Player p = other.GetComponent<Player>();
            if (other.GetComponentInParent<Player>())
            {
                p = other.GetComponentInParent<Player>();
            }

            if (skill != null)
            {
                p.GetSkillManager().AddSkill(skill);
            }
            Destroy(gameObject);
        }
    }
}
