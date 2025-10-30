using System.Collections;
using UnityEngine;

public class AVFXs : MonoBehaviour
{
    [Header("Sprite Flash")]
    [SerializeField] private bool enableSpriteFlash = true; 
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField][Range(0, 1)] private float alphaIndex = 0.6f;
    [Space]
    [Header("Knockback")]
    [SerializeField, Tooltip("Default horizontal impulse for knockback")] private float knockbackImpulse = 8f;
    [SerializeField, Tooltip("Additional upward impulse for knockback")] private float knockbackUpwardImpulse = 2f;
    [SerializeField, Tooltip("Play sprite flash on knockback")] private bool flashOnKnockback = true;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
    }

    public void SpriteFlash()
    {
        if (!enableSpriteFlash) return;

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
    public void knockback()
    {
        if (rb == null) return;
        Vector2 dir = new Vector2(-Mathf.Sign(transform.localScale.x), 1f).normalized;
        ApplyKnockback(dir, knockbackImpulse, knockbackUpwardImpulse);
    }

    // Knockback away from a world-space source position
    public void KnockbackFrom(Vector2 sourcePosition, float? horizontalImpulse = null, float? upwardImpulse = null)
    {
        if (rb == null) return;
        Vector2 dir = ((Vector2)transform.position - sourcePosition).normalized;
        ApplyKnockback(dir, horizontalImpulse ?? knockbackImpulse, upwardImpulse ?? knockbackUpwardImpulse);
    }

    // Knockback in a specific world-space direction
    public void Knockback(Vector2 direction, float? horizontalImpulse = null, float? upwardImpulse = null)
    {
        if (rb == null) return;
        Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.zero;
        if (dir == Vector2.zero) return;
        ApplyKnockback(dir, horizontalImpulse ?? knockbackImpulse, upwardImpulse ?? knockbackUpwardImpulse);
    }

    private void ApplyKnockback(Vector2 dir, float horizontal, float upward)
    {
        // Compose impulse: push along dir with extra upwards lift
        Vector2 impulse = dir * horizontal;
        impulse.y += Mathf.Abs(upward);
        rb.AddForce(impulse, ForceMode2D.Impulse);

        if (flashOnKnockback) SpriteFlash();
    }
}
