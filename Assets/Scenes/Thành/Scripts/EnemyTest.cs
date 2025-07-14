using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public Rigidbody2D rb;
    public int Health = 100;
    public int MaxHealth = 100;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Health = MaxHealth;
    }

    void Update()
    {

    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
            Debug.Log("Enemy has taken damage: " + damage + ". Remaining health: " + Health);
        }
    }
    private void Die()
    {
        Debug.Log("Enemy has died.");
        Destroy(gameObject);
    }
    public void OnTriggerEnter2D(Collider collision)
    {
        DashTakeDamage dash = collision.gameObject.GetComponent<DashTakeDamage>();
        if (dash != null)
        {
            dash.TakeDamage(10);
            Debug.Log("Player hit by EnemyTest, dealt 10 damage.");
        }

    }
    
}
