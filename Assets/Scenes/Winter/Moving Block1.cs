using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MovingBlock1 : MonoBehaviour
{
    [SerializeField] float speed;
    Vector3 targetPos;

    Rigidbody2D rb;
    Vector3 moveDirection;

    [Header("MovePoint phải là object cha của các Points")]
    [SerializeField] public GameObject MovePoints;
    public Transform[] Points;
    int pointIndex, pointCount, direction = 1;

    public float waitDuration;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Points = new Transform[MovePoints.transform.childCount];
        for (int i = 0; i < MovePoints.gameObject.transform.childCount; i++)
        {
            Points[i] = MovePoints.transform.GetChild(i).gameObject.transform;
        }
    }

    void Start()
    {
        pointIndex = 1;
        pointCount = Points.Length;
        targetPos = Points[1].transform.position;
        DirectionCal();
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            NextPoint();
        }
    }

    void NextPoint()
    {
        transform.position = targetPos;
        moveDirection = Vector3.zero;
        if (pointIndex == pointCount - 1) { direction = -1; }
        if (pointIndex == 0) { direction = 1; }
        pointIndex += direction;
        targetPos = Points[pointIndex].transform.position;

        StartCoroutine(WaitNextPoint());
    }

    IEnumerator WaitNextPoint()
    {
        yield return new WaitForSeconds(waitDuration);
        DirectionCal();
    }

    void FixedUpdate()
    {
        rb.velocity = moveDirection * speed;
    }

    void DirectionCal()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    /*void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController.isOnPlatform = true;
            playerController.platformRb = rb;
            // Tăng trọng lực của player lên 20 lần để khi di chuyển dọc không bị nảy
            //playerRb.gravityScale = playerRb.gravityScale * 20;
            playerController.SetGravityMultiplier(10f);

           
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController.isOnPlatform = false;
            playerController.SetGravityMultiplier(1f);
            //playerRb.gravityScale = playerRb.gravityScale / 20; //giảm 20 lần khi rời platform
        }
    }*/
}
