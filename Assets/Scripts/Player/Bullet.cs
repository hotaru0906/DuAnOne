using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private float speed = 5f; // Tốc độ bay của viên đạn
    private float direction = 1f; // Hướng bay của viên đạn (1: phải, -1: trái)

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    public void SetDirection(float playerDirection)
    {
        direction = playerDirection;

        // Lật viên đạn theo hướng bay
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction); // Đảm bảo scale.x luôn dương trước khi nhân với hướng
        transform.localScale = scale;
    }

    private void Update()
    {
        // Di chuyển viên đạn theo hướng của Player
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
                    takeDamageMethod.Invoke(enemy, new object[] { damage });
                    Debug.Log("Enemy hit by Bullet, dealt " + damage + " damage.");
                }
            }
        }
        else if (other.CompareTag("Boss"))
        {
            // Hỗ trợ tất cả các loại Boss
            Boss1 boss1 = other.GetComponent<Boss1>();
            if (boss1 != null)
            {
                boss1.TakeDamage(damage);
                Debug.Log("Boss1 hit by Bullet, dealt " + damage + " damage.");
                return;
            }

            Boss2 boss2 = other.GetComponent<Boss2>();
            if (boss2 != null)
            {
                boss2.TakeDamage(damage);
                Debug.Log("Boss2 hit by Bullet, dealt " + damage + " damage.");
                return;
            }

            Boss3 boss3 = other.GetComponent<Boss3>();
            if (boss3 != null)
            {
                boss3.TakeDamage(damage);
                Debug.Log("Boss3 hit by Bullet, dealt " + damage + " damage.");
                return;
            }

            Boss4 boss4 = other.GetComponent<Boss4>();
            if (boss4 != null)
            {
                boss4.TakeDamage(damage);
                Debug.Log("Boss4 hit by Bullet, dealt " + damage + " damage.");
                return;
            }
            MiniBoss1 mb1 = other.GetComponent<MiniBoss1>();
            if (mb1 != null)
            {
                mb1.TakeDamage(damage);
                Debug.Log("MiniBoss1 hit by Attack1, dealt " + damage + " damage.");
                return;
            }
        }
        Destroy(gameObject,3f);
    }
}
