using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Character_AVFXs : MonoBehaviour
{
    public bool IsKnocked => isKnocked;

    [Header("Sprite Flash")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Material flashMaterial;
    [Space]
    [Header("Knockback")]
    [SerializeField, Tooltip("Force applied when the player is knocked back")] private Vector2 knockbackForce = new Vector2(10f, 5f);
    [SerializeField, Tooltip("Duration of the knockback effect")] private float knockbackDuration = 1f;
    [SerializeField, Tooltip("Is the player currently knocked back?")] private bool isKnocked = false;
    [SerializeField, Tooltip("Can the player be knocked back?")] private bool canBeKnocked = true;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private CinemachineImpulseSource source;


    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
        source = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        originalMaterial = spriteRenderer.material;
    }

    // EFFECT: FLASH
    // Flash sprite by changing its material temporarily to a flash material and back
    public void SpriteFlash()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material = originalMaterial;
    }

    // EFFECT: KNOCKBACK
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

    // EFFECT: SCREEN SHAKE
    public void ShakeScreen() => source.GenerateImpulse();
}
