using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDm : MonoBehaviour
{
    public int Health = 100; // Player's health
    public int MaxHealth = 100; // Maximum health
    public Rigidbody2D rb; // Rigidbody component for physics interactions
    public Animator animator; // Animator component for animations


    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Animator animator = GetComponent<Animator>();
        Health = MaxHealth; // Initialize health to maximum
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamage(int damage)
    {
        Health -= damage; // Reduce health by damage amount
        if (Health <= 0)
        {
            animator.Play("Die");
            Die(); // Call the Die method if health is zero or below
        }
    }
    private void Die()
    {
        Debug.Log("Player has died.");
    }
}
