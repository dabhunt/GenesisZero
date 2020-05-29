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
    private SpriteRenderer rend;
    public Vector3 StartScale = Vector3.one;
    public Vector3 EndScale = Vector3.one;
    public bool DisableWhileNotCharging = true;
    private Color endColor;
    private Color startColor;
    public float FlashTime = 0.5f; //The warning will flash when there is this much time remaining before attacking (during charging)
    public Color FlashColor = Color.white;
    public float FlashRate = 40f;
    public bool AimAtTarget = false;
    private float localDist = 0.0f;
    public bool UseAiAim = false;
    public bool StayAtProjectilePoint = false;

    private void Awake()
    {
        controller = GetComponentInParent<AIController>();
        rend = GetComponentInChildren<SpriteRenderer>();
        endColor = rend.color;
        startColor = new Color(endColor.r, endColor.g, endColor.b, 0.0f);
        if (controller != null)
        {
            if (controller.BehaviorProperties != null)
            {
                FlashTime = Mathf.Min(FlashTime, controller.BehaviorProperties.AttackChargeTime);
            }
            localDist = (controller.Origin - transform.localPosition).magnitude;
        }

        if (AimAtTarget)
        {
            transform.parent = null;
        }
    }

    private void Update()
    {
        if (controller == null) { Destroy(gameObject); }
        if (controller.BehaviorProperties == null) { return; }

        float normTime = CalculateNormalizedTime();
        transform.localScale = Vector3.Lerp(StartScale, EndScale, normTime);

        if (AimAtTarget && controller.Target != null)
        {
            if (StayAtProjectilePoint)
            {
                Vector3 targetDir = UseAiAim ? controller.GetAimDirection() : (controller.Target.position - controller.GetProjectilePoint()).normalized;
                transform.position = controller != null ? controller.GetProjectilePoint() : transform.position;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(-targetDir.y, targetDir.x, 0.0f));
            }
            else
            {
                Vector3 targetDir = UseAiAim ? controller.GetAimDirection() : (controller.Target.position - controller.GetOrigin()).normalized;
                transform.position = controller.GetOrigin() + targetDir * localDist;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(-targetDir.y, targetDir.x, 0.0f));
            }
        }

        if (rend != null)
        {
            if (normTime < 0.99f)
            {
                rend.color = Color.Lerp(startColor, endColor, normTime);
            }
            else
            {
                rend.color = Color.Lerp(FlashColor, endColor, (Mathf.Sin(Time.unscaledTime * FlashRate) + 1.0f) * 0.5f);
            }

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

    private float CalculateNormalizedTime()
    {
        if (controller != null)
        {
            if (controller.BehaviorProperties != null && controller.GetState() == AIController.AIState.Charge)
            {
                return Mathf.Clamp01(controller.GetStateTime() / Mathf.Max(0.001f, controller.BehaviorProperties.AttackChargeTime - FlashTime));
            }
        }
        return 0.0f;
    }
}
