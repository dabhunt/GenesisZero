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
    public float horCastPadding = 0.2f; // offset used for the sides casts
    public float vertCastPadding = 0.1f; // offset used for the feet/head casts
    public float rollDuration = 3f;
    public float rollSpeedMult = 1.5f;
    public float rollCooldownDuration = 3.0f;
    public float jumpBufferTime = 0.2f;
    public LayerMask immoveables; //LayerMask for bound checks
    public LayerMask rollingLayerMask; //layermask used while rolling
    public bool debug;

    [Header("Physics")]
    public float gravity = 18f;
    public float terminalVel = 24f;
    public float fallSpeedMult = 1.45f;
    public float airControlMult = 0.5f;
    public float airSpeedMult = 0.85f;
    public float slopeRayDistMult = 1.25f;
    public float fallSpeedWhileRolling = 1.05f;
    public float fallSpeedWhileDashing = .9f;
    private float resetfallSpeed = 1.45f;
    [Header("Canvas")]
    public Canvas canvasRef;

    [Header("Gun")]
    public GameObject gunObject;
    [Tooltip("GameObject Crosshair (invisible)")]
    public GameObject worldXhair;
    [Tooltip("Canvas Crosshair (visible)")]
    public RectTransform screenXhair;
    public float gamePadSens = 15f;
    private float timeToFire = 0;

    [Header("Animations")]
    public float triggerResetTime = 0.25f;

    //Component Parts
    private Player player;
    private OverHeat overheat;
    
    //Input variables
    GameInputActions inputActions;
    private Vector2 movementInput;
    private Vector2 aimInputMouse;
    private Vector2 aimInputController;
    private float fireInput;
    private float interactInput;

    //Movement Variables
    private RaycastHit groundHitInfo;
    [HideInInspector]
    public Vector3 moveVec = Vector3.right;
    private float maxSpeed;
    private float vertVel;
    private float currentSpeed = 0;
    private float jumpCount = 2;
    private float jumpPressedTime;
    private float timeRolled = 0;
    private float rollCooldown = 0;
    private int rollDirection;
    private Vector3 lastRollingPosition;

    //Aim Variables
    private float gamepadAimTime;
    private Gun gun;

    //Animation/States Variables
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private bool isRolling;
    private bool isDashing;
    private bool isFacingRight = true;
    private bool isAimingRight;

    //sound variables
    private bool walkSoundPlaying = false;
    private PlayerSounds sound;
    private LayerMask defaultLayerMask;
    

    private void Start()
    {
        inputActions = GameInputManager.instance.GetInputActions();
        inputActions.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.AimMouse.performed += ctx => aimInputMouse = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.AimController.performed += ctx => aimInputController = ctx.ReadValue<Vector2>();
        inputActions.PlayerControls.Fire.performed += ctx => fireInput = ctx.ReadValue<float>();
        inputActions.PlayerControls.Interact.performed += ctx => interactInput = ctx.ReadValue<float>();
        defaultLayerMask = immoveables;
        resetfallSpeed = fallSpeedMult;
        sound = FindObjectOfType<AudioManager>().GetComponent<PlayerSounds>();
        animator = GetComponent<Animator>();
        gun = GetComponent<Gun>();
        overheat = GetComponent<OverHeat>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        AnimStateUpdate();
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();
        ApplyGravity();
        Aim();
        Move();
        UpdateJump();
        UpdateDodgeRoll();
        DrawDebugLines();
    }
    //this swaps out the layermask while abilities are active / player is invulnerable so they can pass through enemies and bullets
    public void NewLayerMask(LayerMask newMask, float duration)
    {
        immoveables = newMask;
        Invoke("ResetLayerMask", duration);
    }
    public void ResetLayerMask()
    {
        immoveables = defaultLayerMask;
    }

    /* This controls the character general movements
     * It updates the movement vector every frame then apply
     * it based on the input
     */
    private void Move()
    {
        if (StateManager.instance.IsPaused())
            return;
        float multiplier = isGrounded ? 1 : airControlMult;
        float startVel = Mathf.Abs(currentSpeed);
        Vector3 distXhair = worldXhair.transform.position - transform.position;

        maxSpeed = GetComponent<Player>().GetSpeed().GetValue();
        
        CalculateForwardDirection();
        if (isRolling) return;

        if (isGrounded)
        {
            //Play running sound if player's moving on the ground
            if(currentSpeed > 0 && !walkSoundPlaying)
            {
                walkSoundPlaying = true;
                sound.Walk();
            }
            //Stop running sound if player not moving
            else if (currentSpeed <= 0)
            {
                walkSoundPlaying = false;
                sound.StopWalk();
            }
        }
        else
        {
            //Stop running sound if player's in the air
            walkSoundPlaying = false;
            sound.StopWalk();
        }

        if (movementInput.x != 0)
        {
            if (movementInput.x > 0 && IsBlocked(Vector3.right))
            {
                currentSpeed = 0;
                return;
            }
            if (movementInput.x < 0 && IsBlocked(Vector3.left))
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
            if (movementInput.x > 0 && transform.eulerAngles.y != 90)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                isFacingRight = true;
            }
            if (movementInput.x < 0 && transform.eulerAngles.y != -90)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
                isFacingRight = false;
            }
            transform.position += moveVec * (startVel * Time.fixedDeltaTime + (0.5f * acceleration * Mathf.Pow(Time.fixedDeltaTime, 2)));
            if (gamepadAimTime > 0)
                worldXhair.transform.position = transform.position + distXhair;
        }
        else
        {
            //Decelerate
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
            if (!inputActions.PlayerControls.enabled) return;
            if (isRolling) return;
            if (isGrounded && jumpCount < 2) return;
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
            else if (!isGrounded && jumpCount > 0)
            {
                jumpCount--;
                animator.SetTrigger("startDoubleJump");
                StartCoroutine(ResetTrigger("startDoubleJump", triggerResetTime));
                isJumping = true;
                vertVel = doubleJumpStrength;
                if (isFacingRight && movementInput.x <= 0)
                    currentSpeed = 0;
                if (!isFacingRight && movementInput.x >= 0)
                    currentSpeed = 0;
            }
        }
    }

    /* This function is used to update the jump cycle and its behavior
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
                if (hit.distance < 1f * characterHeight + vertCastPadding)
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * 1* characterHeight , 5 * Time.fixedDeltaTime);

            if (vertVel < 0)
                vertVel = 0;
            if (!isJumping)
                jumpCount = 2;
        }
        else
        {
            isGrounded = false;
        }
    }

    /* This function just push the character back out
     * if it was lodged into a wall.
     */
    private void CheckWall()
    {
        RaycastHit hit;

        if (IsBlocked(Vector3.left))
        {
            if (Physics.Raycast(transform.position, Vector3.left, out hit, (characterWidth * 0.5f) + vertCastPadding, immoveables))
                if (hit.distance < .5f * characterWidth + horCastPadding)
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * characterWidth * .5f, 5 * Time.fixedDeltaTime);
        }
        if (IsBlocked(Vector3.right))
        {
            if (Physics.Raycast(transform.position, Vector3.right, out hit, (characterWidth * 0.5f) + vertCastPadding, immoveables))
                if (hit.distance < .5f * characterWidth + horCastPadding)
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left * characterWidth * .5f, 5 * Time.fixedDeltaTime);
        }
        if (IsBlocked(Vector3.up))
        {
            if (Physics.Raycast(transform.position, Vector3.up, out hit, characterHeight * 0.5f, immoveables))
                if (hit.distance < .5f * characterHeight + vertCastPadding)
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down * .5f * characterHeight, 5 * Time.fixedDeltaTime);
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
        if (isRolling)
            fallSpeedMult = fallSpeedWhileRolling;
        if (isDashing)
            fallSpeedMult = fallSpeedWhileDashing;
        if (!isDashing && !isRolling)
        {
            fallSpeedMult = resetfallSpeed;
        }
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
            if (!inputActions.PlayerControls.enabled) return;
            if (!isRolling && rollCooldown == 0)
            {
                animator.SetTrigger("startRoll");
                StartCoroutine(ResetTrigger("startRoll", triggerResetTime));
                //Select roll direction based on crosshair position and input
                if (movementInput.x != 0)
                    rollDirection = movementInput.x > 0 ? 1 : -1;
                else
                    rollDirection =  isAimingRight ? 1 : -1;
                isRolling = true;
                GetComponent<Player>().SetInvunerable(rollDuration);
                NewLayerMask(rollingLayerMask, rollDuration);
                timeRolled = 0;
                //lastRollingPosition = transform.position;

                //Rotate the character depending on roll direction
                if (rollDirection < 0 && isFacingRight)
                {
                    //if the 90 degree values are zero, then the roll works while moving but not when standing still
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    isFacingRight = false;
                }
                if (rollDirection > 0 && !isFacingRight)
                {
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    isFacingRight = true;
                }
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
                rollCooldown = rollCooldownDuration;
                StartCoroutine(ResetCooldown(rollCooldownDuration));
                return;
            }
            //continue rolling if rollDuration is not reached
            if (timeRolled < rollDuration)
            {
                transform.position += moveVec * rollSpeed * Time.fixedDeltaTime;
                timeRolled += Time.fixedDeltaTime;
            }
            else
            {
                isRolling = false;
                animator.SetTrigger("endRoll");
                StartCoroutine(ResetTrigger("endRoll", triggerResetTime));
                rollCooldown = rollCooldownDuration;
                StartCoroutine(ResetCooldown(rollCooldownDuration));
            }
            //lastRollingPosition = transform.position;
        }
    }

    /* This function control character aiming
     * Crosshair is moved using mouse/rightStick
     * Gun rotates to point at crosshair
     */
    private void Aim()
    {   
        float camZ = Mathf.Abs(canvasRef.worldCamera.transform.position.z - transform.position.z);
        Vector3 mouseWorldPos = canvasRef.worldCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camZ));
        Vector3 maxBounds = canvasRef.worldCamera.ViewportToWorldPoint(new Vector3(1, 1, camZ));
        Vector3 minBounds = canvasRef.worldCamera.ViewportToWorldPoint(new Vector3(0, 0, camZ));
        Vector2 screenXhairPos;

        //Clamp the mouse position to bind worldXhair inside screen when using mouse
        mouseWorldPos.x = Mathf.Clamp(mouseWorldPos.x, minBounds.x, maxBounds.x);
        mouseWorldPos.y = Mathf.Clamp(mouseWorldPos.y, minBounds.y, maxBounds.y);

        Vector3 mouseScreenPos = canvasRef.worldCamera.WorldToScreenPoint(mouseWorldPos);
        //Converts MouseScreen position into localpoint in canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRef.transform as RectTransform, mouseScreenPos, canvasRef.worldCamera, out screenXhairPos);
        screenXhair.anchoredPosition = screenXhairPos;

        /*
        gamepadAimTime -= Time.fixedDeltaTime;
        gamepadAimTime = Mathf.Max(gamepadAimTime, 0);
        if (aimInputController != Vector2.zero)
        { 
            //Stops the crosshair from going off screen when using controller
            gamepadAimTime = 30;
            worldXhair.transform.position += mouseWorldPos;
        }
        else
        {
            if (aimInputMouse != Vector2.zero && gamepadAimTime > 0)
                gamepadAimTime = 0;
            if (gamepadAimTime == 0)
                worldXhair.transform.position = mouseWorldPos;
        }
        */
        // checking where the player's aiming
        if (transform.position.x < worldXhair.transform.position.x)
            isAimingRight = true;
        else
            isAimingRight = false;
        if (isRolling) return;
        //rotate the gun
        gunObject.transform.LookAt(worldXhair.transform);

        // Shoot()
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
        
        float radius = (0.5f * characterWidth) - vertCastPadding;
        float maxDist = (0.5f * characterHeight) - radius + vertCastPadding;
        Vector3 halfY = new Vector3 (0, characterHeight * 0.25f, 0);
        Vector3 boxCenter = isRolling ? transform.position - halfY : transform.position;
        float boxHalf = isRolling ? (0.25f * characterHeight) - (0.5f * horCastPadding) : (0.5f * characterHeight) - horCastPadding;
        Vector3 halfExtends = new Vector3(0, boxHalf, 0);
        Vector3 horDirection;

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
            horDirection = isFacingRight ? moveVec : -moveVec;
            isBlock = Physics.BoxCast(boxCenter, halfExtends, horDirection, out hit, Quaternion.identity, 0.5f * characterWidth + vertCastPadding, immoveables, QueryTriggerInteraction.UseGlobal);
            if (isBlock && (hit.collider.isTrigger))
                isBlock = false;
        }
        else if (dir == Vector3.left)
        {
            horDirection = !isFacingRight ? moveVec : -moveVec;
            isBlock = Physics.BoxCast(boxCenter, halfExtends, horDirection, out hit, Quaternion.identity, 0.5f * characterWidth + vertCastPadding, immoveables, QueryTriggerInteraction.UseGlobal);
            if (isBlock && (hit.collider.isTrigger))
                isBlock = false;
        }
        else
        {
            Debug.LogError("Invalid Vector used in IsBlocked");
        }
        return isBlock;
    }

    public bool BlockedAll()
    {
    	if(IsBlocked(Vector3.left)) return true;
    	if(IsBlocked(Vector3.right)) return true;
    	if(IsBlocked(Vector3.up)) return true;
    	return false;
    }

    /* This function updates information
     * about the player's state for animations
     */
    private void AnimStateUpdate()
    {
        float xSpeed = GetCurrentSpeed();
        float ySpeed = vertVel;
        if (isFacingRight != isAimingRight)
            xSpeed *= -1;
        animator.SetFloat("xSpeed", xSpeed);
        animator.SetFloat("ySpeed", ySpeed);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRolling", isRolling);
        animator.SetBool("isFacingRight", isFacingRight);
        animator.SetBool("isAimingRight", isAimingRight);
    }
    public float GetCurrentSpeed()
    {
        var xSpeed = currentSpeed != 0 ? currentSpeed / maxSpeed : 0;
        return xSpeed;
    }
    public bool IsFacingRight(){
        return isFacingRight;
    }
    public bool IsAimingRight()
    {
        return isAimingRight;
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

    private IEnumerator ResetCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        rollCooldown = 0;
    }

    public void SetVertVel(float vel)
    {
        vertVel = vel;
    }
    public void Dash(float duration)
    {
        isDashing = true;
        //if fall speed unspecified, default to the roll fall speed
        fallSpeedWhileDashing = fallSpeedWhileRolling;
        Invoke("StopDash", duration);
    }
    public void Dash(float duration, float fallSpeed)
    {
        isDashing = true;
        fallSpeedWhileDashing = fallSpeed;
        Invoke("StopDash", duration);
    }
    public void StopDash()
    {
        isDashing = false;
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
    public float GetFireInput()
    {
        return fireInput;
    }
}
