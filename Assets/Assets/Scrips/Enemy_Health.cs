using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 3;

    private int currentHealth;

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Pilobolus_Crystal ai_pilobolus = GetComponent<Pilobolus_Crystal>();
        if (ai_pilobolus != null) ai_pilobolus.enabled = false;

        Destroy(gameObject);
    }
}