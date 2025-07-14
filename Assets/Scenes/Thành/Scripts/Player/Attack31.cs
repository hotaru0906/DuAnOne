using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack31 : MonoBehaviour
{
    private int damage = 22;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D attack31)
    {
        EnemyTest enemy = attack31.GetComponent<EnemyTest>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Enemy hit by Attack3.1, dealt " + damage + " damage.");
        }
    }
}
