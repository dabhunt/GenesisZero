using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is to demonstrate how Tooltips can be added or changed dynamically.
currently this script serves as an example of how you (Kenny) can add tooltips to display modifier / ability info
you could probably copy this whole void function into your skill manager (or wherever you assign descriptions) and it will work
 */
public class TTDynamicExample : MonoBehaviour
{
    public SimpleTooltipStyle dynamicTooltipStyle;
    public void AddTooltip(string toolText)
    {
        // So first lets check if we already have a tooltip component on the object
        var tooltip = GetComponent<SimpleTooltip>();
        if (tooltip)
        {
            // if you wanted to change the info after it had already been set at some point
            //tooltip.infoLeft = "You can also change the text after. Remember that ~tags `still !work";
            tooltip.infoLeft = toolText;

            // Forces to start showing instead of waiting for the mouse to enter the collider
            tooltip.ShowTooltip(); 
        }
        // If the object doesn't have a tooltip, we add a new one
        else
        {
            tooltip = gameObject.AddComponent<SimpleTooltip>();
            tooltip.infoLeft = toolText;
            tooltip.ShowTooltip(); // Force to show tooltip even if not hovered

            // If you wish, you may also change the style too, or else it will use the default one (I already made the default style for us)
            if (dynamicTooltipStyle)
                tooltip.simpleTooltipStyle = dynamicTooltipStyle;
        }
    }
}
