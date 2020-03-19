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
            RemoveStack();
        }

    }
    public void RemoveStack()
    {
        SkillObject s = GetComponent<SkillUIElement>().Skill;
        sManager.RemoveSkill(s);
        GameObject mod = sManager.SpawnMod(player.transform.position, s.name);
        //destroy the mod after 3 seconds to prevent abuse
        mod.AddComponent<DestroyAfterXTime>().time = 3;
        mod.GetComponent<SkillPickup>().SetDropped(true);
        Destroy(mod.GetComponent<InteractPopup>());
    }
}
