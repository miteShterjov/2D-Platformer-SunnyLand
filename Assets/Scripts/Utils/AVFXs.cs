using System;
using System.Collections;
using UnityEngine;

public class AVFXs : MonoBehaviour
{
    public bool IsKnocked => isKnocked;

    [Header("Sprite Flash")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField][Range(0, 1)] private float alphaIndex = 0.6f;
    [Space]
    [Header("Knockback")]
    [SerializeField, Tooltip("Force applied when the player is knocked back")] private Vector2 knockbackForce = new Vector2(10f, 5f);
    [SerializeField, Tooltip("Duration of the knockback effect")] private float knockbackDuration = 1f;
    [SerializeField, Tooltip("Is the player currently knocked back?")] private bool isKnocked = false;
    [SerializeField, Tooltip("Can the player be knocked back?")] private bool canBeKnocked = true;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
    }

    public void SpriteFlash()
    {
        StartCoroutine(FlashSprite());
    }

    private IEnumerator FlashSprite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(flashColor.r, flashColor.g, flashColor.b, alphaIndex);

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
    }

    // Simple default knockback: away from facing direction, with default forces
    public void Knockback()
    { 
        if (isKnocked || !canBeKnocked) return;

        StartCoroutine(KnockbackCoroutine());
        
        rb.linearVelocity = new Vector2(knockbackForce.x * (transform.localScale.x > 0 ? -1 : 1), knockbackForce.y);
    }

    private IEnumerator KnockbackCoroutine()
    {
        isKnocked = true;
        canBeKnocked = false;
        // Determine knockback direction based on current facing
        int knockbackDir = transform.localScale.x > 0 ? -1 : 1;

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        canBeKnocked = true;
    }
}
