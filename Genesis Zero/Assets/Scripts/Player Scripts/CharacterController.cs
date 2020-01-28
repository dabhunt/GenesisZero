using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 20f;
    public float jumpStrength = 6f;
    public float distToGround = 1f;
    public LayerMask ground;

    [Header("Physics")]
    public float gravity = 14f;

    [Header("Camera")]
    public Camera mainCam;

    [Header("Gun")]
    public GameObject gun;
    public GameObject crosshair;

    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector2 aimInput;
    private Vector3 moveVec = Vector3.zero;
    private Vector3 lookVec = Vector3.zero;

    private float vertForce;
    private float currentSpeed = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.LookAim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        Aim();
    }

    private void FixedUpdate()
    {
        ApplyDownForce();
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
        if (movementInput.x != 0)
        {
            currentSpeed += movementInput.x * acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(Mathf.Abs(currentSpeed), Mathf.Abs(GetComponent<Player>().GetSpeed().GetValue())) * movementInput.x;
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= acceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }
        }
        moveVec.x = currentSpeed;
        moveVec.y = vertForce;
        rb.velocity = transform.TransformDirection(moveVec);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGround, ground);
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            vertForce = jumpStrength;
        }
    }

    private void ApplyDownForce()
    {
        if (!IsGrounded())
        {
            vertForce -= gravity * Time.deltaTime;
        }
        else
        {
            if (vertForce < 0)
                vertForce = 0;
        }
    }

    private void Aim()
    {
        gun.transform.LookAt(crosshair);
    }
}
