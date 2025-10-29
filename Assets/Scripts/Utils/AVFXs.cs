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

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
}
