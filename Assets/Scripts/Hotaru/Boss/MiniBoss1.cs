using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss1 : MonoBehaviour
{
    [Header("Boss Movement Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f; // Phạm vi phát hiện player
    public float attackRange = 2f; // Phạm vi tấn công
    public bool canMove = true;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float attackAnimationDuration = 1.5f;
    public int damage = 10;
    public GameObject hitbox;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;

    [Header("Animation Settings")]
    public string walkAnimationName = "MiniBoss_Walk";
    public string idleAnimationName = "MiniBoss_Idle";
    public string attackAnimationName = "MiniBoss_Attack";
    public Animator animator;
    public int exp = 50; // Số kinh nghiệm khi đánh bại boss

    [Header("Coin Settings")]
    public GameObject coinPrefab; // Prefab for the coin to drop
    public int amount = 50; // Amount of gold the boss drops

    private Transform player;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool facingRight = true;
    private float lastAttackTime = 0f;

    void Start()
    {
        // Tìm player trong scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure player has 'Player' tag.");
        }

        // Lấy Animator component
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning($"No Animator found on {gameObject.name}. Animation will not work.");
        }
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D enemyCollider = GetComponent<Collider2D>();
            if (playerCollider != null && enemyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, playerCollider);
            }
        }
        currentHealth = maxHealth; // Initialize health
    }

    void Update()
    {
        if (player != null && !isDead)
        {

            if (canMove && !isAttacking)
            {
                HandleMovement();
            }
            else if (!isMoving && !isAttacking)
            {
                PlayIdleAnimation();
            }
        }
    }

    void HandleMovement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (isMoving)
            {
                isMoving = false;
            }

            float directionX = player.position.x - transform.position.x;
            FlipSprite(directionX);

            HandleAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            float directionX = player.position.x - transform.position.x;
            if (Mathf.Abs(directionX) > 0.1f)
            {
                Vector2 moveDirection = new Vector2(Mathf.Sign(directionX), 0).normalized;
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

                FlipSprite(directionX);
                if (!isMoving)
                {
                    PlayWalkAnimation();
                    isMoving = true;
                }
            }
            else
            {
                if (isMoving)
                {
                    PlayIdleAnimation();
                    isMoving = false;
                }
            }
        }
        else
        {
            if (isMoving)
            {
                PlayIdleAnimation();
                isMoving = false;
            }
        }
    }

    void HandleAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown && !isDead)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        PlayAttackAnimation();

        yield return new WaitForSeconds(attackAnimationDuration * 0.3f);
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        yield return new WaitForSeconds(attackAnimationDuration * 0.7f);
        StopAttack();
    }

    void AttackPlayer()
    {
        if (hitbox != null)
        {
            hitbox.SetActive(true);
        }
    }

    void StopHitBoxAttack()
    {
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
    }
    void StopAttack()
    {
        isAttacking = false;
            if (!isMoving)
            {
                PlayIdleAnimation();
            }
    }

    void FlipSprite(float directionX)
    {
        if (directionX > 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void PlayIdleAnimation()
    {
        if (animator != null)
        {
            animator.Play(idleAnimationName);
        }
    }

    void PlayWalkAnimation()
    {
        if (animator != null)
        {
            animator.Play(walkAnimationName);
        }
    }

    void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.Play(attackAnimationName);
        }
    }
    public void SetCanMove(bool canMoveState)
    {
        canMove = canMoveState;
        if (!canMove && isMoving)
        {
            PlayIdleAnimation();
            isMoving = false;
        }
    }

    // Method để force reset tất cả states (backup emergency)
    public void ForceResetStates()
    {
        isAttacking = false;
        isMoving = false;
        PlayIdleAnimation();
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        isMoving = false;
        isAttacking = false;
        StopAllCoroutines();
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.GainExperience(exp);
            }
        }
        DestroyBoss();
    }

    // Animation Event Method: Destroy Boss (gọi từ animation event của death animation)
    public void DestroyBoss()
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

    // Vẽ gizmo để hiển thị phạm vi phát hiện và tấn công
    void OnDrawGizmosSelected()
    {
        // Vẽ phạm vi phát hiện player
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ phạm vi tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ hướng đang face
        Gizmos.color = facingRight ? Color.green : Color.blue;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position, direction * 2f);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerHealth = collision.GetComponent<Player>();
            if (playerHealth != null)
            {
                if (hitbox != null && hitbox.activeSelf) // During attack
                {
                    playerHealth.TakeDamage(damage); // Deal damage when hitbox is active
                    Debug.Log("Player hit by enemy attack.");
                }
            }
        }
    }
}
