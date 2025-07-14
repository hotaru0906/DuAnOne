using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack2 : MonoBehaviour
{
    private int damage = 15;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D attack2)
    {
        EnemyTest enemy = attack2.GetComponent<EnemyTest>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Enemy hit by Attack2, dealt " + damage + " damage.");
        }
    }
}
