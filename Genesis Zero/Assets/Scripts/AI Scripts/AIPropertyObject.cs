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
    public float DetectRadius;
    public float AvoidRadius;
    public float AttackChargeTime;
    public float AttackCooldownTime;
}
