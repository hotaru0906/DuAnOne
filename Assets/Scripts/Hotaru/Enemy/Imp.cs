using System.Collections;
using System.Collections.Generic;
using UnityEngine; // Ensure UnityEngine namespace is included for Player class reference

public class Imp : MonoBehaviour
{
    [Header("Flying Enemy Settings")]
    public bool isFlyingEnemy = false; // Đặt true cho enemy bay
    public bool canMoveWhileAttacking = true; // True: vừa di chuyển vừa bắn, False: đứng yên khi bắn
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;
    public float shootInterval = 3f; // Nếu không có animation attack
    private float lastShootTime = 0f;

    [Header("Enemy Movement Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f; // Phạm vi phát hiện player
    public float attackRange = 2f; // Phạm vi tấn công
    public bool canMove = true;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float attackAnimationDuration = 1.5f;
    public int damage = 10;


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

    [Header("Shooting Point")]
    public Transform shootPoint; // Vị trí spawn đạn (có thể gán trong Inspector)

    private Transform player;
    private Transform boss;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool facingRight = true;
    private float patrolFlipTimer = 0f; // Timer for flipping during patrol
    private float patrolFlipInterval = 0f; // Random interval for flipping
    private Vector2 shootDirection; // Hướng bắn được lưu tại thời điểm bắt đầu animation attack

    private bool isDiving = false;
    private bool isWaitingAfterAttack = false;

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
        GameObject bossObj = GameObject.FindGameObjectWithTag("Boss");
        if (bossObj != null)
        {
            boss = bossObj.transform;
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

        if (boss != null)
        {
            Collider2D bossCollider = boss.GetComponent<Collider2D>();
            Collider2D enemyCollider = GetComponent<Collider2D>();
            if (bossCollider != null && enemyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, bossCollider);
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

        if (isDead) return;

        // Nếu đang dive attack thì không bay lên, không patrol
        if (isDiving)
        {
            return;
        }

        // Nếu không có player thì patrol
        if (player == null)
        {
            Patrol();
            return;
        }

        // Nếu máu thấp thì bỏ chạy
        if (currentHealth <= maxHealth * 0.2f)
        {
            FleeFromPlayer();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Nếu đang chờ sau attack thì không làm gì
        if (isWaitingAfterAttack)
        {
            return;
        }

        float targetY = player.position.y + 3f;
        float yDiff = Mathf.Abs(transform.position.y - targetY);
        // Luôn bay lên phía trên player nếu chưa đúng vị trí
        if (yDiff > 0.1f)
        {
            Vector3 targetPos = new Vector3(transform.position.x, targetY, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            // Khi đang bay lên thì idle
            if (isMoving)
            {
                PlayIdleAnimation();
                isMoving = false;
            }
        }
        else
        {
            // Đã ở đúng vị trí phía trên player
            if (!isAttacking && !isWaitingAfterAttack)
            {
                Patrol();
            }
            // Nếu đủ điều kiện tấn công thì bắt đầu dive
            if (!isAttacking && !isWaitingAfterAttack && distanceToPlayer <= detectionRange)
            {
                if (Mathf.Abs(transform.position.x - player.position.x) < detectionRange)
                {
                    if (Time.time - lastShootTime >= attackCooldown)
                    {
                        StartCoroutine(DiveAttack());
                    }
                }
            }
        }
    }

    IEnumerator DiveAttack()
    {
        isDiving = true;
        // Tính vị trí dive: x lệch player 2f, y = player
        float targetX = player.position.x + (transform.position.x < player.position.x ? -2f : 2f);
        Vector3 targetPos = new Vector3(targetX, player.position.y, transform.position.z);
        // Flip hướng trước khi dive
        float directionX = targetX - transform.position.x;
        FlipSprite(directionX);
        // Bay xuống vị trí tấn công
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * 2f * Time.deltaTime);
            yield return null;
        }
        // Đã tới vị trí, chỉ chạy animation attack (không tự shoot)
        isAttacking = true;
        PlayAttackAnimation();
        yield return new WaitForSeconds(attackAnimationDuration);
        isAttacking = false;
        lastShootTime = Time.time;
        // Đứng yên 3 giây (Idle)
        PlayIdleAnimation();
        isWaitingAfterAttack = true;
        yield return new WaitForSeconds(3f);
        isWaitingAfterAttack = false;
        isDiving = false;
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

    // Bắn đạn về phía player
    public void Shoot()
    {
        if (bulletPrefab == null) return;
        // Dùng shootPoint nếu có, không thì dùng vị trí enemy
        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Đòn bắn theo hướng enemy đang hướng mặt về (không phụ thuộc player)
            Vector2 dir = facingRight ? Vector2.right : Vector2.left;
            rb.velocity = dir * bulletSpeed;
            // Flip sprite viên đạn theo hướng bay
            if (Mathf.Abs(dir.x) > 0.01f)
            {
                Vector3 scale = bullet.transform.localScale;
                scale.x = dir.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                bullet.transform.localScale = scale;
            }
        }
    }

    // Kiểu có animation attack: đứng yên khi bắn
    IEnumerator PerformShootAttack()
    {
        isAttacking = true;
        // Lưu hướng bắn tại thời điểm bắt đầu animation attack
        if (player != null)
        {
            shootDirection = (player.position - transform.position).normalized;
        }
        else
        {
            shootDirection = facingRight ? Vector2.right : Vector2.left;
        }
        PlayAttackAnimation();
        yield return new WaitForSeconds(attackAnimationDuration);
        lastShootTime = Time.time; // Đặt cooldown sau khi bắn xong
        isAttacking = false;
        if (!isMoving)
        {
            PlayIdleAnimation();
        }
    }
    void OnBecameInvisible()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
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

    // Hàm kiểm tra nếu animation có event Shoot (nếu bạn dùng event)
    bool HasShootEventInAnimation()
    {
        // Đơn giản trả về false nếu không dùng event, hoặc kiểm tra Animator nếu muốn
        return false;
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
}
