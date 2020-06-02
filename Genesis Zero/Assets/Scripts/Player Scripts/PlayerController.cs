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
public class PlayerController : MonoBehaviour
{
    [Header("CharacterDims")]
    public float characterHeight = 1.8f;
    public float characterWidth = 0.3f;

    [Header("Movement")]
    public float acceleration = 35f;
    public float jumpStrength = 12f;
    public float doubleJumpStrength = 10f;
    public float horCastLength = 0.45f;
    public float verCastLength = 1.05f;
    public float groundCheckPadding = 0.15f;
    public float jumpingHitboxHeight = 1f;
    [Header("Phase Dash")]
    public Color phaseColor = Color.black;
    public float rollDuration = 3f;
    public float rollSpeedMult = 1.5f;
    public float rollCooldownDuration = 3.0f;
    public float jumpBufferTime = 0.2f;
    public LayerMask immoveables; //LayerMask for bound checks
    public LayerMask rollingLayerMask; //layermask used while rolling
    public bool debug;

    [Header("Physics")]
    public float gravity = 18f;
    private float resetGravity;
    public float terminalVel = 32f;

    public float fallSpeedMult = 1.45f;
    public float airControlMult = 0.5f;
    public float airSpeedMult = 0.85f;
    public float fallSpeedWhileRolling = 1.05f;
    public float fallSpeedWhileDashing = .9f;
    private float resetTerminalVel = 24f;
    private float resetfallSpeed = 1.45f;
    [Header("Gun")]
    public GameObject gunObject;
    [Tooltip("GameObject Crosshair (invisible)")]
    public GameObject worldXhair;
    [Tooltip("Canvas Crosshair (visible)")]
    public RectTransform screenXhair;
    public float deadZoneRadius = 1.5f;
    private float timeToFire = 0;

    [Header("Animations")]
    public float triggerResetTime = 0.25f;

    //Component Parts
    private Player player;
    private OverHeat overheat;

    //Input variables
    GameInputActions inputActions;
	[HideInInspector]
    public Vector2 movementInput;
    private Vector2 aimInputMouse;
    private Vector2 aimInputController;
    private float fireInput;
    private float interactInput;
    public float rotationspeed;

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
    private float lastPressed = 0;
    private float dJumpDelay = 0.15f;
    private int rollDirection;
    private Gun gun;
    private float currentHitboxHeight;
    private Vector3 currentHitBoxCenter;
    private CapsuleCollider hurtBoxCol;
    private bool isFallingFast = false;
    //Animation/States Variables
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private bool isDoubleJumping;
    private bool isRolling;
    private bool isDashing;
    private bool isFacingRight = true;
    private bool isAimingRight;

    //sound variables
    private bool walkSoundPlaying = false;
    private PlayerSounds sound;
    private LayerMask defaultLayerMask;
    private Camera camRef;

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
        sound = AudioManager.instance.GetComponent<PlayerSounds>();
        animator = GetComponent<Animator>();
        gun = GetComponent<Gun>();
        overheat = GetComponent<OverHeat>();
        player = GetComponent<Player>();
        camRef = Camera.main;
        worldXhair.transform.position = transform.position;
        resetGravity = gravity;
        hurtBoxCol = gameObject.GetComponent<Hurtbox>().colliders[0].GetComponent<CapsuleCollider>();
        currentHitboxHeight = hurtBoxCol.height;
        currentHitBoxCenter = hurtBoxCol.center;
    }

    private void Update()
    {
        AnimStateUpdate();
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();
        DrawDebugLines();
        ApplyGravity();
        Aim();
        Move();
        UpdateJump();
        UpdateDodgeRoll();
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
        float startVel = currentSpeed;
        maxSpeed = GetComponent<Player>().GetSpeed().GetValue();
        if (isRolling || isDashing) return;
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
                if (walkSoundPlaying == true)
                    sound.StopWalk();
                walkSoundPlaying = false;
            }
        }
        else
        {
            //Stop running sound if player's in the air
            if (walkSoundPlaying == true)
                sound.StopWalk();
            walkSoundPlaying = false;
        }
        
        if (movementInput.x != 0)
        {
            CalculateForwardDirection();
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
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            else
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed * airSpeedMult);
            transform.position += movementInput.x * moveVec * (startVel * Time.fixedDeltaTime + (0.5f * acceleration * Mathf.Pow(Time.fixedDeltaTime, 2)));
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
        
        if (FacingRight == 1)
        {
            float y = Mathf.Lerp(transform.rotation.eulerAngles.y, 90, rotationspeed);
            transform.rotation = Quaternion.Euler(new Vector3(0, y, 0));

        }
        if (FacingRight == 2)
        {
            float y = Mathf.Lerp(transform.rotation.eulerAngles.y, 270, rotationspeed);
            transform.rotation = Quaternion.Euler(new Vector3(0, y, 0));
            //rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, 270, 0)), rotationspeed);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void DrawDebugLines()
    {
        if (!debug)
            return;
        Vector3 castOrigin = transform.position + new Vector3(0, 0.5f * characterHeight, 0);
        Debug.DrawLine(transform.position, transform.position + moveVec * 2f, Color.red);
        Debug.DrawLine(castOrigin, castOrigin + Vector3.down * verCastLength, Color.red);
    }

    private void OnDrawGizmos() 
    {
        Vector3 point0 = transform.position + Vector3.up * 0.5f * characterHeight;
        Vector3 point1 = transform.position;
        Gizmos.DrawSphere(point0, 0.05f);
        Gizmos.DrawSphere(point1, 0.05f);
    }

    /* This function is called in Move()
     * to calculate the move vector in order to deal with ramps and such
     */
    private void CalculateForwardDirection()
    {
        if (!isGrounded)
        {
            moveVec = Vector3.right;
            return;
        }
        Vector3 castOrigin = transform.position + new Vector3(0, 0.5f * characterHeight + 0.5f * characterWidth, 0);
        Physics.SphereCast(castOrigin, 0.5f * characterWidth, Vector3.down, out groundHitInfo, verCastLength + groundCheckPadding, immoveables, QueryTriggerInteraction.UseGlobal);
        moveVec = Vector3.Cross(groundHitInfo.normal, Vector3.forward);
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
            if (!isJumping && jumpCount > 1)
            {
                lastPressed = Time.time;
                jumpCount--;
                sound.Jump();
                isJumping = true;
                vertVel = jumpStrength;
            }
            else if (!isGrounded && jumpCount > 0)
            {
                if (vertVel <= 0.5f) lastPressed = dJumpDelay;
                if (Time.time - lastPressed < dJumpDelay) return;
                jumpCount--;
                isJumping = true;
                vertVel = doubleJumpStrength;
				VFXManager.instance.PlayEffect("VFX_DoubleJump", transform.position + new Vector3(0, .5f, 0));
                if (isFacingRight && movementInput.x <= 0)
                    currentSpeed = 0;
                if (!isFacingRight && movementInput.x >= 0)
                    currentSpeed = 0;
            }
        }
    }
    public void Down(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
            return;
        FallFaster(-45);
        isFallingFast = true;
        Hitbox hit = this.transform.Find("Center").Find("Down").GetComponent<Hitbox>();
        hit.InitializeHitbox(Player.instance.GetAbilityPower().GetValue(), Player.instance, false);
        int stacks = Player.instance.GetSkillStack("Anti-Gravity Boots");
        if (stacks > 0)
        {
            hit.Damage = (stacks * Player.instance.GetAbilityPower().GetValue()) + Player.instance.GetAbilityPower().GetValue();
        }

    }
    public void FallFaster(float terminal)
    {
        if (vertVel > terminal * .35f)
            vertVel = terminal * .35f;
        terminalVel = terminal * -1;
        transform.Find("Center").Find("Down").gameObject.SetActive(true);
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
            vertVel = 0;

        if (vertVel > 0)
        {
            vertVel -= gravity * (Player.instance.GetWeight().GetValue()/10) *  Time.fixedDeltaTime;
            transform.position += Vector3.up * (startVel * Time.fixedDeltaTime + (0.5f * -gravity* (Player.instance.GetWeight().GetValue() / 10)) * Mathf.Pow(Time.fixedDeltaTime, 2));
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
        Collider[] cols;
        Vector3 point0 = transform.position + Vector3.up * 0.5f * characterHeight;
        Vector3 point1 = transform.position;
        //cols = Physics.OverlapSphere(origin, 0.075f, immoveables, QueryTriggerInteraction.UseGlobal);
        cols = Physics.OverlapCapsule(point0, point1, 0.05f, immoveables, QueryTriggerInteraction.UseGlobal);
        if (IsBlocked(Vector3.down))
        {
            isGrounded = true;
            if (isFallingFast)
            {
                VFXManager.instance.PlayEffect("VFX_Lightning", this.transform.position);
                isFallingFast = false;
                Invoke("DisableDown", .75f);
            }
            hurtBoxCol.center = currentHitBoxCenter;
            hurtBoxCol.height = currentHitboxHeight;
            if (cols.Length != 0)
            {
                foreach (var col in cols)
                {
                    //If there's at least one non-trigger collider under -> grounded
                    if (!col.isTrigger)
                    {
                        transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * 0.1f, 25 * Time.fixedDeltaTime);
                        break;
                    }
                }
            }
            //Resets jump counts, play landing sound
            terminalVel = resetTerminalVel;
            if (vertVel < 0)
            {
                vertVel = 0;
                if  (Time.realtimeSinceStartup > 12f)
                    sound.Land();
            }
               
            if (!isJumping && !isDashing && !isRolling)
                jumpCount = 2;
        }
        else
        {
            hurtBoxCol.center = Vector3.zero;
            hurtBoxCol.height = jumpingHitboxHeight;
            isGrounded = false;
        }
    }

    private void CheckWall()
    {
        Collider[] cols;
        Vector3 halfExtends = new Vector3(0.1f, 0.25f * characterHeight, 0);
        Vector3 rightOrigin = transform.position + new Vector3(0.5f * characterWidth, 0.5f * characterHeight, 0);
        Vector3 leftOrigin = transform.position + new Vector3(-0.5f * characterWidth, 0.5f * characterHeight, 0);
        if (IsBlocked(Vector3.right))
        {
            cols = Physics.OverlapBox(rightOrigin, halfExtends, Quaternion.identity, immoveables, QueryTriggerInteraction.UseGlobal);
            if (cols.Length != 0)
            {
                foreach (var col in cols)
                {
                    if (!col.isTrigger)
                    {
                        transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left * 0.1f, 25 * Time.fixedDeltaTime);
                    }
                }
            }
        }
        if (IsBlocked(Vector3.left))
        {
            cols = Physics.OverlapBox(leftOrigin, halfExtends, Quaternion.identity, immoveables, QueryTriggerInteraction.UseGlobal);
            if (cols.Length != 0)
            {
                foreach (var col in cols)
                {
                    if (!col.isTrigger)
                    {
                        transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * 0.1f, 25 * Time.fixedDeltaTime);
                    }
                }
                
            }
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
        {
            fallSpeedMult = 0;
            vertVel = 0;
        }
        if (isDashing)
        {
            fallSpeedMult = 0;
            vertVel = 0;
        }
        if (!isDashing && !isRolling)
        {
            fallSpeedMult = resetfallSpeed;
        }
        if (!isJumping)
        {
            vertVel -= gravity* (Player.instance.GetWeight().GetValue() / 10) * fallSpeedMult * Time.fixedDeltaTime;
            vertVel = Mathf.Clamp(vertVel, -terminalVel, 0);
            transform.position += Vector3.up * (startVel * Time.fixedDeltaTime + (0.5f * fallSpeedMult * -gravity* (Player.instance.GetWeight().GetValue() / 10)) * Mathf.Pow(Time.fixedDeltaTime, 2));
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
                //animator.SetTrigger("startRoll");
                //StartCoroutine(ResetTrigger("startRoll", triggerResetTime));
                gun.PhaseTrigger = true;
                GetComponent<UniqueEffects>().PhaseTrigger = true;
                sound.Roll();
                //VFXManager.instance.PlayEffect("VFX_PlayerDashStart", transform.position);

                GameObject dashstart = VFXManager.instance.PlayEffectOnObject("VFX_PlayerDashStart", this.gameObject, new Vector2(0,1));
                if (IsFacingRight() == false)
                    dashstart.transform.Rotate(new Vector3(0, 180, 0));
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false; //TEMPORARY CHANGE THIS
                GameObject dash = VFXManager.instance.PlayEffectForDuration("VFX_PlayerDashEffect", transform.position, rollDuration);
                dash.transform.SetParent(transform);
                if (phaseColor != Color.black)
                {
                    VFXManager.instance.ChangeColor(dashstart, phaseColor);
                    VFXManager.instance.ChangeColor(dash, phaseColor);
                }
                //Select roll direction based on crosshair position and input
                if (movementInput.x != 0)
                    rollDirection = movementInput.x > 0 ? 1 : -1;
                else
                    rollDirection =  isAimingRight ? 1 : -1;
                isRolling = true;
                GetComponent<Player>().SetInvunerable(rollDuration+.1f);
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
    private void EndRoll()
    {
        isRolling = false;
        //animator.SetTrigger("endRoll");
        //StartCoroutine(ResetTrigger("endRoll", triggerResetTime));
        rollCooldown = rollCooldownDuration;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        StartCoroutine(ResetCooldown(rollCooldownDuration));
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
                EndRoll();
                return;
            }
            //continue rolling if rollDuration is not reached
            if (timeRolled < rollDuration)
            {
                transform.position += rollDirection * moveVec * rollSpeed * Time.fixedDeltaTime;
                timeRolled += Time.fixedDeltaTime;
            }
            else
            {
                EndRoll();
            }

        }
    }

    /* This function control character aiming
     * Crosshair is moved using mouse/rightStick
     * Gun rotates to point at crosshair
     */
    private void Aim()
    {
        float camZ = Mathf.Abs(camRef.transform.position.z);
        Vector3 worldXhairPos;
        Vector2 screenXhairPos;

        //Calculating to stop crosshair from going off screen;
        Vector3 maxBounds = camRef.ViewportToScreenPoint(new Vector3(1, 1, 0));
        Vector3 minBounds = camRef.ViewportToScreenPoint(new Vector3(0, 0, 0));
        //Shifting origin of deadzone to middle of character
        Vector3 offsetOrigin = transform.position + Vector3.up * 0.5f * characterHeight;
        Vector3 worldMousePos = camRef.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, camZ + 0.5f));
        worldMousePos.z = 0;
        //Clamping crosshair to a circle around player
        // if mouse is too close to player
        if (Vector3.Distance(worldMousePos, offsetOrigin) < deadZoneRadius)
        {
            Vector3 direction = (worldMousePos - offsetOrigin).normalized;
            Vector3 circlePoint = offsetOrigin + (direction * deadZoneRadius);
            screenXhairPos = camRef.WorldToScreenPoint(circlePoint);
        }
        else
        {
            screenXhairPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        //Clamping position of crosshair inside the screen
        screenXhairPos.x = Mathf.Clamp(screenXhairPos.x, minBounds.x, maxBounds.x);
        screenXhairPos.y = Mathf.Clamp(screenXhairPos.y, minBounds.y, maxBounds.y);
        //Updating position of screenXhair
        screenXhair.position = screenXhairPos;

        //Updating position of worldXhair to match screenXhair
        worldXhairPos = camRef.ScreenToWorldPoint(new Vector3(screenXhairPos.x, screenXhairPos.y, camZ + 0.5f));
        worldXhairPos.z = 0;
        worldXhair.transform.position = worldXhairPos;

        // checking where the player's aiming
        if (transform.position.x < worldXhair.transform.position.x)
            isAimingRight = true;
        else
            isAimingRight = false;
        if (isRolling) return;
        //rotate the gun
        //gunObject.transform.LookAt(worldXhair.transform);

        // Shoot()
        if (fireInput > 0)
        {
            if (Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / player.GetAttackSpeed().GetValue();
                gun.Shoot();
                //animator.SetTrigger("gunFired");
                sound.GunShot();
                //StartCoroutine(ResetTrigger("gunFired", triggerResetTime));
            }
        }
    }

    /* This function checks if the model is blocked
     * in a certain direction(argument == Vector3.up/Vector3.down.....)
     */
    private bool IsBlocked(Vector3 dir)
    {
        bool isBlock = false;
        float halfWidth = 0.5f * characterWidth;
        float halfHeight = 0.5f * characterHeight;
        Vector3 halfExtends = new Vector3(0.1f, 0.5f * halfHeight, 0);
        RaycastHit[] array = new RaycastHit[0];

        if (dir == Vector3.up)
        {
            Vector3 castOrigin = transform.position + new Vector3(0, halfHeight - halfWidth, 0);
            array = Physics.SphereCastAll(castOrigin, halfWidth, Vector3.up, verCastLength, immoveables, QueryTriggerInteraction.UseGlobal);
        }
        else if (dir == Vector3.down)
        {
            Vector3 castOrigin = transform.position + new Vector3(0, halfHeight + halfWidth, 0);
            array = Physics.SphereCastAll(castOrigin, halfWidth, Vector3.down, verCastLength, immoveables, QueryTriggerInteraction.UseGlobal);
        }
        else if (dir == Vector3.left)
        {
            //Casting side casts from the center of the character
            Vector3 castOrigin = transform.position + new Vector3(0, halfHeight, 0);
            array = Physics.BoxCastAll(castOrigin, halfExtends, Vector3.left, Quaternion.identity, horCastLength, immoveables, QueryTriggerInteraction.UseGlobal);
        }
        else if (dir == Vector3.right)
        {
            //Casting side casts from the center of the character
            Vector3 castOrigin = transform.position + new Vector3(0, halfHeight, 0);
            array = Physics.BoxCastAll(castOrigin, halfExtends, Vector3.right, Quaternion.identity, horCastLength, immoveables, QueryTriggerInteraction.UseGlobal);
        }
        else
        {
            Debug.LogError("Invalid Vector used in IsBlocked");
        }

        if (array.Length == 0)
        {
            isBlock = false;
        }
        else
        {
            foreach (var item in array)
            {
                if (!item.collider.isTrigger)
                {
                    isBlock = true;
                    break;
                }
            }
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
    public void DisableDown()
    {
        transform.Find("Center").Find("Down").gameObject.SetActive(false);
    }
    /* This function updates information
     * about the player's state for animations
     */
    private void AnimStateUpdate()
    {
        float xSpeed = GetCurrentSpeed();
        float ySpeed = vertVel;
        // if (FacingRight==1)
        // xSpeed *= -1;
        float animspeed = movementInput.x * FacingSign;
        GameObject indic = transform.Find("Center").Find("PhaseIndicator").gameObject;
        if (rollCooldown == 0)
            indic.SetActive(true);
        else
            indic.SetActive(false);
        animator.SetFloat("xSpeed",animspeed);
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

    private int FacingRight
    {
        get
        {
            //Debug.Log("aimvector-transform.position.x " + (worldXhair.transform.position.x - transform.position.x));
            // Debug.Log("Sign aimvector-transform= "+Mathf.Sign(aimvector.x - transform.position.x));
            float facing = (worldXhair.transform.position.x - transform.position.x);
            return facing >= .3f ? 1 : facing <= -.3f ? 2 : 3;
        }
    }//returns a value of 1-3 based on 

    private int FacingSign
    {
        get
        {
            Vector3 perp = Vector3.Cross(transform.forward, Vector3.forward);
            float dir = Vector3.Dot(perp, transform.up);
            return dir > 0f ? -1 : dir < 0f ? 1 : 0;
        }
    }

    public bool IsFacingRight(){
        return isFacingRight;
    }
    public bool IsAimingRight()
    {
        return isAimingRight;
    }
	public bool IsRolling()
	{
		return isRolling;
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
    public Vector2 CenterPoint()
    {
        return (this.transform.position + new Vector3(0, characterHeight / 2, 0));
    }
    public void SetVertVel(float vel)
    {
        vertVel = vel;
    }
    public float GetNormalGravity()
    {
        return resetGravity;
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
