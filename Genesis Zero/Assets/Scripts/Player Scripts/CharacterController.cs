using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [System.Serializable]
    public class MovementSettings
    {
        public float movementSpeed = 12f;
        public float jumpStrength = 2f;
        public float distToGround = 1f;
        public LayerMask ground;
    }
    [System.Serializable]
    public class PhysicsSettings
    {
        //this is for later use to replace the default gravity
        public float gravity = 1f;
    }

    //References to the different settings objects
    public MovementSettings movementSettings;
    public MovementSettings physicsSettings;
    private PlayerInputActions inputActions;

    Rigidbody rb;
    Vector2 movementInput;
    Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
        MovementSettings moveSetting = new MovementSettings();

        inputActions.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Move()
    {
        velocity.x = movementInput.x * movementSettings.movementSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + velocity);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, movementSettings.distToGround, movementSettings.ground);
    }
    public void Jump()
    {
        //Debug.Log(IsGrounded());
        if (IsGrounded())
        {
            rb.AddForce(new Vector2(0f, movementSettings.jumpStrength), ForceMode.Impulse);
        }
    }
}
