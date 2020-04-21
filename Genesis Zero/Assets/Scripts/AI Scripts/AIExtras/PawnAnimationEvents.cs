using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Justin Couch
 * PawnAnimationEvents is used to allow the animator control the Pawn class
 */
public class PawnAnimationEvents : MonoBehaviour
{
    private Pawn parentPawn;

    /**
     * Sets the parent pawn
     */
    public void SetParentPawn(Pawn p)
    {
        parentPawn = p;
    }

    /**
     * Used to allow the animator to destroy the gameobject
     */
    public void Destroy()
    {
        if (parentPawn != null)
        {
            Destroy(parentPawn.gameObject);
        }
    }
}
