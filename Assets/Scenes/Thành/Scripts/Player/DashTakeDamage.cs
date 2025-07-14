using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTakeDamage : MonoBehaviour
{
    public int health = 100;
    private Rigidbody2D rb;
    public int maxHealth = 100;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }
    void Update()
    {

    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
            Debug.Log("Player has taken damage: " + damage + ". Remaining health: " + health);
        }
    }
    private void Die()
    {
        Debug.Log("Player has died.");
        Destroy(gameObject);
    }
}
