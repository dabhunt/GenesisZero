﻿/* CharacterController class deals with general input processing for
 * movements, aiming, shooting.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 35f;
    public float jumpStrength = 12f;
    public float doubleJumpStrength = 10f;
    public float horCastPadding = 0.1f; // offset used for the sides casts
    public float vertCastPadding = 0.1f; // offset used for the feet/head casts
    public float rollDistance = 5f;
    public float rollSpeedMult = 1.5f;
    public bool debug;
    public LayerMask immoveables; //LayerMask for bound checks

    [Header("Physics")]
    public float gravity = 18f;
    public float terminalVel = 24f;
    public float fallSpeedMult = 1.45f;
    public float airControlMult = 0.5f;
    public float airSpeedMult = 0.85f;
    public float slopeRayDistMult = 1.25f;

    [Header("Camera")]
    public Camera mainCam;

    [Header("Gun")]
    public GameObject gunObject;
    public GameObject crosshair;
    public float sensitivity;

    //Component parts used in this Scripts
    private PlayerInputActions inputActions;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;

    //This chunk relates to movements
    private Vector2 movementInput;
    private RaycastHit groundHitInfo;
    private Vector3 moveVec = Vector3.right;
    private float maxSpeed;
    private float vertVel;
    private float currentSpeed = 0;
    private bool canDoubleJump = false;
    private float distanceRolled = 0;
    private int rollDirection;
    private Vector3 lastRollingPosition;


    //This chunk relate to aim
    private Vector2 aimInputMouse;
    private Vector2 aimInputController;
    private Vector3 aimVec = Vector3.zero;
    private float fireInput;

    //This chunk of variables is related to Animation/state
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private bool isRolling;
    private bool isLookingRight;

    private Gun gun;
    

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.AimMouse.performed += ctx => aimInputMouse = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.AimController.performed += ctx => aimInputController = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.Fire.performed += ctx => fireInput = ctx.ReadValue<float>();
        animator = GetComponent<Animator>();
        gun = GetComponent<Gun>();
    }

    private void Start()
    {
        if (GetComponent<Player>() != null){
            maxSpeed = GetComponent<Player>().GetSpeed().GetValue();
        }
    }

    private void Update()
    {
        AnimStateUpdate();
        //LogDebug();
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyGravity();
        Aim();
        UpdateDodgeRoll();
        Move();
        UpdateJump();
        DrawDebugLines();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    /* This controls the character general movements
     * It updates the movement vector every frame then apply
     * it based on the input
     */
    private void Move()
    {
        float multiplier = isGrounded ? 1 : airControlMult;
        float startVel = currentSpeed;
        // this is to deal with left stick returning floats
        var input = movementInput.x < 0 ? Mathf.Floor(movementInput.x) : Mathf.Ceil(movementInput.x);
        maxSpeed = GetComponent<Player>().GetSpeed().GetValue();
        
        CalculateForwardDirection();
        if (isRolling) return;

        if (input != 0)
        {
            currentSpeed += input * multiplier * acceleration * Time.fixedDeltaTime;
            if (isGrounded)
                currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
            else
                currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * airSpeedMult, maxSpeed * airSpeedMult);
            
            if (currentSpeed > 0 && IsBlocked(Vector3.right))
            {
                currentSpeed = 0;
                return;
            }
            if (currentSpeed < 0 && IsBlocked(Vector3.left))
            {
                currentSpeed = 0;
                return;
            }
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= acceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            if (currentSpeed < 0)
            {
                currentSpeed += acceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }
        }
        transform.position += moveVec * (startVel * Time.fixedDeltaTime + (0.5f * acceleration * input) * Mathf.Pow(Time.fixedDeltaTime, 2));
    }

    /* This function is called in Move()
     * to calculate the move vector in order to deal with ramps and such
     */
    private void CalculateForwardDirection()
    {
        if (!isGrounded)
        {
            moveVec = transform.right;
            return;
        }

        moveVec = Vector3.Cross(groundHitInfo.normal, transform.forward);
    }

    /* This function is called with an event invoked
     * when player press jump button to make player jump
     */
    public void Jump(InputAction.CallbackContext ctx)
    {   
        if (ctx.performed)
        {
            //if (isRolling) return;
            if (isGrounded)
            {
                animator.SetTrigger("startJump");
                isJumping = true;
                vertVel = jumpStrength;
            }
            else
            {
                if (canDoubleJump)
                {
                    animator.SetTrigger("startJump");
                    isJumping = true;
                    canDoubleJump = false;
                    vertVel = doubleJumpStrength;
                    if (currentSpeed > 0 && movementInput.x <= 0)
                        currentSpeed = 0;
                    if (currentSpeed < 0 && movementInput.x >= 0)
                        currentSpeed = 0;  
                }
            }
        }
    }

    /* This function is used to update the jump cylce and its behavior
     */
    private void UpdateJump()
    {
        if (!isJumping) return;
        float startVel = vertVel;

        if (IsBlocked(Vector3.up))
            vertVel = -1;
        if (vertVel > 0)
        {
            vertVel -= gravity * Time.fixedDeltaTime;
            transform.position += Vector3.up * (startVel * Time.fixedDeltaTime + (0.5f * -gravity) * Mathf.Pow(Time.fixedDeltaTime, 2));
        }
        else
        {
            isJumping = false;
        }
    }

    /* This function is used to get information about the player
     * relative to the ground, it also adjust the player's position
     * if the player is somehow stuck underground
     */
    private void CheckGround()
    {
        RaycastHit hit;

        if (IsBlocked(Vector3.down))
        {
            isGrounded = true;
            canDoubleJump = true;

            if (groundHitInfo.distance < (capsuleCollider.height * 0.5) - capsuleCollider.radius + vertCastPadding)
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * ((capsuleCollider.height * 0.5f) - capsuleCollider.radius), 5 * Time.fixedDeltaTime);

            if (!isJumping)
                vertVel = 0;
        }
        else
        {
            isGrounded = false;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, ((capsuleCollider.height * 0.5f) * slopeRayDistMult), immoveables));
                if (hit.normal == Vector3.up)
                    return;
                else
                    groundHitInfo = hit;
        }
    }

    /* This fuction applies gravity to player
     * when the player is falling
     */
    private void ApplyGravity()
    {
        float startVel = vertVel;
        if (isGrounded)
            return;

        if (!isJumping)
        {
            vertVel -= gravity * fallSpeedMult * Time.fixedDeltaTime;
            vertVel = Mathf.Clamp(vertVel, -terminalVel, 0);
            transform.position += Vector3.up * (startVel * Time.fixedDeltaTime + (0.5f * fallSpeedMult * -gravity) * Mathf.Pow(Time.fixedDeltaTime, 2));
        }
    }

    /* This function is invoked when player press 
     * roll button for dodgerolling. 
     * It will put the player into rolling state
     */
    public void DodgeRoll(InputAction.CallbackContext ctx)
    {
        //Events triggers multiple time
        // this condition is to make sure it only activate once
        if (ctx.performed)
        {
            animator.SetTrigger("startRoll");
            isRolling = true;
            distanceRolled = 0;
            lastRollingPosition = transform.position;
            if (movementInput.x != 0)
                rollDirection = movementInput.x > 0 ? 1 : -1;
            else
                rollDirection =  isLookingRight ? 1 : -1;
            //Shrink*
            capsuleCollider.enabled = false;
            sphereCollider.enabled = true;
        }
    }

    /* This function keeps track of rolling state
     * and make player exit rolling state if necessary
     */
    private void UpdateDodgeRoll()
    {
        float rollSpeed = maxSpeed * rollSpeedMult;
        if (isRolling)
        {
            //interupts roll if it's blocked
            if ((rollDirection > 0 && IsBlocked(Vector3.right)) || (rollDirection < 0 && IsBlocked(Vector3.left)))
            {   
                isRolling = false;
            }

            transform.position += moveVec * rollDirection * rollSpeed * Time.fixedDeltaTime;
            //continue rolling if rollDistance is not reached
            if (distanceRolled < rollDistance)
            {
                distanceRolled += Vector3.Distance(transform.position, lastRollingPosition);
            }
            else
            {
                //make sure player doesn't get stuck under blocks
                if (IsBlocked(Vector3.up))
                    distanceRolled = rollDistance - sphereCollider.radius;
                else
                    isRolling = false;
            }
            lastRollingPosition = transform.position;
        }
        else
        {
            //unShrink*
            if (!capsuleCollider.enabled)
                capsuleCollider.enabled = true;
            if (sphereCollider.enabled)
                sphereCollider.enabled = false;
        }
    }

    /* This function control character aiming
     * Crosshair is moved using mouse/rightStick
     * Gun rotates to point at crosshair
     */
    private void Aim()
    {
        Vector3 pos = mainCam.WorldToScreenPoint(transform.position);
        aimVec.x = aimInputMouse.x - pos.x; 
        aimVec.y = aimInputMouse.y - pos.y;

        float tmpAngle = Mathf.Atan2(aimVec.y, aimVec.x) * Mathf.Rad2Deg;
        gunObject.transform.localRotation = Quaternion.Euler(0, 0, tmpAngle);
        if (tmpAngle < 89 && tmpAngle > -91)
            isLookingRight = true;
        else
            isLookingRight = false;

        if (fireInput > 0)
            gun.Shoot();
    }

    /* This function checks if the model is blocked
     * in a certain direction(argument == Vector3.up/Vector3.down.....)
     */
    private bool IsBlocked(Vector3 dir)
    {
        bool isBlock = false;
        RaycastHit hit;
        
        float radius = capsuleCollider.radius - vertCastPadding;
        float maxDist = (capsuleCollider.height / 2) - radius + vertCastPadding;
        Vector3 rayCenter = isRolling ? transform.position - sphereCollider.center : transform.position;
        float boxHalf = isRolling ? (sphereCollider.radius / 2) - horCastPadding : (capsuleCollider.height / 2) - horCastPadding;
        Vector3 halfExtends = new Vector3(0, boxHalf, 0);

        if (dir == Vector3.up)
        {
            isBlock = Physics.SphereCast(transform.position, radius, Vector3.up, out hit, maxDist, immoveables, QueryTriggerInteraction.UseGlobal);
            if (isBlock && hit.collider.isTrigger)
                isBlock = false;
        }
        else if (dir == Vector3.down)
        {
            isBlock = Physics.SphereCast(transform.position, radius, Vector3.down, out groundHitInfo, maxDist, immoveables, QueryTriggerInteraction.UseGlobal);
            if (isBlock && groundHitInfo.collider.isTrigger)
                isBlock = false;
        }
        else if (dir == Vector3.right)
        {
            isBlock = Physics.BoxCast(rayCenter, halfExtends, Vector3.right, out hit, Quaternion.identity, capsuleCollider.radius + vertCastPadding, immoveables, QueryTriggerInteraction.UseGlobal);
            if (isBlock && hit.collider.isTrigger || hit.normal != Vector3.left)
                isBlock = false;
        }
        else if (dir == Vector3.left)
        {
            isBlock = Physics.BoxCast(rayCenter, halfExtends, Vector3.left, out hit, Quaternion.identity, capsuleCollider.radius + vertCastPadding, immoveables, QueryTriggerInteraction.UseGlobal);
            if (isBlock && hit.collider.isTrigger || hit.normal != Vector3.right)
                isBlock = false;
        }
        else
        {
            Debug.LogError("Invalid Vector used in IsBlocked");
        }
        return isBlock;
    }

    /* This function updates information
     * about the player's state for animations
     */
    private void AnimStateUpdate()
    {
        var xSpeed = currentSpeed != 0 ? currentSpeed / maxSpeed : 0;
        var ySpeed = vertVel;
        animator.SetFloat("xSpeed", xSpeed);
        animator.SetFloat("ySpeed", ySpeed);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRolling", isRolling);
        animator.SetBool("isLookingRight", isLookingRight);
    }
    
    private void DrawDebugLines()
    {
        if (!debug) return;
        Debug.DrawLine(transform.position, transform.position + moveVec * 2, Color.red);
    }

    private void LogDebug()
    {
        //Debug.Log(movementInput);
        //Debug.Log("blockedRight?:" + IsBlocked(Vector3.right));
        //Debug.Log("blockedLeft?:" + IsBlocked(Vector3.left));
        //Debug.Log(rb.velocity.y);
        //Debug.Log("lastRollingPosition: " + lastRollingPosition);
        //Debug.Log(IsGrounded());
        //Debug.Log(canDoubleJump);
        //Debug.Log("distanceRolled: " + distanceRolled);
    }
}
