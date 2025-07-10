using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private NPC nearbyNPC = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        Vector2 movement = new Vector2(moveHorizontal, 0);
        rb.velocity = movement * speed;

        if (moveHorizontal < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveHorizontal > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.E) && nearbyNPC != null)
        {
            nearbyNPC.Interact();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null && npc.canInteract)
        {
            nearbyNPC = npc;
            Debug.Log("Press E to interact");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NPC>() == nearbyNPC)
        {
            nearbyNPC = null;
        }
    }
}
