using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : MonoBehaviour
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
            // Tìm component EnemyTest hoặc các enemy script khác
            var enemy = other.GetComponent<MonoBehaviour>();
            if (enemy != null)
            {
                // Thử gọi method TakeDamage nếu có
                var takeDamageMethod = enemy.GetType().GetMethod("TakeDamage");
                if (takeDamageMethod != null)
                {
                    takeDamageMethod.Invoke(enemy, new object[] { player.attack1Damage });
                    Debug.Log("Enemy hit by Attack1, dealt " + player.attack1Damage + " damage.");
                }
            }
        }
        else if (other.CompareTag("Boss"))
        {
            // Hỗ trợ tất cả các loại Boss
            Boss1 boss1 = other.GetComponent<Boss1>();
            if (boss1 != null)
            {
                boss1.TakeDamage(player.attack1Damage);
                Debug.Log("Boss1 hit by Attack1, dealt " + player.attack1Damage + " damage.");
                return;
            }
            
            Boss2 boss2 = other.GetComponent<Boss2>();
            if (boss2 != null)
            {
                boss2.TakeDamage(player.attack1Damage);
                Debug.Log("Boss2 hit by Attack1, dealt " + player.attack1Damage + " damage.");
                return;
            }
            
            Boss3 boss3 = other.GetComponent<Boss3>();
            if (boss3 != null)
            {
                boss3.TakeDamage(player.attack1Damage);
                Debug.Log("Boss3 hit by Attack1, dealt " + player.attack1Damage + " damage.");
                return;
            }
            
            Boss4 boss4 = other.GetComponent<Boss4>();
            if (boss4 != null)
            {
                boss4.TakeDamage(player.attack1Damage);
                Debug.Log("Boss4 hit by Attack1, dealt " + player.attack1Damage + " damage.");
                return;
            }
        }
    }
}
