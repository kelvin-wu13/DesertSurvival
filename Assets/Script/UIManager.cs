using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;
    public Image healthBar;

    private GunController gunController;
    private HealthSystem playerHealth;

    void Start()
    {
        gunController = FindObjectOfType<GunController>();
        playerHealth = FindObjectOfType<PlayerMovement>().GetComponent<HealthSystem>();

        if (gunController == null || playerHealth == null)
        {
            Debug.LogError("GunController or PlayerHealth not found in the scene!");
        }
        else
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(playerHealth.GetHealthPercentage());
        }
        UpdateAmmoDisplay();
    }

    void Update()
    {
        UpdateAmmoDisplay();
    }

    void UpdateAmmoDisplay()
    {
        if (gunController != null && ammoText != null)
        {
            ammoText.text = $" {gunController.GetCurrentAmmo()}/30";
        }
    }

    void UpdateHealthUI(float healthPercentage)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {playerHealth.GetCurrentHealth():F0}";
        }
        if (healthBar != null)
        {
            healthBar.fillAmount = healthPercentage;
        }
    }
}