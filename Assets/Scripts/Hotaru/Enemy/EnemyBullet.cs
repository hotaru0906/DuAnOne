using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
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
            Destroy(gameObject);
        }
        // Nếu muốn đạn biến mất khi chạm vào tường hoặc vật cản, có thể thêm điều kiện tại đây
    }
}
