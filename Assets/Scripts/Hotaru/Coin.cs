using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1; // The amount of gold this coin represents

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.gold += value; // Add the coin's value to the player's gold
                Debug.Log($"Player collected {value} gold. Total gold: {player.gold}");
                Destroy(gameObject); // Destroy the coin after collection
            }
        }
    }
}
