using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);  // Ensure health doesn't go below 0
        
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);

        OnHealthChanged.Invoke(GetHealthPercentage());

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    private void HandleDeath()
    {
        OnDeath.Invoke();
        ZombieBehaviorController zombieController = GetComponent<ZombieBehaviorController>();
        if (zombieController != null)
        {
            zombieController.HandleDeath();
        }
        else
        {
            // This is likely the player
            Debug.Log("Player has died!");
            // Add any player-specific death logic here
        }
    }
}