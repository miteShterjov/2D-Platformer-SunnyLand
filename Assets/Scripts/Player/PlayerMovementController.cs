using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovementController : MonoBehaviour
{
    #region Class Params
    public enum PlayerState { Idle, Running, Sprinting, Jumping, Falling, WallSlide, WallSlideJump }

    public static readonly int anim_param_move = Animator.StringToHash("xVelocity");
    public static readonly int anim_param_jump = Animator.StringToHash("yVelocity");
    public static readonly int anim_param_grounded = Animator.StringToHash("isGrounded");
    public static readonly int anim_param_wallSliding = Animator.StringToHash("isWallSliding");


    [Header("Movement")]
    [SerializeField, Tooltip("Movement speed of the player")] private float moveSpeed = 5f;
    [SerializeField, Tooltip("Sprint speed multiplier")] private float sprintMultiplier = 1.5f;
    [Space]
    [Header("Jumping")]
    [SerializeField, Tooltip("Force applied when jumping")] private float jumpForce = 7f;
    [SerializeField, Tooltip("Maximum number of jumps available")] private int maxJumps = 2;
    [SerializeField, Tooltip("Post-leave ground window")] private float coyoteTime = 0.12f;
    [SerializeField, Tooltip("Pre-ground press window")] private float jumpBuffer = 0.12f;
    [SerializeField, Tooltip("Multiplier for falling speed")] private float fallMultiplier = 2.2f;
    [SerializeField, Tooltip("Multiplier for low jumps")] private float lowJumpMultiplier = 2.0f;
    [SerializeField, Tooltip("Stamina cost for jumping")] private float staminaJumpCost = 4.0f;
    [SerializeField, Tooltip("Speed at which the player falls while wall sliding")] private float wallFallSpeed = 0.5f;

    [Space]
    [Header("Collision Detection")]
    [SerializeField, Tooltip("Layer mask for ground detection")] private LayerMask groundLayer;
    [SerializeField, Tooltip("Distance to check for ground")] private float groundCheckDistance = 0.1f;
    [SerializeField, Tooltip("Point to check for ground")] private Transform groundCheckPoint;
    [SerializeField, Tooltip("Point to check for wall")] private Transform wallPrimaryCheck;
    [SerializeField, Tooltip("Point to check for wall")] private Transform wallSecondaryCheck;
    [SerializeField, Tooltip("Distance to check for wall")] private float wallCheckDistance = 0.5f;
    [SerializeField, Tooltip("Lockout time after a wall jump where wall detection won't re-enter WallSlide")] private float wallJumpLockDuration = 0.15f;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerState currentState { get; set; }
    private InputSystem_Actions inputActions;
    private Vector2 movementInput;
    private Character_AVFXs vfx;
    private int remainingJumps;
    private bool isGrounded;
    private float coyoteTimer;
    private float bufferTimer;
    private bool jumpHeld;
    
    private float wallJumpLockTimer;
    private float baseGravityScale;
    private float lastWallNormalX; // from raycast
    #endregion

    #region Unity Methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        inputActions = new InputSystem_Actions();
        vfx = GetComponentInChildren<Character_AVFXs>();
        baseGravityScale = rb.gravityScale;
    }

    void Start()
    {
        inputActions.Player.Move.performed += ctx => currentState = PlayerState.Running;
        inputActions.Player.Move.canceled += ctx => currentState = PlayerState.Idle;
        inputActions.Player.Sprint.performed += ctx => currentState = PlayerState.Sprinting;
        inputActions.Player.Sprint.canceled += ctx => currentState = PlayerState.Running;
        inputActions.Player.Jump.performed += ctx => PlayerJumpStarted(ctx);
        inputActions.Player.Jump.canceled += ctx => PlayerJumpCanceled(ctx);
        
        inputActions.Player.Testing.performed += ctx => GetComponent<PlayerHealthController>().PlayerDies();


    }

    void Update()
    {
        // Environment checks first
        bool wasGrounded = isGrounded;
        isGrounded = IsGrounded();

        // Reset jumps and resolve landing state
        if (isGrounded && !wasGrounded)
        {
            remainingJumps = maxJumps;
            currentState = inputActions.Player.Move.IsPressed() ? PlayerState.Running : PlayerState.Idle;
        }

        // Wall interactions (ignore while wall jump lock is active)
        wallJumpLockTimer = Mathf.Max(0f, wallJumpLockTimer - Time.deltaTime);
        bool touchingWall = IsTouchingWall();

        if (touchingWall && !isGrounded && rb.linearVelocity.y <= 0.01f && wallJumpLockTimer <= 0f)
        {
            currentState = PlayerState.WallSlide;
        }

        if (touchingWall && !isGrounded && inputActions.Player.Jump.WasPressedThisFrame() && wallJumpLockTimer <= 0f)
        {
            currentState = PlayerState.WallSlideJump;
        }

        // Timers
        coyoteTimer = isGrounded ? coyoteTime : Mathf.Max(0f, coyoteTimer - Time.deltaTime);
        bufferTimer = Mathf.Max(0f, bufferTimer - Time.deltaTime);

        // Now run state behavior and animations
        HandlePlayerState();
        HandleAnimEvents();
    }

    private void FixedUpdate()
    {
        // Consume jump if buffered and allowed
        if (bufferTimer > 0f && CanJumpNow())
        {
            DoJump();
            bufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // Better fall feel (skip while wall sliding; that state manages fall)
        Vector2 tempVector = rb.linearVelocity;
        if (currentState != PlayerState.WallSlide)
        {
            if (tempVector.y < -0.01f) tempVector += Vector2.up * Physics2D.gravity.y * rb.gravityScale * (fallMultiplier - 1f) * Time.fixedDeltaTime;
            else if (tempVector.y > 0.01f && !jumpHeld) tempVector += Vector2.up * Physics2D.gravity.y * rb.gravityScale * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = tempVector;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
    #endregion

    #region Player State Handlers
    private void HandlePlayerState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                PlayerIdle();
                break;
            case PlayerState.Running:
                PlayerMove();
                break;
            case PlayerState.Sprinting:
                PlayerSprint();
                break;
            case PlayerState.Jumping:
                // Jump logic is handled in PlayerJumpStarted callback and FixedUpdate
                break;
            case PlayerState.Falling:
                // Handle falling state logic
                break;
            case PlayerState.WallSlide:
                PlayerWallSlide();
                break;
            case PlayerState.WallSlideJump:
                PlayerWallSlideJump();
                break;
        }
    }

    private void PlayerIdle() => SetVelocity(new Vector2(0f, rb.linearVelocity.y));

    private void PlayerMove()
    {
        movementInput = GetVelocity();
        Vector2 velocity = new Vector2(movementInput.x * moveSpeed, rb.linearVelocity.y);
        SetVelocity(velocity);
    }

    private void PlayerSprint()
    {
        if (PlayerStats.instance.CurrentStamina <= 0) return;

        if (inputActions.Player.Sprint.IsPressed())
        {
            PlayerStats.instance.SpendStamina();
            movementInput = GetVelocity();
            Vector2 velocity = new Vector2(movementInput.x * moveSpeed * sprintMultiplier, rb.linearVelocity.y);
            SetVelocity(velocity);
        }
    }

    private void PlayerJumpStarted(InputAction.CallbackContext _)
    {
        jumpHeld = true;
        bufferTimer = jumpBuffer;
        currentState = PlayerState.Jumping;
    }
    private void PlayerJumpCanceled(InputAction.CallbackContext _)
    {
        jumpHeld = false;
        if (IsGrounded() && rb.linearVelocity.x > 0.1f) currentState = PlayerState.Running;
        if (rb.linearVelocity.y < 0) currentState = PlayerState.Falling;
    }

    // Helpers
    // Allow if grounded (via coyote) or have air jumps left
    private bool CanJumpNow() => coyoteTimer > 0f || remainingJumps > 0;

    private void DoJump()
    {
        if (PlayerStats.instance.CurrentStamina <= 0) return;

        PlayerStats.instance.SpendStamina(staminaJumpCost);

        // Set clean upward takeoff
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // If grounded via coyote, we’re spending the first jump now.
        // With remainingJumps model, just decrement if not grounded.
        if (!isGrounded) remainingJumps = Mathf.Max(remainingJumps - 1, 0);
        else remainingJumps = Mathf.Max(maxJumps - 1, 0);
    }

    private void PlayerWallSlide()
    {
        // Keep normal gravity and cap downward speed to -wallFallSpeed
        rb.gravityScale = baseGravityScale;
        float cappedY = Mathf.Max(rb.linearVelocity.y, -Mathf.Abs(wallFallSpeed));
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, cappedY);
    }

    private void PlayerWallSlideJump()
    {
        jumpHeld = true;

        // Prefer wall normal when available; fallback to facing
        float awayX = (Mathf.Abs(lastWallNormalX) > 0.001f) ? lastWallNormalX : -Mathf.Sign(transform.localScale.x);
        Vector2 wallJumpDirection = new Vector2(awayX, 1f).normalized;

        // Apply jump impulse
        rb.gravityScale = baseGravityScale;
        rb.linearVelocity = wallJumpDirection * jumpForce;

        // Prevent immediate re-entering wall slide
        wallJumpLockTimer = wallJumpLockDuration;
        currentState = PlayerState.Jumping;
    }

    #endregion

    #region Utility Methods
    
    public void Push(Vector2 direction, float duration = 0)
    {
        StartCoroutine(PushCoroutine(direction, duration));
    } 

    private IEnumerator PushCoroutine(Vector2 direction, float duration)
    {
        inputActions.Disable();

        // rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        inputActions.Enable();
    }

    private Vector2 GetVelocity() => inputActions.Player.Move.ReadValue<Vector2>();

    private void SetVelocity(Vector2 velocity)
    {
        // If knockback is active, don't override X velocity; let physics carry the impulse
        if (vfx != null && vfx.IsKnocked)
        {
            // Optionally still allow Y changes (e.g., gravity adjustments)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocity.y);
            return;
        }
        rb.linearVelocity = velocity;
        FlipPlayerSprite();
    }

    private bool IsGrounded() => Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckDistance, groundLayer);

    private bool IsTouchingWall()
    {
        Vector2 dir = Vector2.right * Mathf.Sign(transform.localScale.x);
        RaycastHit2D hit1 = Physics2D.Raycast(wallPrimaryCheck.position, dir, wallCheckDistance, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(wallSecondaryCheck.position, dir, wallCheckDistance, groundLayer);
        if (hit1.collider != null)
        {
            lastWallNormalX = hit1.normal.x;
            return true;
        }
        if (hit2.collider != null)
        {
            lastWallNormalX = hit2.normal.x;
            return true;
        }
        return false;
    }

    private void HandleAnimEvents()
    {
        animator.SetFloat(anim_param_move, rb.linearVelocity.x);
        animator.SetFloat(anim_param_jump, rb.linearVelocity.y);
        animator.SetBool(anim_param_grounded, isGrounded);
        animator.SetBool(anim_param_wallSliding, currentState == PlayerState.WallSlide);
    }

    private void FlipPlayerSprite()
    {
        if (rb.linearVelocity.x > 0.1f) transform.localScale = new Vector3(1, 1, 1);
        else if (rb.linearVelocity.x < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallPrimaryCheck.position, wallPrimaryCheck.position + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(wallSecondaryCheck.position, wallSecondaryCheck.position + Vector3.right * wallCheckDistance);
    }
    #endregion
}
