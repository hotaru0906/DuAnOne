using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack4 : MonoBehaviour
{
    private Player playerAttack;
    
    void Start()
    {
        // Lấy reference tới Player component
        playerAttack = GetComponentInParent<Player>();
        if (playerAttack == null)
        {
            playerAttack = FindObjectOfType<Player>();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerAttack == null) return;

        if (other.CompareTag("Enemy"))
        {
            // Tìm component Enemy script
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(playerAttack.attack3Damage);
                Debug.Log($"Enemy {other.name} hit by Attack31, dealt {playerAttack.attack3Damage} damage.");
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
                boss1.TakeDamage(playerAttack.attack3Damage);
                Debug.Log("Boss1 hit by Attack31, dealt " + playerAttack.attack3Damage + " damage.");
                return;
            }

            Boss2 boss2 = other.GetComponent<Boss2>();
            if (boss2 != null)
            {
                boss2.TakeDamage(playerAttack.attack3Damage);
                Debug.Log("Boss2 hit by Attack31, dealt " + playerAttack.attack3Damage + " damage.");
                return;
            }

            Boss3 boss3 = other.GetComponent<Boss3>();
            if (boss3 != null)
            {
                boss3.TakeDamage(playerAttack.attack3Damage);
                Debug.Log("Boss3 hit by Attack31, dealt " + playerAttack.attack3Damage + " damage.");
                return;
            }

            Boss4 boss4 = other.GetComponent<Boss4>();
            if (boss4 != null)
            {
                boss4.TakeDamage(playerAttack.attack3Damage);
                Debug.Log("Boss4 hit by Attack31, dealt " + playerAttack.attack3Damage + " damage.");
                return;
            }
            MiniBoss1 mb1 = other.GetComponent<MiniBoss1>();
            if (mb1 != null)
            {
                mb1.TakeDamage(playerAttack.attack3Damage);
                Debug.Log("MiniBoss1 hit by Attack3, dealt " + playerAttack.attack3Damage + " damage.");
                return;
            }
        }
    }
}
