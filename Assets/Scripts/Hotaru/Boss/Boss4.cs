using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4 : MonoBehaviour
{
    [Header("Summon Settings")]
    public Transform summonPoint; // Vị trí dịch chuyển khi summon
    public GameObject portalType1;
    public GameObject portalType2;
    public GameObject portalType3;
    public string summonAnimationName = "Boss4_Summon";
    private GameObject currentSummonedBoss;
    public Rigidbody2D rb;
    private bool isSummoning = false;
    [Header("Boss Movement Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f; // Phạm vi phát hiện player
    public float attackRange = 2f; // Phạm vi tấn công
    public bool canMove = true;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float attackAnimationDuration = 1.5f; // Thời gian animation attack (điều chỉnh theo animation thực tế)
    public GameObject hitbox1; // Hitbox cho Attack Type 1
    public GameObject hitbox2; // Hitbox cho Attack Type 2
    public GameObject hitbox3; // Hitbox cho Attack Type 3
    public float dashSpeed = 10f; // Tốc độ lướt trong attack 2
    public float dashDistance = 3f; // Khoảng cách lướt tối đa
    public int damage1 = 10, damage2 = 15, damage3 = 20;

    [Header("Skill 1 Settings (Heal)")]
    public float skill1Cooldown = 10f; // Skill 1 mỗi 10 giây
    public float healAmount = 10f; // Hồi máu 10%
    public string healAnimationName = "Boss4_Heal"; // Animation hồi máu
    private bool[] healthThresholdUsed = new bool[3]; // Track các mốc máu đã dùng [75%, 50%, 25%]

    [Header("Skill 2 Settings (Dash)")]
    public float skill2Cooldown = 20f; // Skill 2 cooldown 20s
    public float dashAwayDistance = 5f; // Khoảng cách dash ra xa
    public string dashAnimationName = "Boss4_Dash"; // Animation dash
    private float lastSkill2Time = -20f; // Cho phép dùng ngay từ đầu

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;

    [Header("Animation Settings")]
    public string walkAnimationName = "Boss4_Walk";
    public string idleAnimationName = "Boss4_Idle";
    public string attackAnimationName = "Boss4_Attack";
    public string deathAnimationName = "Boss4_Death";
    public Animator animator;
    public int exp = 100; // Số kinh nghiệm khi đánh bại boss

    [Header("Coin Settings")]
    public GameObject coinPrefab; // Prefab for the coin to drop
    public int amount = 50; // Amount of gold the boss drops


    private Transform player;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isUsingSkill1 = false;
    private bool isUsingSkill2 = false;
    private bool isDashing = false; // Trạng thái đang dash
    private bool facingRight = true;
    private float lastAttackTime = 0f;
    private float lastSkill1Time = 0f;
    private int currentAttackType = 1; // Loại attack hiện tại (1, 2, hoặc 3)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        Collider2D myCollider = GetComponent<Collider2D>();
        GameObject[] allBosses = GameObject.FindGameObjectsWithTag("Boss");
        foreach (GameObject boss in allBosses)
        {
            if (boss != this.gameObject)
            {
                Collider2D bossCollider = boss.GetComponent<Collider2D>();
                if (myCollider != null && bossCollider != null)
                {
                    Physics2D.IgnoreCollision(myCollider, bossCollider);
                }
            }
        }
        currentHealth = maxHealth; // Initialize health
    }

    void Update()
    {
        if (player != null && !isDead)
        {
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Boss4 States - Moving: {isMoving}, Attacking: {isAttacking}, UsingSkill1: {isUsingSkill1}, Health: {currentHealth}/{maxHealth}, Summoning: {isSummoning}");
            }
            // Nếu đang summon thì không làm gì cả
            if (isSummoning)
                return;

            // Kiểm tra Skill 1 cooldown (ưu tiên cao nhất)
            CheckSkill1();

            // Logic di chuyển và tấn công bình thường (khi không dùng skill)
            if (canMove && !isAttacking && !isUsingSkill1 && !isUsingSkill2 && !isDashing)
            {
                HandleMovement();
            }
            else if (!isMoving && !isAttacking && !isUsingSkill1 && !isUsingSkill2 && !isDashing)
            {
                PlayIdleAnimation();
            }
        }
    }

    void CheckSkill1()
    {
        // Kiểm tra cooldown 10s
        bool cooldownReady = Time.time - lastSkill1Time >= skill1Cooldown;

        // Kiểm tra các mốc máu
        bool healthThresholdMet = CheckHealthThresholds();

        // Sử dụng skill nếu đủ điều kiện
        if ((cooldownReady || healthThresholdMet) && !isUsingSkill1 && !isAttacking && !isDead && !isUsingSkill2)
        {
            StartCoroutine(UseSkill1());
        }
    }

    bool CheckHealthThresholds()
    {
        float healthPercent = (currentHealth / maxHealth) * 100f;
        // 75% threshold
        if (healthPercent <= 75f && !healthThresholdUsed[0])
        {
            healthThresholdUsed[0] = true;
            StartCoroutine(DoSummonPhase(0));
            return false;
        }
        // 50% threshold
        if (healthPercent <= 50f && !healthThresholdUsed[1])
        {
            healthThresholdUsed[1] = true;
            StartCoroutine(DoSummonPhase(1));
            return false;
        }
        // 25% threshold
        if (healthPercent <= 25f && !healthThresholdUsed[2])
        {
            healthThresholdUsed[2] = true;
            StartCoroutine(DoSummonPhase(2));
            return false;
        }
        return false;
    }

    // Coroutine thực hiện summon boss phụ và portal
    IEnumerator DoSummonPhase(int phase)
    {
        // Đặt trạng thái Boss4
        isSummoning = true;
        canMove = false;
        isMoving = false;
        isAttacking = false;
        isUsingSkill1 = false;
        isUsingSkill2 = false;
        isDashing = false;

        Vector3 portalSpawnPosition = transform.position; // Vị trí spawn portal

        if (rb != null)
            rb.gravityScale = 0f; // Tắt trọng lực trong khi summon

        // Chạy animation summon
        if (animator != null)
        {
            animator.Play(summonAnimationName);
            animator.Update(0f);
            animator.speed = 0f;
        }

        // 1. Spawn portal tại vị trí boss đang đứng
        GameObject portal = null;
        switch (phase)
        {
            case 0:
                if (portalType1 != null)
                    portal = Instantiate(portalType1, portalSpawnPosition, Quaternion.identity);
                break;
            case 1:
                if (portalType2 != null)
                    portal = Instantiate(portalType2, portalSpawnPosition, Quaternion.identity);
                break;
            case 2:
                if (portalType3 != null)
                    portal = Instantiate(portalType3, portalSpawnPosition, Quaternion.identity);
                break;
        }

        // 2. Ngay lập tức dịch Boss4 về giữa màn hình
        if (summonPoint != null)
            transform.position = summonPoint.position;

        // 3. Nếu portal có script spawn boss phụ → chờ boss phụ chết
        if (portal != null)
        {
            PortalSpawn portalSpawn = portal.GetComponent<PortalSpawn>();
            if (portalSpawn != null)
            {
                portalSpawn.SpawnObject();

                // Tìm boss phụ gần nhất
                GameObject[] summonedBosses = GameObject.FindGameObjectsWithTag("Boss");
                float minDist = float.MaxValue;
                GameObject closest = null;
                foreach (var b in summonedBosses)
                {
                    if (b == this.gameObject) continue; // bỏ qua Boss4
                    float dist = Vector3.Distance(b.transform.position, portalSpawnPosition);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = b;
                    }
                }

                if (closest != null)
                    yield return StartCoroutine(WaitForSummonedBossDeath(closest));
            }
        }

        // 4. Hoàn tất summon, trả lại trạng thái bình thường
        isSummoning = false;
        canMove = true;
        if (animator != null)
            animator.speed = 1f;
        if (rb != null)
            rb.gravityScale = 1f;

        Debug.Log("Boss4: Summon phase ended, returning to normal state");
    }
    // Theo dõi boss phụ, khi chết thì boss 4 trở lại trạng thái bình thường
    IEnumerator WaitForSummonedBossDeath(GameObject summonedBoss)
    {
        // Chờ tới khi boss phụ biến mất hoặc bị xóa
        while (summonedBoss != null)
        {
            yield return null;
        }

        // Reset trạng thái
        isSummoning = false;
        canMove = true;

        if (animator != null)
            animator.speed = 1f;

        if (rb != null)
            rb.gravityScale = 1f;

        Debug.Log("Boss4: Summoned boss defeated, returning to normal state");
    }


    IEnumerator UseSkill1()
    {
        isUsingSkill1 = true;
        lastSkill1Time = Time.time;

        // Dừng di chuyển
        if (isMoving)
        {
            isMoving = false;
        }

        // Chạy animation heal
        PlayHealAnimation();

        // Animation event sẽ gọi HealBoss() khi cần
        // Animation event sẽ gọi EndSkill1() khi hoàn thành

        yield return null; // Coroutine sẽ được kết thúc bởi EndSkill1()
    }

    // Animation Event Method 1: Hồi máu
    public void HealBoss()
    {
        float healValue = maxHealth * (healAmount / 100f); // Tính 10% của max health
        currentHealth += healValue;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Không vượt quá max health

        Debug.Log($"Boss4 healed {healValue} HP! Current health: {currentHealth}/{maxHealth}");
    }

    // Animation Event Method 2: Kết thúc skill heal
    public void EndSkill1()
    {
        isUsingSkill1 = false;
        Debug.Log("Boss4 Heal skill ended, returning to normal state");

        // Reset các state về bình thường
        isAttacking = false;
        isMoving = false;

        // Quay về idle
        PlayIdleAnimation();
    }

    void HandleMovement()
    {
        // Tính khoảng cách đến player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Kiểm tra xem player có trong phạm vi tấn công không
        if (distanceToPlayer <= attackRange)
        {
            // Player trong vùng tấn công - dừng di chuyển và tấn công
            if (isMoving)
            {
                isMoving = false;
            }

            // Flip sprite hướng về player
            float directionX = player.position.x - transform.position.x;
            FlipSprite(directionX);

            // Thực hiện tấn công
            HandleAttack();
        }
        // Kiểm tra xem player có trong phạm vi phát hiện không
        else if (distanceToPlayer <= detectionRange)
        {
            // Tính hướng di chuyển chỉ theo trục X
            float directionX = player.position.x - transform.position.x;

            // Kiểm tra xem có cần di chuyển không (tránh rung lắc khi quá gần)
            if (Mathf.Abs(directionX) > 0.1f)
            {
                // Di chuyển theo hướng player
                Vector2 moveDirection = new Vector2(Mathf.Sign(directionX), 0).normalized;
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

                // Flip sprite theo hướng di chuyển
                FlipSprite(directionX);

                // Phát animation walk
                if (!isMoving)
                {
                    PlayWalkAnimation();
                    isMoving = true;
                }
            }
            else
            {
                // Đứng yên khi đã đủ gần
                if (isMoving)
                {
                    PlayIdleAnimation();
                    isMoving = false;
                }
            }
        }
        else
        {
            // Player ngoài phạm vi, đứng yên
            if (isMoving)
            {
                PlayIdleAnimation();
                isMoving = false;
            }
        }
    }

    void HandleAttack()
    {
        // Kiểm tra cooldown tấn công (chỉ tấn công khi không dùng skill và không dead)
        if (Time.time - lastAttackTime >= attackCooldown && !isUsingSkill1 && !isUsingSkill2 && !isDead && !isDashing)
        {
            // Chọn loại attack ngẫu nhiên (1, 2, 3)
            currentAttackType = Random.Range(1, 4);
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Phát animation tấn công (chung cho cả 3 loại)
        PlayAttackAnimation();

        Debug.Log($"Boss4 performing attack type {currentAttackType}");

        // Đợi animation hoàn thành
        yield return new WaitForSeconds(attackAnimationDuration);

        // Kết thúc attack
        StopAttack();
    }

    // Animation Event Method: Attack Type 1 - Mở hitbox1
    public void AttackType1()
    {
        Debug.Log("Boss4 Attack Type 1: Basic hitbox attack");
        if (hitbox1 != null)
        {
            hitbox1.SetActive(true);
        }
    }

    // Animation Event Method: Attack Type 2 - Dash to player (Part 1)
    public void AttackType2_StartDash()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Chỉ dash nếu không ở gần player
            if (distanceToPlayer > attackRange)
            {
                StartCoroutine(DashToPlayer());
                Debug.Log("Boss4 Attack Type 2: Dashing to player");
            }
            else
            {
                // Nếu đã ở gần, chỉ mở hitbox
                AttackType2_ActivateHitbox();
                Debug.Log("Boss4 Attack Type 2: Already close, activating hitbox");
            }
        }
    }

    // Animation Event Method: Attack Type 2 - Activate hitbox2 (Part 2)
    public void AttackType2_ActivateHitbox()
    {
        Debug.Log("Boss4 Attack Type 2: Activating hitbox after dash");
        if (hitbox2 != null)
        {
            hitbox2.SetActive(true);
        }
    }

    IEnumerator DashToPlayer()
    {
        isDashing = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.position;

        // Giới hạn khoảng cách dash
        Vector3 direction = (targetPosition - startPosition).normalized;
        float actualDistance = Mathf.Min(Vector3.Distance(startPosition, targetPosition), dashDistance);
        targetPosition = startPosition + direction * actualDistance;

        float dashTime = actualDistance / dashSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < dashTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / dashTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        transform.position = targetPosition;
        isDashing = false;

        // Flip hướng về player sau khi dash
        if (player != null)
        {
            float directionX = player.position.x - transform.position.x;
            FlipSprite(directionX);
        }
    }

    // Animation Event Method: Attack Type 3 - Mở hitbox3
    public void AttackType3()
    {
        Debug.Log("Boss4 Attack Type 3: Special hitbox attack");
        if (hitbox3 != null)
        {
            hitbox3.SetActive(true);
        }
    }


    void StopHitBoxAttack()
    {
        // Tắt tất cả hitbox
        if (hitbox1 != null)
        {
            hitbox1.SetActive(false);
        }
        if (hitbox2 != null)
        {
            hitbox2.SetActive(false);
        }
        if (hitbox3 != null)
        {
            hitbox3.SetActive(false);
        }
    }
    void StopAttack()
    {
        isAttacking = false;

        // Chỉ chuyển về idle nếu không đang dùng skill
        if (!isUsingSkill1)
        {
            if (!isMoving)
            {
                PlayIdleAnimation();
            }
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

    void PlaySkill1Animation()
    {
        if (animator != null)
        {
            animator.Play(healAnimationName);
        }
    }

    void PlayHealAnimation()
    {
        if (animator != null)
        {
            animator.Play(healAnimationName);
        }
    }

    void PlayDashAnimation()
    {
        if (animator != null)
        {
            animator.Play(dashAnimationName);
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
        isUsingSkill1 = false;
        isMoving = false;

        PlayIdleAnimation();

        Debug.Log("Boss4: Force reset all states to normal");
    }

    // Public method để nhận damage
    public void TakeDamage(float damage)
    {
        if (isDead) return; // Không nhận damage nếu đã chết
        if (isSummoning) return; // Không nhận damage khi đang summon

        // Kiểm tra skill2 dash cooldown
        bool canDash = Time.time - lastSkill2Time >= skill2Cooldown;

        if (canDash && !isUsingSkill1 && !isUsingSkill2 && !isDashing)
        {
            // Sử dụng skill2 để dash ra xa thay vì nhận damage
            StartCoroutine(UseSkill2DashAway());
            Debug.Log("Boss4 used Skill2 Dash to avoid damage!");
            return; // Không nhận damage
        }

        // Nhận damage bình thường nếu skill2 trong cooldown
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Không cho âm

        Debug.Log($"Boss4 took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator UseSkill2DashAway()
    {
        isUsingSkill2 = true;
        isDashing = true;
        lastSkill2Time = Time.time;

        // Dừng di chuyển
        if (isMoving)
        {
            isMoving = false;
        }

        // Chạy animation dash
        PlayDashAnimation();

        // Tính vị trí dash ra xa player
        Vector3 dashPosition = CalculateDashAwayPosition();

        // Thực hiện dash
        Vector3 startPosition = transform.position;
        float dashTime = dashAwayDistance / dashSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < dashTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / dashTime;
            transform.position = Vector3.Lerp(startPosition, dashPosition, progress);
            yield return null;
        }

        transform.position = dashPosition;

        // Flip hướng về player sau khi dash
        if (player != null)
        {
            float directionX = player.position.x - transform.position.x;
            FlipSprite(directionX);
        }

        isUsingSkill2 = false;
        isDashing = false;

        Debug.Log("Boss4 Skill2 Dash completed, returning to normal state");
        PlayIdleAnimation();
    }

    Vector3 CalculateDashAwayPosition()
    {
        if (player == null) return transform.position;

        // Tính hướng ra xa player
        Vector3 direction = (transform.position - player.position).normalized;

        // Tính vị trí dash (chỉ theo trục X để tránh bay lên trời)
        Vector3 dashPosition = transform.position + new Vector3(direction.x * dashAwayDistance, 0, 0);

        return dashPosition;
    }

    void Die()
    {
        isDead = true;
        isMoving = false;
        isAttacking = false;
        isUsingSkill1 = false;
        isUsingSkill2 = false;
        isDashing = false;

        // Dừng tất cả coroutines để tránh xung đột
        StopAllCoroutines();

        Debug.Log("Boss4 has died!");

        // Chạy animation death
        PlayDeathAnimation();
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.GainExperience(exp);
            }
        }
    }

    // Animation Event Method: Destroy Boss (gọi từ animation event của death animation)
    public void DestroyBoss()
    {
        Debug.Log("Boss4 destroyed!");
        Destroy(gameObject);

        // Check if the coinPrefab is assigned
        if (coinPrefab != null)
        {
            // Instantiate a single coin at the boss's position
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            // Set the coin's value if the Coin script is attached
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.value = amount; // Set the coin's value to the boss's amount property
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

    public void DeactivateHitbox1()
    {
        if (hitbox1 != null)
        {
            hitbox1.SetActive(false);
            Debug.Log("Boss4 Hitbox1 deactivated");
        }
    }

    public void DeactivateHitbox2()
    {
        if (hitbox2 != null)
        {
            hitbox2.SetActive(false);
            Debug.Log("Boss4 Hitbox2 deactivated");
        }
    }

    public void DeactivateHitbox3()
    {
        if (hitbox3 != null)
        {
            hitbox3.SetActive(false);
            Debug.Log("Boss4 Hitbox3 deactivated");
        }
    }

    // Method để tắt tất cả hitbox cùng lúc
    public void DeactivateAllHitboxes()
    {
        DeactivateHitbox1();
        DeactivateHitbox2();
        DeactivateHitbox3();
        Debug.Log("Boss4 All hitboxes deactivated");
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
                if (hitbox1 != null && hitbox1.activeSelf) // During attack
                {
                    playerHealth.TakeDamage(damage1); // Deal damage when hitbox is active
                    Debug.Log("Player hit by enemy attack.");
                }
                else if (hitbox2 != null && hitbox2.activeSelf) // During attack
                {
                    playerHealth.TakeDamage(damage2); // Deal damage when hitbox is active
                    Debug.Log("Player hit by enemy attack.");
                }
                else if (hitbox3 != null && hitbox3.activeSelf) // During attack
                {
                    playerHealth.TakeDamage(damage3); // Deal damage when hitbox is active
                    Debug.Log("Player hit by enemy attack.");
                }
            }
        }
    }
}
