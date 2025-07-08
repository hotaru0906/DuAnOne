using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Skill : MonoBehaviour
{
    [Header("Skill Settings")]
    public GameObject hitbox; // Hitbox để gây damage
    
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
        else
        {
            Debug.LogWarning($"Boss1Skill: Cannot activate hitbox - Hitbox is null on {gameObject.name}");
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
        else
        {
            Debug.LogWarning($"Boss1Skill: Cannot deactivate hitbox - Hitbox is null on {gameObject.name}");
        }
    }
    
    // Animation Event Method 3: Destroy gameobject
    public void DestroySkillObject()
    {
        Debug.Log($"Boss1Skill: Destroying skill object {gameObject.name}");
        
        // Đảm bảo hitbox tắt trước khi destroy
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
        
        // Destroy gameobject
        Destroy(gameObject);
    }
    
    // Optional: Method để destroy sau một khoảng thời gian (backup safety)
    public void DestroyAfterDelay(float delay)
    {
        StartCoroutine(DestroyAfterDelayCoroutine(delay));
    }
    
    IEnumerator DestroyAfterDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (gameObject != null)
        {
            Debug.Log($"Boss1Skill: Auto-destroying skill object {gameObject.name} after {delay} seconds");
            DestroySkillObject();
        }
    }
    
    // Optional: Collision detection nếu cần
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hitbox != null && hitbox.activeInHierarchy)
        {
            Debug.Log($"Boss1Skill: Hit player with {gameObject.name}!");
            // Có thể thêm logic damage player ở đây
            // other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
