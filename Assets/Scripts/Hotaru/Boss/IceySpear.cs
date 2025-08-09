using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceySpear : MonoBehaviour
{
    [Header("Icey Spear Settings")]
    public float fallSpeed = 5f;
    public int damage = 20;
    public GameObject iceySpearGroundPrefab; // Prefab khi chạm ground
    public Animator animator;

    [Header("Sound Settings")]
    public AudioClip iceStartSound;
    public AudioSource audioSource;

    private Rigidbody2D rb;

    private bool hasHitGround = false;

    void Start()
    {
        if (audioSource != null && iceStartSound != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.PlayOneShot(iceStartSound);
        }
        {
            if (animator == null)
                animator = GetComponent<Animator>();
            // Khi xuất hiện, chạy animation start
            if (animator != null)
                animator.Play("Ice2_Start");

            // Xoay spear 90 độ
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);

            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
            }
        }
    }

    void Update()
    {
        // Không cần Update nếu dùng Rigidbody2D để rơi
    }

    // Animation Event: gọi từ animation start để chuyển sang loop
    public void SwitchToLoopAnimation()
    {
        if (animator != null)
        {
            animator.Play("Ice2_Repeat");
        }
        if (rb != null)
        {
            rb.gravityScale = 1f; // Bắt đầu rơi tự nhiên
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHitGround) return;
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
            }
            // Có thể thêm hiệu ứng hoặc destroy spear nếu muốn
        }
        else if (collision.CompareTag("Ground"))
        {
            hasHitGround = true;
            SpawnGroundSpear();
            Destroy(gameObject);
        }
    }

    private void SpawnGroundSpear()
    {
        if (iceySpearGroundPrefab != null)
        {
            Instantiate(iceySpearGroundPrefab, transform.position, Quaternion.identity);
        }
    }
}
