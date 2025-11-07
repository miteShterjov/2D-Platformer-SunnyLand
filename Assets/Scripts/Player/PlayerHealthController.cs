using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthController : MonoBehaviour
{
    public static readonly int anim_param_hurt = Animator.StringToHash("Hurt");
    
    [Header("Health Settings")]
    [SerializeField, Tooltip("Event triggered when the player dies")] private GameObject deathEffect;

    private int maxHealth;
    private int currentHealth;
    private Animator animator;
    private Character_AVFXs aVeffects;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        aVeffects = GetComponent<Character_AVFXs>();
    }

    void Start()
    {
        maxHealth = PlayerStats.instance.CurrentHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        PlayerDoHurtSequence();

        if (currentHealth <= 0) PlayerDeathSequence();
    }

    public void PlayerDies() => PlayerDeathSequence();

    private void PlayerDoHurtSequence()
    {
        animator.SetTrigger(anim_param_hurt);
        aVeffects?.SpriteFlash();
        aVeffects?.Knockback();
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