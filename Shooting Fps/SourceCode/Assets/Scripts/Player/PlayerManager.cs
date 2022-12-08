using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Stats")]
    private float maxHealth = 100;
    public float health;


    [Header("Health Bar")]
    [SerializeField]
    private Image healthBarSprite;
    [SerializeField]
    private HealthbarScript healthbar;


    private void Start()
    {
        health = maxHealth;

        healthbar.UpdateHealthBar(maxHealth, health);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthbar.UpdateHealthBar(maxHealth, health);

        if(health <= 0)
            Invoke(nameof(DestroyPlayer), 0.1f);
    }

    private void DestroyPlayer()
    {
        Destroy(gameObject);
    }

}
