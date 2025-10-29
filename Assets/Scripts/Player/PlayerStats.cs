using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public int CurrentHealth => currentHealth;
    public float CurrentStamina => currentStamina;

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

    private void RegenerateStamina()
    {
        if (currentStamina >= maxStamina) return;

        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }
}
