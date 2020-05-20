using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName = "Credit Card")]
public class CreditCard : ScriptableObject
{
    [TextArea]
    public string Role;
    public string Name;
    public Image Image;

    [Tooltip("Words to highlight in different color, Comma seperated list")]
    public string WordsToHighlight;
    [Space]
    //public variables below here have default values if they are set to 0 in inspector
    [Header("Optional Variables to override defaults")]
    public Color MainTextColor = Color.white;
    public Color HighlightTextColor;
    public float ShowDuration;
    public float FadeDuration;
    public float TextInactiveDuration;
}
