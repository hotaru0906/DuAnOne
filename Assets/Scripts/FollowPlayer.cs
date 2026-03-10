using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public string playerTag = "Player";
    public float followSpeed = 5f;
    private float initialX;
    private float initialY;

    void Start()
    {
        initialX = transform.position.x;
        initialY = transform.position.y;
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            float targetX = initialX + player.position.x;
            Vector3 targetPosition = new Vector3(targetX, initialY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
