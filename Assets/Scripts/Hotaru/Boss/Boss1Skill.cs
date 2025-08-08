using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Skill : MonoBehaviour
{
    [Header("Skill Settings")]
    public int damage = 10;
    public GameObject hitbox;
    
    void Start()
    {
        // Đảm bảo hitbox tắt khi bắt đầu
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
        
        Debug.Log($"Boss1Skill object spawned: {gameObject.name}");
    }

    void Update()
    {
        // Có thể thêm logic update nếu cần (ví dụ: di chuyển, hiệu ứng)
    }
    
    // Animation Event Method 1: Bật hitbox
    public void ActivateHitbox()
    {
        if (hitbox != null)
        {
            hitbox.SetActive(true);
            Debug.Log($"Boss1Skill: Hitbox activated on {gameObject.name}");
        }
    }
    
    // Animation Event Method 2: Tắt hitbox
    public void DeactivateHitbox()
    {
        if (hitbox != null)
        {
            hitbox.SetActive(false);
            Debug.Log($"Boss1Skill: Hitbox deactivated on {gameObject.name}");
        }
    }
    
    public void DestroySkillObject()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();
            if (playerScript != null)
            {
                if (hitbox != null && hitbox.activeSelf) // Ensure hitbox is active
                {
                    playerScript.TakeDamage(damage); // Deal damage to the player
                    Debug.Log($"Player hit by {gameObject.name}, dealt {damage} damage.");
                }
                else
                {
                    Debug.LogWarning($"Hitbox is not active on {gameObject.name}, no damage dealt. Hitbox active state: {hitbox?.activeSelf}");
                }
            }
            else
            {
                Debug.LogError($"Player script not found on collided object: {collision.name}");
            }
        }
        else
        {
            Debug.Log($"Collision detected with non-player object: {collision.name}");
        }
    }
}
