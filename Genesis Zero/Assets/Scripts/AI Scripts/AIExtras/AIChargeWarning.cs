using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * This class controls warning effects for enemy attacks.
 */
public class AIChargeWarning : MonoBehaviour
{
    private AIController controller;
    private Renderer rend;
    public Vector3 StartScale = Vector3.one;
    public Vector3 EndScale = Vector3.one;
    public bool DisableWhileNotCharging = true;
    private Color endColor;
    private Color startColor;

    private void Awake()
    {
        controller = GetComponentInParent<AIController>();
        rend = GetComponentInChildren<Renderer>();
        endColor = rend.material.color;
        startColor = new Color(0, 0, 0, 0);
    }

    private void FixedUpdate()
    {
        if (controller == null) { return; }

        transform.localScale = Vector3.Lerp(StartScale, EndScale, controller.GetNormalizedChargeTime());

        if (rend != null)
        {
            rend.material.color = Color.Lerp(startColor, endColor, controller.GetNormalizedChargeTime());
            if (controller.GetState() == AIController.AIState.Charge)
            {
                if (!rend.enabled)
                {
                    rend.enabled = true;
                }
            }
            else if (rend.enabled && DisableWhileNotCharging)
            {
                rend.enabled = false;
            }
        }
    }
}
