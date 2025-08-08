using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack2 : MonoBehaviour
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
                int damage = player.GetAttack2Damage();
                if (player.IsCriticalHit())
                {
                    damage = Mathf.CeilToInt(damage * 1.5f);
                    Debug.Log("Critical Hit on Enemy!");
                }
                enemy.TakeDamage(damage);
                Debug.Log($"Enemy {other.name} hit by Attack1, dealt {damage} damage.");
            }
            var imp = other.GetComponent<Imp>();
            if (imp != null)
            {
                int damage = player.GetAttack2Damage();
                if (player.IsCriticalHit())
                {
                    damage = Mathf.CeilToInt(damage * 1.5f);
                    Debug.Log("Critical Hit on Enemy!");
                }
                imp.TakeDamage(damage);
                Debug.Log($"Enemy {other.name} hit by Attack2, dealt {damage} damage.");
            }
            var nb = other.GetComponent<NightBorne>();
            if (nb != null)
            {
                int damage = player.GetAttack2Damage();
                if (player.IsCriticalHit())
                {
                    damage = Mathf.CeilToInt(damage * 1.5f);
                    Debug.Log("Critical Hit on Enemy!");
                }
                nb.TakeDamage(damage);
                Debug.Log($"Enemy {other.name} hit by Attack2, dealt {damage} damage.");
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
                boss1.TakeDamage(player.attack2Damage);
                Debug.Log("Boss1 hit by Attack2, dealt " + player.attack2Damage + " damage.");
                return;
            }

            Boss2 boss2 = other.GetComponent<Boss2>();
            if (boss2 != null)
            {
                boss2.TakeDamage(player.attack2Damage);
                Debug.Log("Boss2 hit by Attack2, dealt " + player.attack2Damage + " damage.");
                return;
            }

            Boss3 boss3 = other.GetComponent<Boss3>();
            if (boss3 != null)
            {
                boss3.TakeDamage(player.attack2Damage);
                Debug.Log("Boss3 hit by Attack2, dealt " + player.attack2Damage + " damage.");
            }

            Boss4 boss4 = other.GetComponent<Boss4>();
            if (boss4 != null)
            {
                boss4.TakeDamage(player.attack2Damage);
                Debug.Log("Boss4 hit by Attack2, dealt " + player.attack2Damage + " damage.");
            }
            Boss5 boss5 = other.GetComponent<Boss5>();
            if (boss5 != null)
            {
                int damage = player.GetAttack2Damage();
                if (player.IsCriticalHit())
                {
                    damage = Mathf.CeilToInt(damage * 1.5f);
                    Debug.Log("Critical Hit on Enemy!");
                }
                boss5.TakeDamage(damage);
                Debug.Log("Boss5 hit by Attack1, dealt " + damage + " damage.");
                return;
            }
            Boss6 boss6 = other.GetComponent<Boss6>();
            if (boss6 != null)
            {
                int damage = player.GetAttack2Damage();
                if (player.IsCriticalHit())
                {
                    damage = Mathf.CeilToInt(damage * 1.5f);
                    Debug.Log("Critical Hit on Enemy!");
                }
                boss6.TakeDamage(damage);
                Debug.Log("Boss6 hit by Attack1, dealt " + damage + " damage.");
                return;
            }
            MiniBoss1 mb1 = other.GetComponent<MiniBoss1>();
            if (mb1 != null)
            {
                mb1.TakeDamage(player.attack2Damage);
                Debug.Log("MiniBoss1 hit by Attack2, dealt " + player.attack2Damage + " damage.");
                return;
            }
        }
    }
}
