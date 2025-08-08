using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireColumn : MonoBehaviour
{
    [Header("Fire Column Settings")]
    public int damage = 20;
    public Animator animator;
    public BoxCollider2D fireCollider; // Collider để kiểm tra va chạm

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Animation Event: gọi từ animation start để bật collider
    public void EnableCollider()
    {
        if (fireCollider != null)
        {
            fireCollider.enabled = true; // Bật collider để nhận va chạm
        }
    }
    public void DisableCollider()
    {
        if (fireCollider != null)
        {
            fireCollider.enabled = false; // Tắt collider khi không cần thiết
        }
    }
    public void DestroyFireColumn()
    {
        Destroy(gameObject, 1f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
            }
        }
    }
}
