using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthController : MonoBehaviour
{
    public int currentHealth { get => PlayerStats.instance.CurrentHealth; set => PlayerStats.instance.CurrentHealth = value; }

    [Header("Health Settings")]
    [SerializeField, Tooltip("Event triggered when the player dies")] private GameObject deathEffect;

    private static readonly int anim_param_hurt = Animator.StringToHash("Hurt");
    private int maxHealth;
    private Animator animator;
    private Character_AVFXs aVeffects;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        aVeffects = GetComponent<Character_AVFXs>();
    }

    public void TakeDamage(int damage, Transform damageSource)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        PlayerDoHurtSequence(damageSource);

        if (currentHealth <= 0) PlayerDeathSequence();
    }

    public void PlayerDies() => PlayerDeathSequence();

    private void PlayerDoHurtSequence(Transform damageSource)
    {
        animator.SetTrigger(anim_param_hurt);
        aVeffects?.SpriteFlash();
        aVeffects?.Knockback(damageSource.position.x);
        // Additional hurt logic (e.g., invincibility frames, sound effects) can be added here.                             
    }

    private void PlayerDeathSequence()
    {
        // Implement death logic here (e.g., respawn, game over screen, etc.)
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        print("start coroutine in health controller.");
        GameManager.Instance.StartPlayerRespawnCo();
        this.gameObject.SetActive(false);
    }
}