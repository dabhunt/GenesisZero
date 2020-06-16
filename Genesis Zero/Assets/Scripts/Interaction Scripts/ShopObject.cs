using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Shop Object")]
public class ShopObject: ScriptableObject
{
    public Sprite Icon;
    public string VisibleName;
    [TextArea]
    public string Description;
    public int Cost;
    [Range(0,3)]
    [Tooltip("0 = Modifier, 1 = Healthtype, 2 = InventoryExpander, 3 = LockPick ")]
    public int Type;
    [Space]
    public float health;
    public float shield;
    public int quantityLeft = 1;
    [Tooltip("Should match the prefix of the name of shop item")]
    public int orderInList = 0;
    public SkillObject mod;
    public void CopyValues(ShopObject item)
    {
        Icon = item.Icon;
        VisibleName = item.VisibleName;
        Description = item.Description;
        Cost= item.Cost;
        Type = item.Type;
        health = item.health;
        shield = item.shield;
        quantityLeft = item.quantityLeft;
        orderInList = item.orderInList;
        mod = item.mod;
    }
}
