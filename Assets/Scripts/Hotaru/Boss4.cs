using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4 : MonoBehaviour
{
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
        
        // Khởi tạo máu
        currentHealth = maxHealth;
        Debug.Log($"Boss4 initialized with {currentHealth}/{maxHealth} health");
    }

    void Update()
    {
        if (player != null && !isDead)
        {
            // Debug states (tạm thời để debug)
            if (Time.frameCount % 60 == 0) // Debug mỗi giây
            {
                Debug.Log($"Boss4 States - Moving: {isMoving}, Attacking: {isAttacking}, UsingSkill1: {isUsingSkill1}, Health: {currentHealth}/{maxHealth}");
            }
            
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
        
        // Kiểm tra mốc 75%
        if (healthPercent <= 75f && !healthThresholdUsed[0])
        {
            healthThresholdUsed[0] = true;
            return true;
        }
        
        // Kiểm tra mốc 50%
        if (healthPercent <= 50f && !healthThresholdUsed[1])
        {
            healthThresholdUsed[1] = true;
            return true;
        }
        
        // Kiểm tra mốc 25%
        if (healthPercent <= 25f && !healthThresholdUsed[2])
        {
            healthThresholdUsed[2] = true;
            return true;
        }
        
        return false;
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
            StartCoroutine(DisableHitbox1AfterDelay(0.3f));
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
            StartCoroutine(DisableHitbox2AfterDelay(0.3f));
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
            StartCoroutine(DisableHitbox3AfterDelay(0.3f));
        }
    }
    
    IEnumerator DisableHitbox1AfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox1 != null)
        {
            hitbox1.SetActive(false);
        }
    }
    
    IEnumerator DisableHitbox2AfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox2 != null)
        {
            hitbox2.SetActive(false);
        }
    }
    
    IEnumerator DisableHitbox3AfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox3 != null)
        {
            hitbox3.SetActive(false);
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
    }
    
    // Animation Event Method: Destroy Boss (gọi từ animation event của death animation)
    public void DestroyBoss()
    {
        Debug.Log("Boss4 destroyed!");
        Destroy(gameObject);
    }

    // Utility methods để control hitbox riêng biệt (có thể dùng cho animation events)
    public void ActivateHitbox1()
    {
        if (hitbox1 != null)
        {
            hitbox1.SetActive(true);
            Debug.Log("Boss4 Hitbox1 activated");
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
    
    public void ActivateHitbox2()
    {
        if (hitbox2 != null)
        {
            hitbox2.SetActive(true);
            Debug.Log("Boss4 Hitbox2 activated");
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
    
    public void ActivateHitbox3()
    {
        if (hitbox3 != null)
        {
            hitbox3.SetActive(true);
            Debug.Log("Boss4 Hitbox3 activated");
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
}
