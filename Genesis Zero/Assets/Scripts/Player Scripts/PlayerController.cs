/* CharacterController class deals with general input processing for
 * movements, aiming, shooting.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("CharacterDims")]
    public float characterHeight = 1.8f;
    public float characterWidth = 0.3f;

    [Header("Movement")]
    public float acceleration = 35f;
    public float jumpStrength = 12f;
    public float doubleJumpStrength = 10f;
    public float horCastPadding = 0.1f; // offset used for the sides casts
    public float vertCastPadding = 0.1f; // offset used for the feet/head casts
    public float rollDistance = 5f;
    public float rollSpeedMult = 1.5f;
    public float jumpBufferTime = 0.2f;
    public LayerMask immoveables; //LayerMask for bound checks
    public bool debug;

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
    public RectTransform crosshair;
    public Canvas canvasRef;
    public float gamePadSens = 15f;
    private float timeToFire = 0;

    [Header("Animations")]
    public float triggerResetTime = 0.25f;
    //Component parts used in this Script
    private PlayerInputActions inputActions;
    private Player player;
    private OverHeat overheat;
    //This chunk relates to movements
    private Vector2 movementInput;
    private RaycastHit groundHitInfo;
    private Vector3 moveVec = Vector3.right;
    private float maxSpeed;
    private float vertVel;
    private float currentSpeed = 0;
    private float jumpCount = 2;
    private float jumpPressedTime;
    private float distanceRolled = 0;
    private int rollDirection;
    private Vector3 lastRollingPosition;


    //This chunk relate to aim
    private Vector2 aimInputMouse;
    private Vector2 aimInputController;
    private float fireInput;
    private Gun gun;

    //This chunk of variables is related to Animation/state
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private bool isRolling;
    private bool isLookingRight = true;
    private bool isAimingRight;

    //sound variables
    private bool walkSoundPlaying = false;
    private PlayerSounds sound;
    

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.AimMouse.performed += ctx => aimInputMouse = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.AimController.performed += ctx => aimInputController = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.Fire.performed += ctx => fireInput = ctx.ReadValue<float>();
        sound = FindObjectOfType<AudioManager>().GetComponent<PlayerSounds>();
        animator = GetComponent<Animator>();
        gun = GetComponent<Gun>();
        overheat = GetComponent<OverHeat>();
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        player = GetComponent<Player>();
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
        float startVel = Mathf.Abs(currentSpeed);
        // this is to deal with left stick returning floats
        var input = movementInput.x < 0 ? Mathf.Floor(movementInput.x) : Mathf.Ceil(movementInput.x);
        maxSpeed = GetComponent<Player>().GetSpeed().GetValue();
        
        CalculateForwardDirection();
        if (isRolling) return;

        if (isGrounded)
        {
            if(currentSpeed > 0 && !walkSoundPlaying)
            {
                walkSoundPlaying = true;
                sound.Walk();
            }
            else if (currentSpeed <= 0)
            {
                walkSoundPlaying = false;
                sound.StopWalk();
            }
        }
        else
        {
            walkSoundPlaying = false;
            sound.StopWalk();
        }

        if (input != 0)
        {
            if (input > 0 && IsBlocked(Vector3.right))
            {
                currentSpeed = 0;
                return;
            }
            if (input < 0 && IsBlocked(Vector3.left))
            {
                currentSpeed = 0;
                return;
            }

            currentSpeed += multiplier * acceleration * Time.fixedDeltaTime;

            //This is to calculate air control speeds
            if (isGrounded)
                currentSpeed = Mathf.Clamp(currentSpeed, maxSpeed, maxSpeed);
            else
                currentSpeed = Mathf.Clamp(currentSpeed, maxSpeed * airSpeedMult, maxSpeed * airSpeedMult);
            //Rotation depending on input
            if (input > 0 && transform.eulerAngles.y != 90)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                isLookingRight = true;
            }
            if (input < 0 && transform.eulerAngles.y != -90)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
                isLookingRight = false;
            }
            transform.position += moveVec * (startVel * Time.fixedDeltaTime + (0.5f * acceleration * Mathf.Pow(Time.fixedDeltaTime, 2)));
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= acceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
        }
    }

    /* This function is called in Move()
     * to calculate the move vector in order to deal with ramps and such
     */
    private void CalculateForwardDirection()
    {
        if (!isGrounded)
        {
            moveVec = transform.forward;
            return;
        //if they are on the ground, and their speed is greater than .1, and walk sound is not playing, start playing it
        }
        moveVec = Vector3.Cross(groundHitInfo.normal, -transform.right);
    }

    /* This function is called with an event invoked
     * when player press jump button to make player jump
     */
    public void Jump(InputAction.CallbackContext ctx)
    {   
        if (ctx.performed)
        {
            if (isRolling) return;
            jumpPressedTime = jumpBufferTime;
            if (!isJumping && jumpCount > 0)
            {
                jumpCount--;
                animator.SetTrigger("startJump");
                sound.Jump();
                StartCoroutine(ResetTrigger("startJump", triggerResetTime));
                isJumping = true;
                vertVel = jumpStrength;
            }
            else if (isJumping && jumpCount > 0)
            {
                jumpCount--;
                animator.SetTrigger("startDoubleJump");
                StartCoroutine(ResetTrigger("startDoubleJump", triggerResetTime));
                isJumping = true;
                vertVel = doubleJumpStrength;
                if (isLookingRight && movementInput.x <= 0)
                    currentSpeed = 0;
                if (!isLookingRight && movementInput.x >= 0)
                    currentSpeed = 0;
            }
        }
    }

    /* This function is used to update the jump cylce and its behavior
     */
    private void UpdateJump()
    {
        if (jumpPressedTime > 0)
            jumpPressedTime -= Time.fixedDeltaTime;
        else
            jumpPressedTime = 0;

        if (!isJumping)
        {
            if (isGrounded && jumpPressedTime > 0)
            {
                jumpPressedTime = 0;
                isJumping = true;
                vertVel = jumpStrength;
            }
            return;
        }

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
            if (Physics.Raycast(transform.position, Vector3.down, out hit, characterHeight * 0.5f, immoveables))
                if (hit.distance < 0.5f * characterHeight + vertCastPadding)
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * characterHeight * 0.5f, 5 * Time.fixedDeltaTime);

            if (!isJumping)
            {
                vertVel = 0;
                jumpCount = 2;
            }
        }
        else
        {
            isGrounded = false;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, characterHeight * 0.5f * slopeRayDistMult, immoveables))
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
            StartCoroutine(ResetTrigger("startRoll", triggerResetTime));
            isRolling = true;
            distanceRolled = 0;
            lastRollingPosition = transform.position;
            if (movementInput.x != 0)
                rollDirection = movementInput.x > 0 ? 1 : -1;
            else
                rollDirection =  isAimingRight ? 1 : -1;

            if (rollDirection < 0 && isLookingRight)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
                isLookingRight = false;
            }
            if (rollDirection > 0 && !isLookingRight)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                isLookingRight = true;
            }

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
                animator.SetTrigger("endRoll");
                StartCoroutine(ResetTrigger("endRoll", triggerResetTime));
            }

            transform.position += moveVec * rollSpeed * Time.fixedDeltaTime;
            //continue rolling if rollDistance is not reached
            if (distanceRolled < rollDistance)
            {
                distanceRolled += Vector3.Distance(transform.position, lastRollingPosition);
            }
            else
            {
                //make sure player doesn't get stuck under blocks
                if (IsBlocked(Vector3.up))
                    distanceRolled = rollDistance - 0.5f;
                else
                {
                    isRolling = false;
                    animator.SetTrigger("endRoll");
                    StartCoroutine(ResetTrigger("endRoll", triggerResetTime));
                }
            }
            lastRollingPosition = transform.position;
        }
    }

    /* This function control character aiming
     * Crosshair is moved using mouse/rightStick
     * Gun rotates to point at crosshair
     */
    private void Aim()
    {   
        Vector2 pos;
        Vector3 worldPos;
        float aimAngle;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRef.transform as RectTransform, Input.mousePosition, canvasRef.worldCamera, out pos);
        if (aimInputMouse != Vector2.zero)
        {
            crosshair.anchoredPosition = pos;
        }
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRef.transform as RectTransform, crosshair.position, canvasRef.worldCamera, out worldPos);

        if (aimInputController != Vector2.zero)
        {
            crosshair.anchoredPosition += aimInputController * gamePadSens * Time.fixedDeltaTime;
        }

        //gunObject.transform.rotation.x = aimAngle;
        print(gunObject.transform.rotation);

        if (transform.position.x < crosshair.transform.position.x)
            isAimingRight = true;
        else
            isAimingRight = false;
        if (fireInput > 0)
        {
            if (Time.time > timeToFire)
            //if (Time.time > timeToFire && !overheat.IsOverheated())
            {
                timeToFire = Time.time + 1 / player.GetAttackSpeed().GetValue();
                gun.Shoot();
                animator.SetTrigger("gunFired");
                sound.GunShot();
                StartCoroutine(ResetTrigger("gunFired", triggerResetTime));
            }
        }
    }

    /* This function checks if the model is blocked
     * in a certain direction(argument == Vector3.up/Vector3.down.....)
     */
    private bool IsBlocked(Vector3 dir)
    {
        bool isBlock = false;
        RaycastHit hit;
        
        float radius = 0.5f * characterWidth - vertCastPadding;
        float maxDist = 0.5f * characterHeight - radius + vertCastPadding;
        Vector3 halfY = new Vector3 (0, characterHeight * 0.5f, 0);
        Vector3 boxCenter = isRolling ? transform.position - halfY : transform.position;
        float boxHalf = isRolling ? 0.25f * characterHeight - horCastPadding : 0.5f * characterHeight - horCastPadding;
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
            isBlock = Physics.BoxCast(boxCenter, halfExtends, Vector3.right, out hit, Quaternion.identity, 0.5f * characterWidth + vertCastPadding, immoveables, QueryTriggerInteraction.UseGlobal);
            if (isBlock && hit.collider.isTrigger || hit.normal != Vector3.left)
                isBlock = false;
        }
        else if (dir == Vector3.left)
        {
            isBlock = Physics.BoxCast(boxCenter, halfExtends, Vector3.left, out hit, Quaternion.identity, 0.5f * characterWidth + vertCastPadding, immoveables, QueryTriggerInteraction.UseGlobal);
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
        var xSpeed = GetCurrentSpeed();
        var ySpeed = vertVel;
        animator.SetFloat("xSpeed", xSpeed);
        animator.SetFloat("ySpeed", ySpeed);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRolling", isRolling);
        animator.SetBool("isLookingRight", isLookingRight);
        animator.SetBool("isAimingRight", isAimingRight);
        // if speed 
        if (xSpeed == 0 && walkSoundPlaying){
            sound.StopWalk();
        }
    }
    public float GetCurrentSpeed()
    {
        var xSpeed = currentSpeed != 0 ? currentSpeed / maxSpeed : 0;
        return xSpeed;
    }
    
    private void DrawDebugLines()
    {
        if (!debug) return;
        Debug.DrawLine(transform.position, transform.position + moveVec * 2, Color.red);
    }

    private IEnumerator ResetTrigger(string trigger, float time)
    {
        yield return new WaitForSeconds(time);
        animator.ResetTrigger(trigger);
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
