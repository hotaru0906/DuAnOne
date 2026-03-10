using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomcs : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(75); // Gọi hàm TakeDamage của Player
            }
        }
    }
}
