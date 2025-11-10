using UnityEngine;

public class Startline : MonoBehaviour
{
    public static readonly int anim_param_start = Animator.StringToHash("triggered");

    [SerializeField] private bool isFliped = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (isFliped) spriteRenderer.flipX = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
