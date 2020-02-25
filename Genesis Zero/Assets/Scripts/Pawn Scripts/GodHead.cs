using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodHead : MonoBehaviour
{
    [Header("General Settings")]
    public float interactRange = 5f;
    private GameInputActions inputActions;
    private GameObject player;
    private GameObject canvas;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // if it's in interact range -> enable UI state
            var range = Vector3.Distance(player.transform.position, transform.position);
            Debug.Log("Range: " + range);
            if (range <= interactRange)
            {
                var tmp = StateManager.instance.GetComponent<SacrificeUI>();
                if (!tmp.enabled)
                    tmp.enabled = !tmp.enabled;
            }
        }
    }
}
