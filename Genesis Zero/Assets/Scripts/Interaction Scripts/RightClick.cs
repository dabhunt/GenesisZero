using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class RightClick : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    private Player player;
    private SkillManager sManager;
    private bool pointerInside;
    private Button button;
    //private EventSystem eventSystem;
    private void Start()
    {
        player = Player.instance;
        sManager = player.GetSkillManager();
        button = GetComponent<Button>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (pointerInside)
                RemoveStack();
        }
    }
    public void RemoveStack()
    {
        GetComponent<SimpleTooltip>().HideTooltip();
        SkillObject s = GetComponent<SkillUIElement>().Skill;
        if (s.IsAbility)
            return;
        sManager.RemoveSkill(s);
        GameObject mod = sManager.SpawnMod(player.transform.position, s.name);
        mod.GetComponent<SkillPickup>().SetDropped(true);
        Collider[] cols = Physics.OverlapSphere(mod.transform.position, 3);
        foreach (Collider c in cols)
        {
            if (c.gameObject.GetComponent<ModConverter>() != null && c.gameObject.GetComponent<ModConverter>().isActive) // if one of the colliders that overlaps has a mod converter that is also active
            {
                mod.GetComponent<SkillPickup>().SetTarget(c.gameObject); //set target to the scrap converter
            }
        }
        Destroy(mod.GetComponent<InteractPopup>());
    }
}
