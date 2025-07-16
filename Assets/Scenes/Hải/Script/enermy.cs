using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public Transform spawnPoint;

    public float detectionRadius = 5f;
    public float moveSpeed = 2f;
    public float patrolOffset = 2f;
    public float attackRange = 1f;

    public Animator animator;
    private bool isChasing = false;
    private bool returningToSpawn = false;
    private bool patrolling = false;
    private bool isDead = false;
    private float timer = 0f;

    private Vector3 patrolPointA;
    private Vector3 patrolPointB;
    private Vector3 currentTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        patrolPointA = spawnPoint.position + Vector3.left * patrolOffset;
        patrolPointB = spawnPoint.position + Vector3.right * patrolOffset;
        currentTarget = patrolPointB;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Phát hiện player → đuổi
        if (distanceToPlayer < detectionRadius)
        {
            isChasing = true;
            returningToSpawn = false;
            patrolling = false;
            timer = 0f;
            animator.SetBool("Run", true);
            MoveTo(player.position);

            // Nếu gần → tấn công
            if (distanceToPlayer < attackRange)
            {
                animator.SetTrigger("Attack");
            }
        }
        else if (isChasing)
        {
            timer += Time.deltaTime;
            if (timer >= 5f)
            {
                isChasing = false;
                returningToSpawn = true;
                timer = 0f;
            }
            else
            {
                animator.SetBool("Run", true);
                MoveTo(player.position);
            }
        }
        else if (returningToSpawn)
        {
            animator.SetBool("Run", true);
            transform.position = spawnPoint.position;
            returningToSpawn = false;
            patrolling = true;
            currentTarget = patrolPointB;
        }
        else if (patrolling)
        {
            animator.SetBool("Run", true);
            //  Debug: Kiểm tra tọa độ đích enemy đang hướng tới
            Debug.Log("Patrolling to: " + currentTarget);

            MoveTo(currentTarget);
            if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
            {
                currentTarget = (currentTarget == patrolPointA) ? patrolPointB : patrolPointA;
            }
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }

    void MoveTo(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Lật hướng enemy
        if (dir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("Die", true);
        // Tùy chọn: Destroy(gameObject, 2f);
    }
}