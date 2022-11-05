using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public PlayerCharMvmt player;
    public AICharacter aiCharacter;
    private void Start()
    {
        player = GetComponentInParent<PlayerCharMvmt>();
        aiCharacter = GetComponentInParent<AICharacter>();
        healthBar = GetComponent<Slider>();

        if (player != null)
        {//case where this is attached to player character
            healthBar.maxValue = player.currentHealth;
            healthBar.value = player.currentHealth;
        }
        else if (aiCharacter != null)
        {//case where this is attached to ai character
            healthBar.maxValue = aiCharacter.currentHealth;
            healthBar.value = aiCharacter.currentHealth;
        }
    }

    public void SetHealth(int hp)
    {//sets the value of the health bar
        healthBar.value = hp;
    }
    public void SetMaxHealth(int maxHealth, int currentHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }
}
