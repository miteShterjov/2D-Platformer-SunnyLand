using System.Security.Cryptography;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float MaxStamina => maxStamina;
    public float CurrentStamina { get => currentStamina; set => currentStamina = value; }

    [Header("Basic Stats")]
    [SerializeField, Tooltip("Maximum health of the player")] private int maxHealth = 100;
    [SerializeField, Tooltip("Current health of the player")] private int currentHealth;
    [SerializeField, Tooltip("Maximum stamina of the player")] private float maxStamina = 100f;
    [SerializeField, Tooltip("Current stamina of the player")] private float currentStamina;
    [SerializeField, Tooltip("Stamina regeneration rate per second")] private float staminaRegenRate = 5f;
    [SerializeField, Tooltip("Stamina consumption rate per second")] private float staminaSpendRate  = 7f;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        RegenerateStamina();
    }

    public void SpendStamina()
    {
        if (currentStamina <= 0) return;

        currentStamina -= staminaSpendRate * Time.deltaTime;
        currentStamina = Mathf.Max(currentStamina, 0);
    }
    
    public void SpendStamina(float amount)
    {
        if (currentStamina <= 0) return;

        currentStamina -= amount;
        currentStamina = Mathf.Max(currentStamina, 0);
    }

    public void RegenerateStamina()
    {
        if (currentStamina >= maxStamina) return;

        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    public void RegenerateStamina(float amount)
    {
        if (currentStamina >= maxStamina) return;

        currentStamina += amount;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    public void RegenerateHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}
