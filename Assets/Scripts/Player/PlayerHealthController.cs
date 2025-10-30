using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthController : MonoBehaviour
{
    public static readonly int anim_param_hurt = Animator.StringToHash("Hurt");

    private int maxHealth;
    private int currentHealth;
    private Animator animator;
    private AVFXs aVeffects;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        aVeffects = GetComponent<AVFXs>();
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

    private void PlayerDoHurtSequence()
    {
        animator.SetTrigger(anim_param_hurt);
        aVeffects?.SpriteFlash();
        aVeffects?.knockback();
        // Additional hurt logic (e.g., invincibility frames, sound effects) can be added here.                             
    }

    private void PlayerDeathSequence()
    {
        print("Player has died.");

        // Implement death logic here (e.g., respawn, game over screen, etc.)
        this.gameObject.SetActive(false);
    }
}