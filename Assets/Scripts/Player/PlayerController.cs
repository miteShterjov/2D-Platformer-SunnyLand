using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Running, Sprinting, Jumping, Falling }

    public static readonly int anim_param_move = Animator.StringToHash("xVelocity");
    public static readonly int anim_param_jump = Animator.StringToHash("yVelocity");
    public static readonly int anim_param_grounded = Animator.StringToHash("isGrounded");


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

    [Space]
    [Header("Collision Detection")]
    [SerializeField, Tooltip("Layer mask for ground detection")] private LayerMask groundLayer;
    [SerializeField, Tooltip("Distance to check for ground")] private float groundCheckDistance = 0.1f;
    [SerializeField, Tooltip("Point to check for ground")] private Transform groundCheckPoint;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerState currentState { get; set; }
    private InputSystem_Actions inputActions;
    private Vector2 movementInput;
    private int jumpCount;
    private int remainingJumps;
    private bool isGrounded;
    private float coyoteTimer;
    private float bufferTimer;
    private bool jumpHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        inputActions = new InputSystem_Actions();
    }

    void Start()
    {
        jumpCount = maxJumps;

        inputActions.Player.Move.performed += ctx => currentState = PlayerState.Running;
        inputActions.Player.Move.canceled += ctx => currentState = PlayerState.Idle;
        inputActions.Player.Sprint.performed += ctx => currentState = PlayerState.Sprinting;
        inputActions.Player.Sprint.canceled += ctx => currentState = PlayerState.Running;
        inputActions.Player.Jump.performed += ctx => PlayerJumpStarted(ctx);
        inputActions.Player.Jump.canceled += ctx => PlayerJumpCanceled(ctx);

    }

    void Update()
    {
        HandlePlayerState();
        HandleAnimEvents();

        // Ground check
        bool wasGrounded = isGrounded;
        isGrounded = IsGrounded();

        // Reset jumps on landing
        if (isGrounded && !wasGrounded)
        {
            // Landed: reset jumps and resolve immediate movement state
            remainingJumps = maxJumps;

            currentState = inputActions.Player.Move.IsPressed() ? PlayerState.Running : PlayerState.Idle;
        }

        // Timers
        coyoteTimer = isGrounded ? coyoteTime : Mathf.Max(0f, coyoteTimer - Time.deltaTime);
        bufferTimer = Mathf.Max(0f, bufferTimer - Time.deltaTime);
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

        // Better fall feel
        var v = rb.linearVelocity;
        if (v.y < -0.01f)
        {
            v += Vector2.up * Physics2D.gravity.y * rb.gravityScale * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (v.y > 0.01f && !jumpHeld)
        {
            v += Vector2.up * Physics2D.gravity.y * rb.gravityScale * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
        rb.linearVelocity = v;

        // Optional: head-bonk ceiling check (simple)
        // If using Tilemap or a top check, clamp v.y when touching ceiling.
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

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
        if (inputActions.Player.Sprint.IsPressed())
        {
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
        // Set clean upward takeoff
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // If grounded via coyote, we’re spending the first jump now.
        // With remainingJumps model, just decrement if not grounded.
        if (!isGrounded) remainingJumps = Mathf.Max(remainingJumps - 1, 0);
        else remainingJumps = Mathf.Max(maxJumps - 1, 0);
    }

    private Vector2 GetVelocity() => inputActions.Player.Move.ReadValue<Vector2>();

    private void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        FlipPlayerSprite();
    }

    private bool IsGrounded() => Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckDistance, groundLayer);

    private void HandleAnimEvents()
    {
        animator.SetFloat(anim_param_move, rb.linearVelocity.x);
        animator.SetFloat(anim_param_jump, rb.linearVelocity.y);
        animator.SetBool(anim_param_grounded, isGrounded);
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
    }
}
