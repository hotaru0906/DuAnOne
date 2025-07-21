using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill1 : MonoBehaviour
{
    private Player player;
    
    void Start()
    {
        // Lấy reference tới Player component
        player = GetComponentInParent<Player>();
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (player == null) return;

        if (other.CompareTag("Enemy"))
        {
            // Tìm component Enemy script
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.skill1Damage);
                Debug.Log($"Enemy {other.name} hit by Skill1, dealt {player.skill1Damage} damage.");
            }
            else
            {
                Debug.LogWarning($"Enemy {other.name} does not have an Enemy script attached.");
            }
        }
        else if (other.CompareTag("Boss"))
        {
            // Hỗ trợ tất cả các loại Boss
            Boss1 boss1 = other.GetComponent<Boss1>();
            if (boss1 != null)
            {
                boss1.TakeDamage(player.skill1Damage);
                Debug.Log("Boss1 hit by Skill1, dealt " + player.skill1Damage + " damage.");
                return;
            }

            Boss2 boss2 = other.GetComponent<Boss2>();
            if (boss2 != null)
            {
                boss2.TakeDamage(player.skill1Damage);
                Debug.Log("Boss2 hit by Skill1, dealt " + player.skill1Damage + " damage.");
                return;
            }

            Boss3 boss3 = other.GetComponent<Boss3>();
            if (boss3 != null)
            {
                boss3.TakeDamage(player.skill1Damage);
                Debug.Log("Boss3 hit by Skill1, dealt " + player.skill1Damage + " damage.");
                return;
            }

            Boss4 boss4 = other.GetComponent<Boss4>();
            if (boss4 != null)
            {
                boss4.TakeDamage(player.skill1Damage);
                Debug.Log("Boss4 hit by Skill1, dealt " + player.skill1Damage + " damage.");
                return;
            }
        }
    }
}
