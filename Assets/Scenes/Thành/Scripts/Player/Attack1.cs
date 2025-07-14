using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : MonoBehaviour
{
    private int damage = 10;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D attack1)
    {
        EnemyTest enemy = attack1.GetComponent<EnemyTest>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Enemy hit by Attack1, dealt " + damage + " damage.");
        }
    }
}
