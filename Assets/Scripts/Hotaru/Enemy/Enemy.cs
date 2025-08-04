using System.Collections;
using System.Collections.Generic;
using UnityEngine; // Ensure UnityEngine namespace is included for Player class reference

public class Enemy : MonoBehaviour
{
    [Header("Enemy Movement Settings")]
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
    public string walkAnimationName = "Enemy_Walk";
    public string idleAnimationName = "Enemy_Idle";
    public string attackAnimationName = "Enemy_Attack";
    public string deathAnimationName = "Enemy_Death";
    public Animator animator;

    [Header("Obstacle Jump Settings")]
    public Transform frontCheckPoint; // Điểm kiểm tra phía trước để raycast
    public float obstacleCheckDistance = 0.5f; // Khoảng cách raycast kiểm tra chướng ngại
    public float obstacleHeightCheck = 1f; // Chiều cao kiểm tra phía trên chướng ngại
    public Rigidbody2D rb; // Rigidbody2D của enemy
    public bool isGrounded = true; // Kiểm tra enemy có đang trên mặt đất không
    public LayerMask obstacleLayer; // Layer for obstacles
    public float jumpForce = 5f; // Force for jumping over obstacles

    [Header("Item Settings")]
    public GameObject coinPrefab;

    [Header("Sound Effects")]
    public AudioClip walkSound;
    public AudioClip hitSound;
    public AudioClip attackSound;
    public AudioSource audioSource;

    [Header("Experience Settings")]
    public int exp = 10; // Experience points granted to the player upon death

    [Header("Coin Settings")]
    public int amount = 10; // Amount of gold the enemy drops

    private Transform player;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool facingRight = true;
    private float lastAttackTime = 0f;
    private float patrolFlipTimer = 0f; // Timer for flipping during patrol
    private float patrolFlipInterval = 0f; // Random interval for flipping

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

        currentHealth = maxHealth;

        // Initialize patrol flip interval
        patrolFlipInterval = Random.Range(3f, 5f);
        patrolFlipTimer = patrolFlipInterval;

        // Ignore collision with the player
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D enemyCollider = GetComponent<Collider2D>();
            if (playerCollider != null && enemyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, playerCollider);
            }
        }
    }

    void Update()
    {
        if (!enabled)
        {
            Debug.Log("Enemy script is disabled.");
            return;
        }

        Debug.Log($"Enemy Update: enabled = {enabled}, canMove = {canMove}");

        if (player != null && !isDead)
        {
            if (currentHealth <= maxHealth * 0.2f) // Below 20% health
            {
                FleeFromPlayer();
                return; // Stop other behaviors when fleeing
            }

            if (canMove && !isAttacking)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer > detectionRange)
                {
                    Patrol(); // Use Patrol when the player is out of detection range
                }
                else
                {
                    HandleMovement();
                }
            }
            else if (!isMoving && !isAttacking)
            {
                PlayIdleAnimation();
            }
        }
        CheckObstacleAndJump(); // Check for obstacles and jump if needed
    }

    void HandleMovement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (isMoving)
            {
                isMoving = false;

                // Stop walk sound immediately
                if (audioSource != null && audioSource.clip == walkSound)
                {
                    audioSource.Stop();
                }
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

                    // Play walk sound immediately
                    if (audioSource != null && walkSound != null)
                    {
                        audioSource.clip = walkSound;
                        audioSource.loop = true;
                        audioSource.Play();
                    }
                }
            }
            else
            {
                if (isMoving)
                {
                    PlayIdleAnimation();
                    isMoving = false;

                    // Stop walk sound immediately
                    if (audioSource != null && audioSource.clip == walkSound)
                    {
                        audioSource.Stop();
                    }
                }
            }
        }
        else
        {
            if (isMoving)
            {
                PlayIdleAnimation();
                isMoving = false;

                // Stop walk sound immediately
                if (audioSource != null && audioSource.clip == walkSound)
                {
                    audioSource.Stop();
                }
            }

            // Handle patrol flip and obstacle detection
            patrolFlipTimer -= Time.deltaTime;
            if (patrolFlipTimer <= 0f)
            {
                FlipSprite(transform.localScale.x > 0 ? -1 : 1);
                patrolFlipInterval = Random.Range(3f, 5f);
                patrolFlipTimer = patrolFlipInterval;
            }
        }
    }
    void CheckObstacleAndJump()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Raycast ngang để kiểm tra chướng ngại
        RaycastHit2D wallHit = Physics2D.Raycast(frontCheckPoint.position, direction, obstacleCheckDistance, obstacleLayer);

        if (wallHit.collider != null && isGrounded)
        {
            // Kiểm tra xem phía trên có trống không để nhảy
            Vector2 upwardStart = wallHit.collider.bounds.center + new Vector3(0, wallHit.collider.bounds.extents.y + 0.1f, 0);
            RaycastHit2D ceilingHit = Physics2D.Raycast(upwardStart, Vector2.up, obstacleHeightCheck, obstacleLayer);

            if (ceilingHit.collider == null)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }

    void Patrol()
    {
        patrolFlipTimer -= Time.deltaTime;

        if (patrolFlipTimer <= 0f)
        {
            FlipSprite(transform.localScale.x > 0 ? -1 : 1); // Flip direction
            patrolFlipInterval = Random.Range(3f, 5f);
            patrolFlipTimer = patrolFlipInterval;
        }

        // Move in the current direction
        Vector2 moveDirection = facingRight ? Vector2.right : Vector2.left;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (!isMoving)
        {
            PlayWalkAnimation();
            isMoving = true;
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

        // Play attack sound immediately
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Đợi một chút để animation bắt đầu (khoảng 30% animation)
        yield return new WaitForSeconds(attackAnimationDuration * 0.3f);

        // Thực hiện tấn công ở giữa animation (kiểm tra xem player vẫn còn trong tầm đánh không)
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
        // Flip sprite dựa trên hướng di chuyển
        if (directionX > 0 && !facingRight)
        {
            // Di chuyển sang phải
            facingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < 0 && facingRight)
        {
            // Di chuyển sang trái
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

    void PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.Play(deathAnimationName);
        }
    }

    // Public method để bật/tắt di chuyển
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

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Không cho âm

        // Play hit sound immediately
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

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
        PlayDeathAnimation();

        // Grant experience to the player
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.GainExperience(exp);
            }
        }
    }

    public void DestroyEnemy()
    {
        // Destroy the enemy game object
        Destroy(gameObject);

        // Check if the coinPrefab is assigned
        if (coinPrefab != null)
        {
            // Instantiate a single coin at the enemy's position
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            // Set the coin's value if the Coin script is attached
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.value = amount; // Set the coin's value to the enemy's amount property
            }

            // Apply random force to the coin
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                float randomXForce = Random.Range(-2f, 2f); // Random horizontal force
                float upwardForce = Random.Range(3f, 5f);   // Random upward force
                coinRb.AddForce(new Vector2(randomXForce, upwardForce), ForceMode2D.Impulse);
            }
        }
        else
        {
            Debug.LogWarning("Coin prefab is not assigned. No coin will be dropped.");
        }
    }

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

        // Visualize raycasts for debugging
        Gizmos.color = Color.red;
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawRay(transform.position, dir * 0.5f); // Horizontal raycast
    }

    void FleeFromPlayer()
    {
        Vector2 fleeDirection = (transform.position - player.position).normalized;
        transform.Translate(fleeDirection * moveSpeed * Time.deltaTime);

        if (fleeDirection.x > 0 && !facingRight)
        {
            FlipSprite(1);
        }
        else if (fleeDirection.x < 0 && facingRight)
        {
            FlipSprite(-1);
        }

        Debug.Log("Enemy is fleeing from the player.");
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Reset grounded state when on ground
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Reset grounded state when leaving ground
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();
            if (playerScript != null)
            {
                if (hitbox != null && hitbox.activeSelf) // During attack
                {
                    playerScript.TakeDamage(damage); // Deal damage when hitbox is active
                    Debug.Log("Player hit by enemy attack.");
                }
                else // Collision with enemy body
                {
                    playerScript.TakeDamage(damage / 2); // Deal reduced damage for body collision
                    Debug.Log("Player collided with enemy.");
                }
            }
        }
    }
}
