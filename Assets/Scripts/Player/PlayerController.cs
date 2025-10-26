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
    [SerializeField, Tooltip("Movement speed of the player")] private float moveSpeed = 5f;
    [SerializeField, Tooltip("Multiplier for sprinting speed")] private float sprintMultiplier = 1.5f;
    [SerializeField, Tooltip("Force applied when jumping")] private float jumpForce = 7f;
    [SerializeField, Tooltip("Maximum number of jumps available")] private int maxJumps = 2;
    [Space]
    [Header("Collision Check")]
    [SerializeField, Tooltip("Radius of the ground check sphere")] private float groundCheckRadius = 0.2f;
    [SerializeField, Tooltip("Layer mask for the ground")] private LayerMask groundLayer;
    [SerializeField, Tooltip("Point at which to check for ground")] private Transform groundCheckPoint;
    [SerializeField, Tooltip("Is the player grounded?")] private bool isGrounded;
    [Space]
    [Header("State")]
    [SerializeField] private PlayerState currentState;

    private Animator animator;
    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;
    
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentState = PlayerState.Idle;
    }
}