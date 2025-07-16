using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pltest : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform tf;
    public Vector2 dichuyen;
    public float Speed = 7f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        dichuyen.x = Input.GetAxisRaw("Horizontal");
    }
    
    void FixedUpdate()
    {
        rb.velocity = dichuyen * Speed;
    }

    void Dichuyenlatmat()
    {
        // Lật mặt 
        if (dichuyen.x != 0)
        {
            if (dichuyen.x > 0)
            {
                tf.localScale = new Vector2(1, 1);
            }
            else
            {
                tf.localScale = new Vector2(-1, 1);
            }
        }
    }
}
