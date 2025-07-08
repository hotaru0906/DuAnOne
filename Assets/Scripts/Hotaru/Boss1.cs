using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    [Header("Boss Movement Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f; // Phạm vi phát hiện player
    public float attackRange = 2f; // Phạm vi tấn công
    public bool canMove = true;
    
    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float attackAnimationDuration = 1.5f; // Thời gian animation attack (điều chỉnh theo animation thực tế)
    public GameObject hitbox;
    
    [Header("Skill 1 Settings")]
    public float skill1Cooldown = 10f; // Skill 1 mỗi 10 giây
    public GameObject skill1Prefab; // GameObject sẽ spawn trên đầu player
    public float skill1HeightOffset = 3f; // Độ cao spawn trên đầu player
    
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;
    
    [Header("Animation Settings")]
    public string walkAnimationName = "Boss1_Walk";
    public string idleAnimationName = "Boss1_Idle";
    public string attackAnimationName = "Boss1_Attack";
    public string skill1AnimationName = "Boss1_Skill";
    public string hitAnimationName = "Boss1_Hit";
    public string deathAnimationName = "Boss1_Death";
    public Animator animator;
    
    private Transform player;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isUsingSkill1 = false;
    private bool isHit = false;
    private bool facingRight = true;
    private float lastAttackTime = 0f;
    private float lastSkill1Time = 0f;
    
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
        Debug.Log($"Boss1 initialized with {currentHealth}/{maxHealth} health");
    }

    void Update()
    {
        if (player != null && !isDead)
        {
            // Debug states (tạm thời để debug)
            if (Time.frameCount % 60 == 0) // Debug mỗi giây
            {
                Debug.Log($"Boss1 States - Moving: {isMoving}, Attacking: {isAttacking}, UsingSkill1: {isUsingSkill1}, Health: {currentHealth}/{maxHealth}");
            }
            
            // Kiểm tra Skill 1 cooldown (ưu tiên cao nhất)
            CheckSkill1();
            
            // Logic di chuyển và tấn công bình thường (khi không dùng skill và không bị hit)
            if (canMove && !isAttacking && !isUsingSkill1 && !isHit)
            {
                HandleMovement();
            }
            else if (!isMoving && !isAttacking && !isUsingSkill1 && !isHit)
            {
                PlayIdleAnimation();
            }
        }
    }
    
    void CheckSkill1()
    {
        // Kiểm tra xem đã đủ thời gian để dùng Skill 1 chưa (KHÔNG được dùng khi đang attack, hit hoặc dead)
        if (Time.time - lastSkill1Time >= skill1Cooldown && !isUsingSkill1 && !isAttacking && !isHit && !isDead)
        {
            StartCoroutine(UseSkill1());
        }
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
        
        // Flip hướng về player
        if (player != null)
        {
            float directionX = player.position.x - transform.position.x;
            FlipSprite(directionX);
        }
        
        // Chạy animation skill 1
        PlaySkill1Animation();
        
        // Chờ animation event để triệu hồi object (sẽ được gọi từ animation event)
        // Animation event sẽ gọi SummonSkill1Object()
        
        // Chờ animation hoàn thành (sẽ được kết thúc bởi animation event)
        // Animation event sẽ gọi EndSkill1()
        
        yield return null; // Coroutine sẽ được kết thúc bởi EndSkill1()
    }
    
    // Animation Event Method 1: Triệu hồi object
    public void SummonSkill1Object()
    {
        if (player != null && skill1Prefab != null)
        {
            // Tạo position trên đầu player - giữ nguyên trục X của player
            Vector3 spawnPosition = new Vector3(
                player.position.x + 0.6f,                    // X: Chính xác vị trí X của player
                player.position.y + skill1HeightOffset, // Y: Vị trí Y của player + offset
                player.position.z                     // Z: Giữ nguyên Z của player
            );
            
            // Spawn object
            GameObject spawnedObject = Instantiate(skill1Prefab, spawnPosition, Quaternion.identity);
            
            Debug.Log($"Boss1 Skill 1: Summoned object above player at {spawnPosition}");
        }
        else
        {
            Debug.LogWarning("Cannot summon Skill 1 object: Player or Skill1Prefab is null");
        }
    }
    
    // Animation Event Method 2: Kết thúc skill
    public void EndSkill1()
    {
        isUsingSkill1 = false;
        Debug.Log("Boss1 Skill 1 ended, returning to normal state");
        
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
        // Kiểm tra cooldown tấn công (chỉ tấn công khi không dùng skill, không hit và không dead)
        if (Time.time - lastAttackTime >= attackCooldown && !isUsingSkill1 && !isHit && !isDead)
        {
            StartCoroutine(PerformAttack());
        }
    }
    
    IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // Phát animation tấn công
        PlayAttackAnimation();
        
        // Đợi một chút để animation bắt đầu (khoảng 30% animation)
        yield return new WaitForSeconds(attackAnimationDuration * 0.3f);
        
        // Thực hiện tấn công ở giữa animation (kiểm tra xem player vẫn còn trong tầm đánh không)
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        
        // Đợi animation tấn công hoàn thành (70% còn lại)
        yield return new WaitForSeconds(attackAnimationDuration * 0.7f);
        
        // Kết thúc attack
        StopAttack();
    }
    
    void AttackPlayer()
    {
        Debug.Log("Boss1 attacked Player! Player took damage!");
        if (hitbox != null)
        {
            hitbox.SetActive(true);
            // Tắt hitbox sau một khoảng thời gian ngắn
            StartCoroutine(DisableHitboxAfterDelay(0.2f));
        }
        // Có thể thêm logic damage ở đây
        // player.GetComponent<PlayerHealth>().TakeDamage(damage);
    }
    
    IEnumerator DisableHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox != null)
        {
            hitbox.SetActive(false);
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
            animator.Play(skill1AnimationName);
        }
    }
    
    void PlayHitAnimationClip()
    {
        if (animator != null)
        {
            animator.Play(hitAnimationName);
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
        
        Debug.Log("Boss1: Force reset all states to normal");
    }

    // Public method để nhận damage
    public void TakeDamage(float damage)
    {
        if (isDead) return; // Không nhận damage nếu đã chết
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Không cho âm
        
        Debug.Log($"Boss1 took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(PlayHitAnimation());
        }
    }
    
    IEnumerator PlayHitAnimation()
    {
        isHit = true;
        
        // Dừng các action hiện tại
        if (isMoving)
        {
            isMoving = false;
        }
        
        // Chạy animation hit
        PlayHitAnimationClip();
        
        // Đợi animation hit hoàn thành (điều chỉnh thời gian theo animation thực tế)
        yield return new WaitForSeconds(0.5f);
        
        isHit = false;
        
        // Quay về idle nếu không có action khác
        if (!isAttacking && !isUsingSkill1)
        {
            PlayIdleAnimation();
        }
    }
    
    void Die()
    {
        isDead = true;
        isMoving = false;
        isAttacking = false;
        isUsingSkill1 = false;
        isHit = false;
        
        Debug.Log("Boss1 has died!");
        
        // Chạy animation death
        PlayDeathAnimation();
    }
    
    // Animation Event Method: Destroy Boss (gọi từ animation event của death animation)
    public void DestroyBoss()
    {
        Debug.Log("Boss1 destroyed!");
        Destroy(gameObject);
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
