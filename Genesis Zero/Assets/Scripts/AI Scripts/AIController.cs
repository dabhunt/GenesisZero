using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * AIControl is the base class controlling the behavior states of enemies and how they interact with the player.
 */
public class AIController : Pawn
{
    public AIPropertyObject BehaviorProperties;

    public enum AIState { Patrolling, Following, Attacking, CoolingDown }
}
