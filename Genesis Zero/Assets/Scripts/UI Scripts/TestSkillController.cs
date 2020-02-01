using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class TestSkillController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Player p = GetComponent<Player>();
        if (Input.GetKeyDown(KeyCode.F))
        {
            p.GetSkillManager().GetRandomSkill();
        }
    }
}
