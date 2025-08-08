using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject coinPrefab;
    public int amount = 0;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OpenChest();
        }
    }

    public void OpenChest()
    {
        Destroy(gameObject);
        if (coinPrefab != null)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.value = amount;
            }

            // Apply random force to the coin
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                float randomXForce = Random.Range(-2f, 2f);
                float upwardForce = Random.Range(3f, 5f);
                coinRb.AddForce(new Vector2(randomXForce, upwardForce), ForceMode2D.Impulse);
            }
        }
        else
        {
            Debug.LogWarning("Coin prefab is not assigned. No coin will be dropped.");
        }
    }
}
