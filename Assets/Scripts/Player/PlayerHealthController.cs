using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthController : MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;

    void Start()
    {
        maxHealth = PlayerStats.instance.CurrentHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0) PlayerDeathSequence();
    }

    private void PlayerDeathSequence()
    {
        print("Player has died.");

        // Implement death logic here (e.g., respawn, game over screen, etc.)
        this.gameObject.SetActive(false);
    }
}