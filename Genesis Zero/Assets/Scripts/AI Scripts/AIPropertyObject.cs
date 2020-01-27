using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Create Behavior Property")]
/**
 * Justin Couch
 * ScriptableObject acting as a serializable container for AI behavior properties.
 * Attach to AIController script on an enemy.
 */
public class AIPropertyObject : ScriptableObject
{
    public float DetectRadius = 1.0f;
    public float AvoidRadius;
    public float AttackChargeTime = 1.0f;
    public float AttackCooldownTime = 1.0f;
}
