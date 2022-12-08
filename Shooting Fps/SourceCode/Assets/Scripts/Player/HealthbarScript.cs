using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour
{
    [SerializeField]
    private Image healthbarSprite;

    [SerializeField]
    private float reduceSpeed = 2;

    private float target = 1f;

    public void UpdateHealthBar(float maxHealth, float CurrentHealt)
    {
        target = CurrentHealt / maxHealth;
    }

    void Update()
    {
        healthbarSprite.fillAmount = Mathf.MoveTowards(healthbarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }
}
