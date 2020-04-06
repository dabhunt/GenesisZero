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
}
