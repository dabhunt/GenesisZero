/* CharacterController class deals with general input processing for
 * movements, aiming, shooting.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 3f;
    public float maxJumpSpeed = 12f;
    public float jumpHeight = 6f;
    public float doubleJumpStrength = 10f;
    public float bodyHeightOffset = -0.1f; // offset used for the sides casts
    public float bodyWidthOffset = 0.05f; // offset used for the feet/head casts
    public float rollDistance = 5f;
    public float rollSpeedMult = 1.5f;
    public float maxGroundAngle = 120f;
    public bool debug;
    public LayerMask immoveables; //LayerMask for bound checks

    private RaycastHit groundHitInfo;
    private Vector3 moveVec = Vector3.right;
    private float groundAngle;


    [Header("Physics")]
    public float gravity = 18f;
    public float terminalVel = 15;
    public float fallSpeedMult = 1.45f;
    public float airControlMult = 0.5f;
    public float slopeRayDistMult = 1.25f;
    public float slopeForceMult = 0.35f;

    [Header("Camera")]
    public Camera mainCam;

    [Header("Gun")]
    public GameObject gunObject;
    public GameObject crosshair;
    public float sensitivity;

    //Component parts used in this Scripts
    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;

    //This chunk relates to movement/aim values
    private Vector2 movementInput;
    private float fireInput;
    private Vector2 aimInputMouse;
    private Vector2 aimInputController;
    private Vector3 aimVec = Vector3.zero;
    private float maxSpeed;
    private float vertVel;
    private float currentSpeed = 0;
    private bool canDoubleJump = false;
    private float distanceRolled = 0;
    private float jumpApexY;
    private int rollDirection;
    private Vector3 lastRollingPosition;

    //This chunk of variables is related to Animation
    private Animator animator;
    private Gun gun;
    private bool isGrounded;
    private bool isJumping;
    private bool isRolling;
    private bool isLookingRight;
    private bool gunFired;
    private bool onSlope;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        maxSpeed = GetComponent<Player>().GetSpeed().GetValue();
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
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void CalculateForwardDirection()
    {
        if (!isGrounded)
        {
            moveVec = transform.right;
            return;
        }

        moveVec = Vector3.Cross(groundHitInfo.normal, transform.forward);
        Debug.Log(moveVec);
    }

    private void CalculateGroundAngle()
    {
        if (!isGrounded)
        {
            groundAngle = 90;
            return;
        }
        groundAngle = Vector3.Angle(groundHitInfo.normal, transform.right);
    }

    private void CheckGround()
    {
        if (IsBlocked(Vector3.down))
        {
            isGrounded = true;
            canDoubleJump = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    /* This controls the character general movements
     * It updates the movement vector every frame then apply
     * it based on the input
     */
    private void Move()
    {
        RaycastHit hit;
        float multiplier = isGrounded ? 1 : airControlMult;
        // this is to deal with left stick returning floats
        var input = movementInput.x < 0 ? Mathf.Floor(movementInput.x) : Mathf.Ceil(movementInput.x);
        maxSpeed = GetComponent<Player>().GetSpeed().GetValue();

        CalculateForwardDirection();

        if (groundAngle >= maxGroundAngle) return;

        if (isRolling) return;
        if (input > 0)
        {

        }
        else if (input < 0)
        {

        }
        else
        {
            
        }
        currentSpeed += multiplier * acceleration * Time.fixedDeltaTime;
        Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
        Debug.Log(currentSpeed);
        transform.position += moveVec * currentSpeed * Time.fixedDeltaTime;
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
                jumpApexY = transform.position.y + jumpHeight;
                Debug.Log(jumpApexY);
            }
        }
    }

    private void UpdateJump()
    {
        if (!isJumping) return;
        if (transform.position.y < jumpApexY)
            transform.position += Vector3.up * maxJumpSpeed * Time.fixedDeltaTime;
        else
            isJumping = false;
    }

    /* This fuction applies gravity to player
     * when the player's off the ground
     */
    private void ApplyGravity()
    {
        if (!isGrounded && !isJumping)
        {
            transform.position += Vector3.down * gravity * Time.fixedDeltaTime;
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
        if (isRolling)
        {
            //continue rolling if rollDistance is not reached
            if (distanceRolled < rollDistance)
            {
                currentSpeed = rollDirection * maxSpeed * rollSpeedMult;
                distanceRolled += Vector3.Distance(transform.position, lastRollingPosition);
            }
            else
            {
                currentSpeed = isGrounded ? 0 : currentSpeed;
                //make sure player doesn't get stuck under blocks
                if (IsBlocked(Vector3.up))
                    distanceRolled = rollDistance - sphereCollider.radius;
                else
                    isRolling = false;
            }

            //interupts roll if it's blocked
            if ((rollDirection > 0 && IsBlocked(Vector3.right)) || (rollDirection < 0 && IsBlocked(Vector3.left)))
            {   
                isRolling = false;
                if (currentSpeed != 0)
                    currentSpeed = 0;
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
        transform.position += moveVec * currentSpeed * Time.fixedDeltaTime;
    }

    /* This function just return a RaycastHit that
     * will be used to determine if a player is going up/down a slope
     */
    private RaycastHit OnSlope()
    {
        RaycastHit hit;
        float maxDist = slopeRayDistMult * capsuleCollider.height / 2;
        if (!isGrounded)
            onSlope = false;
        if(Physics.SphereCast(transform.position, capsuleCollider.radius, Vector3.down, out hit, maxDist, immoveables, QueryTriggerInteraction.UseGlobal))
            if (hit.normal != Vector3.up)
                onSlope = true;
            else
                onSlope = false;
        return hit;
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
        
        float radius = capsuleCollider.radius - bodyWidthOffset;
        float maxDist = (capsuleCollider.height / 2) - radius + bodyWidthOffset;
        Vector3 rayCenter = isRolling ? transform.position - sphereCollider.center : transform.position;
        float boxHalf = isRolling ? (sphereCollider.radius / 2) + bodyHeightOffset : (capsuleCollider.height / 2) + bodyHeightOffset;
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
            isBlock = Physics.BoxCast(rayCenter, halfExtends, Vector3.right, out hit, Quaternion.identity, capsuleCollider.radius + bodyWidthOffset, immoveables, QueryTriggerInteraction.UseGlobal);
            if (isBlock && hit.collider.isTrigger || hit.normal != Vector3.left)
                isBlock = false;
        }
        else if (dir == Vector3.left)
        {
            isBlock = Physics.BoxCast(rayCenter, halfExtends, Vector3.left, out hit, Quaternion.identity, capsuleCollider.radius + bodyWidthOffset, immoveables, QueryTriggerInteraction.UseGlobal);
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
        var xSpeed = moveVec.x != 0 ? moveVec.x / maxSpeed : 0;
        var ySpeed = moveVec.y;
        animator.SetFloat("xSpeed", xSpeed);
        animator.SetFloat("ySpeed", ySpeed);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRolling", isRolling);
        animator.SetBool("isLookingRight", isLookingRight);
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
