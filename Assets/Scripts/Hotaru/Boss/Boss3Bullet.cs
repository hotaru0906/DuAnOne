using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float moveSpeed = 5f;
    public float lifeTime = 3f; // Thời gian tồn tại tối đa của viên đạn
    public int damage = 20;
    
    [Header("Components")]
    public Rigidbody2D rb;
    public Collider2D bulletCollider;
    
    private Vector2 moveDirection;
    private bool hasHitPlayer = false;
    
    void Start()
    {
        // Tự động lấy components nếu chưa được gán
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        if (bulletCollider == null)
        {
            bulletCollider = GetComponent<Collider2D>();
        }
        
        // Đặt collider làm trigger để detect collision
        if (bulletCollider != null)
        {
            bulletCollider.isTrigger = true;
        }
        
        // Tự hủy sau thời gian tồn tại
        StartCoroutine(DestroyAfterTime());
        
        Debug.Log("Boss3 Bullet created");
    }
    
    void FixedUpdate()
    {
        // Di chuyển viên đạn
        if (rb != null && !hasHitPlayer)
        {
            rb.velocity = moveDirection * moveSpeed;
        }
    }
    
    // Thiết lập hướng di chuyển của viên đạn
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        
        // Xoay viên đạn theo hướng di chuyển (optional)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    // Thiết lập hướng di chuyển từ vị trí hiện tại đến target
    public void SetTargetDirection(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        SetDirection(direction);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra va chạm với player
        if (other.CompareTag("Player") && !hasHitPlayer)
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null && playerScript.isInvincible)
            {
                Debug.Log("Player is invincible. Bullet will not disappear or deal damage.");
                return; // Không làm gì nếu player đang bất tử
            }

            hasHitPlayer = true;

            // Gây damage cho player
            DamagePlayer(other.gameObject);

            // Hủy viên đạn
            DestroyBullet();
        }
    }
    
    void DamagePlayer(GameObject player)
    {
        Debug.Log($"Boss3 Bullet hit Player! Damage: {damage}");
        
        Player playerHealth = player.GetComponent<Player>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
    
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        
        if (!hasHitPlayer)
        {
            Debug.Log("Boss3 Bullet destroyed after timeout");
            DestroyBullet();
        }
    }
    
    void DestroyBullet()
    {
        // Dừng di chuyển
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        // Có thể thêm effect khi bullet bị phá hủy ở đây
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
    
    // Public method để thiết lập các thông số viên đạn
    public void SetBulletProperties(float speed, float lifetime, int bulletDamage)
    {
        moveSpeed = speed;
        lifeTime = lifetime;
        damage = bulletDamage;
    }
}
