using System;
using System.Collections;
using UnityEngine;

public class Trap_Bouncer : MonoBehaviour
{
    [Header("Bouncer Trap Settings")]
    [SerializeField] private Sprite bouncerActiveSprite;
    [SerializeField] private Sprite bouncerInactiveSprite;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private Vector2 pushDirection = Vector2.up;
    [SerializeField] private float pushDuration = 0.5f;
    [SerializeField] private float deactivateDelay = 0.5f;
    [SerializeField] private bool canBounce = true;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBounce) return;
        if (collision.CompareTag("Player"))
        {
            ActivateBouncer();
            collision.GetComponent<PlayerMovementController>().Push(transform.up * bounceForce, pushDuration);
            StartCoroutine(DeactivateBouncerAfterDelayCo(deactivateDelay));
        }
    }

    private IEnumerator DeactivateBouncerAfterDelayCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        InactivateBouncer();
    }

    private void ActivateBouncer()
    {
        spriteRenderer.sprite = bouncerActiveSprite;
        canBounce = false;
    }

    private void InactivateBouncer()
    {
        spriteRenderer.sprite = bouncerInactiveSprite;
        canBounce = true;
    }
}
