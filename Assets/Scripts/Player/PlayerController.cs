using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Running,
        Sprinting,
        Jumping,
        Falling,
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintMultiplier = 1.6f;
    [SerializeField] private float accelGround = 45f;
    [SerializeField] private float accelAir = 25f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField, Tooltip("Total jumps allowed per airtime (2 = double jump)")] private int maxJumps = 2;
    [SerializeField, Tooltip("Grace time after leaving ground")] private float coyoteTime = 0.12f;
    [SerializeField, Tooltip("Accept jump pressed slightly before landing")] private float jumpBuffer = 0.12f;
    [SerializeField, Tooltip("Extra gravity when falling")] private float fallMultiplier = 2.2f;
    [SerializeField, Tooltip("Extra gravity when rising and jump is released")] private float lowJumpMultiplier = 2.0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug State")] 
    [SerializeField] private PlayerState currentState = PlayerState.Idle;

    // Animator parameters (optional)
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimGrounded = Animator.StringToHash("Grounded");
    private static readonly int AnimState = Animator.StringToHash("State");

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;

    // Input System
    private InputSystem_Actions input;

    // Runtime/input
    private float moveAxis;        // -1..1 horizontal
    private bool sprintHeld;       // sprint modifier
    private bool jumpHeld;         // for variable jump height
    private float lastGrounded;    // coyote timer
    private float lastJumpPressed; // buffer timer
    private int jumpsUsed;         // jumps since last grounded
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Enable();

        // Jump events
        input.Player.Jump.started += OnJumpStarted;
        input.Player.Jump.canceled += OnJumpCanceled;

        // Sprint held
        input.Player.Sprint.started += ctx => sprintHeld = true;
        input.Player.Sprint.canceled += ctx => sprintHeld = false;
    }

    private void OnDisable()
    {
        input.Player.Jump.started -= OnJumpStarted;
        input.Player.Jump.canceled -= OnJumpCanceled;
        input.Disable();
    }

    private void Update()
    {
        // Read move axis
        Vector2 move = input.Player.Move.ReadValue<Vector2>();
        moveAxis = Mathf.Clamp(move.x, -1f, 1f);

        // Ground check
        if (groundCheckPoint != null)
        {
            bool groundedNow = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
            if (groundedNow && !isGrounded)
            {
                // landed — reset jumps
                jumpsUsed = 0;
            }
            isGrounded = groundedNow;
        }

        // Timers
        if (isGrounded) lastGrounded = coyoteTime; else lastGrounded -= Time.deltaTime;
        lastJumpPressed -= Time.deltaTime;

        // State resolution
        if (!isGrounded)
            currentState = rb.linearVelocity.y >= 0.01f ? PlayerState.Jumping : PlayerState.Falling;
        else
            currentState = Mathf.Abs(moveAxis) > 0.01f ? (sprintHeld ? PlayerState.Sprinting : PlayerState.Running) : PlayerState.Idle;

        // Facing
        if (sprite != null)
        {
            if (moveAxis > 0.01f) sprite.flipX = false;
            else if (moveAxis < -0.01f) sprite.flipX = true;
        }

        // Animator
        if (animator != null)
        {
            animator.SetFloat(AnimSpeed, Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool(AnimGrounded, isGrounded);
            animator.SetInteger(AnimState, (int)currentState);
        }
    }

    private void FixedUpdate()
    {
        // Horizontal movement with smoothing
        float targetSpeed = moveAxis * moveSpeed * (sprintHeld ? sprintMultiplier : 1f);
        float accel = isGrounded ? accelGround : accelAir;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        // Jump consumption (buffer + coyote + extra jumps)
        if (lastJumpPressed > 0f && (lastGrounded > 0f || jumpsUsed < maxJumps))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (isGrounded) jumpsUsed = 1; else jumpsUsed++;
            lastJumpPressed = 0f;
            lastGrounded = 0f;
        }

        // Better jump feel
        if (rb.linearVelocity.y < -0.01f)
        {
            // Falling
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0.01f && !jumpHeld)
        {
            // Rising but jump released — shorter jump
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        jumpHeld = true;
        lastJumpPressed = jumpBuffer;
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        jumpHeld = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}