using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    #region Properties
    [Header("Health Config")]
    [SerializeField] private PlayerStats playerStats;
    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    [Header("Realtime Health State")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float defense;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool isDead;

    public float MaxHP { get { return maxHealth; } }
    public float CurrentHP { get { return currentHealth; } }
    public bool IsDead { get { return isDead; } }
    public int hitCounter { get; private set; } = 0;

    [HideInInspector] public Action OnHit;
    [HideInInspector] public Action OnDeath;
    public Action<float> OnHealthChanged;

    private float hitTimer = 0;
    #endregion

    public void Initialize(PlayerStats config)
    {
        maxHealth = config.maxHp;
        defense = config.defense;
        currentHealth = maxHealth;
        isDead = false;
    }

    void Update()
    {   // TODO : Player의 hitTime기록후 해당 Timeout이 지났을때만 OnDamage될수있도록.

    }

    public void OnDamage(float damage)
    {
        if (isDead) return;

        float prevHealth = currentHealth;
        float calcDamage = Mathf.Max(damage - defense, 0f);
        currentHealth -= calcDamage;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            OnDeath?.Invoke();
        }
        else
        {
            OnHit?.Invoke();
        }
    }
}
