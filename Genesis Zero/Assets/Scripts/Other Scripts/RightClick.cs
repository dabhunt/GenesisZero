using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class RightClick : MonoBehaviour
{
    private Player player;
    private SkillManager sManager;
    private void Start()
    {
        player = FindObjectOfType<Player>();
        sManager = player.GetSkillManager();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("removing stacks...");
            RemoveStack();
        }

    }
    public void RemoveStack()
    {
        SkillObject s = GetComponent<SkillUIElement>().Skill;
        sManager.RemoveSkill(s);
        sManager.SpawnMod(player.transform.position, s.name);
    }
}
