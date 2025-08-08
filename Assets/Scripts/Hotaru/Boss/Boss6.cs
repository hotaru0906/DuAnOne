using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6 : MonoBehaviour
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
    public int damage = 10; // Damage value for attacks

    [Header("Skill 1 Settings")]
    public float skill1Cooldown = 10f; // Skill 1 mỗi 10 giây

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;

    [Header("Animation Settings")]
    public string walkAnimationName = "Boss3_Walk";
    public string idleAnimationName = "Boss3_Idle";
    public string attackAnimationName = "Boss3_Attack";
    public string deathAnimationName = "Boss3_Death";
    public string skill1AnimationName = "Boss3_Skill1";
    public string skill2AnimationName = "Boss3_Skill2";
    public Animator animator;
    public int exp = 100; // Số kinh nghiệm khi đánh bại boss

    [Header("Coin Settings")]
    public GameObject coinPrefab; // Prefab for the coin to drop
    public int amount = 50; // Amount of gold the boss drops
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public Cinemachine.CinemachineVirtualCamera playerCamera;
    [Header("Camera Shake")]
    public float cameraShakeAmplitude = 3f;
    public float cameraShakeDuration = 0.3f;
    public float cameraShakeFrequency = 2f;
    public GameObject playerObject, bossObject; // Reference to the player object
    public GameObject box1, box2;
    private Transform player;
    // ====== ICEY SPEAR SKILL 1 LOGIC ======
    [Header("Skill 1 - Icey Spear")]
    public GameObject iceySpearPrefab;
    public int maxSpears = 10;
    public float spearInterval = 1f;
    public float spearHeightOffset = 2f; // offset y trên đầu boss/player

    private int spearsSpawned = 0;
    private Coroutine spearCoroutine;

    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isUsingSkill1 = false;
    private bool facingRight = true;
    private float lastAttackTime = 0f;
    private float lastSkill1Time = 0f;

    void Start()
    {
        // Tìm player trong scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        GameObject bossObj = GameObject.FindGameObjectWithTag("Boss");
        if (bossObj != null)
        {
            // Nếu có, lấy transform của boss
            bossObject = bossObj;
        }
        else
        {
            Debug.LogWarning("Boss6: Player object not found! Make sure player has 'Player' tag.");
        }
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

        // Ignore collision giữa tất cả các boss với nhau (tag "Boss")
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
            CheckSkill1();

            // Logic di chuyển và tấn công bình thường (khi không dùng skill)
            if (canMove && !isAttacking && !isUsingSkill1)
            {
                HandleMovement();
            }
            else if (!isMoving && !isAttacking && !isUsingSkill1)
            {
                PlayIdleAnimation();
            }
        }
    }

    void CheckSkill1()
    {
        // Kiểm tra xem đã đủ thời gian để dùng Skill 1 chưa (KHÔNG được dùng khi đang attack hoặc dead)
        if (Time.time - lastSkill1Time >= skill1Cooldown && !isUsingSkill1 && !isAttacking && !isDead)
        {
            StartCoroutine(UseSkill1());

        }
    }

    IEnumerator UseSkill1()
    {
        Debug.Log("Boss3 is using Teleport skill (Skill 1)");
        isUsingSkill1 = true;


        // Dừng di chuyển
        if (isMoving)
        {
            isMoving = false;
        }

        PlaySkill1Animation();
        yield return null;

    }
    private IEnumerator ShakeCameraSmooth()
    {
        if (virtualCamera != null)
        {
            var perlin = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
            if (perlin != null)
            {
                float elapsed = 0f;
                while (elapsed < cameraShakeDuration)
                {
                    float strength = Mathf.Lerp(cameraShakeAmplitude, 0f, elapsed / cameraShakeDuration);
                    perlin.m_AmplitudeGain = strength;
                    perlin.m_FrequencyGain = cameraShakeFrequency;
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                perlin.m_AmplitudeGain = 0f;
                perlin.m_FrequencyGain = 0f;
            }
        }
    }


    public void SpawnIceySpear()
    {
        if (spearCoroutine == null)
        {
            Debug.Log("Boss6: Bắt đầu spawn Icey Spear!");
            spearCoroutine = StartCoroutine(SpawnSpearsRoutine());
            PlaySkill2Animation();
        }
    }

    private IEnumerator SpawnSpearsRoutine()
    {
        spearsSpawned = 0;
        while (spearsSpawned < maxSpears)
        {
            SpawnSingleSpear();
            StartCoroutine(ShakeCameraSmooth());
            Debug.Log($"Boss6: Spawned Icey Spear {spearsSpawned + 1}/{maxSpears}");
            spearsSpawned++;
            yield return new WaitForSeconds(spearInterval);
        }
        // Cooldown bắt đầu khi spear cuối cùng spawn ra
        lastSkill1Time = Time.time;
        spearCoroutine = null;
        Debug.Log("Boss6: Đã spawn xong tất cả Icey Spear!");
        PlayIdleAnimation();
        isUsingSkill1 = false;
        isAttacking = false;
        isMoving = false;
    }

    private void SpawnSingleSpear()
    {
        if (player == null) return;
        // Random vị trí x giữa boss và player
        float minX = Mathf.Min(transform.position.x, player.position.x);
        float maxX = Mathf.Max(transform.position.x, player.position.x);
        float randomX = Random.Range(minX, maxX);
        float y = transform.position.y + spearHeightOffset;
        Vector3 spawnPos = new Vector3(randomX, y, transform.position.z);
        if (iceySpearPrefab != null)
        {
            Instantiate(iceySpearPrefab, spawnPos, Quaternion.identity);
        }
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
        if (Time.time - lastAttackTime >= attackCooldown && !isUsingSkill1 && !isDead)
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
    void PlaySkill2Animation()
    {
        if (animator != null)
        {
            animator.Play(skill2AnimationName);
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

        Debug.Log("Boss3: Force reset all states to normal");
    }

    // Public method để nhận damage
    public void TakeDamage(float damage)
    {
        if (isDead) return; // Không nhận damage nếu đã chết

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Không cho âm

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
        isUsingSkill1 = false;

        // Dừng tất cả coroutines để tránh xung đột
        StopAllCoroutines();

        hitbox.SetActive(false);
        if (virtualCamera != null)
            virtualCamera.Follow.gameObject.SetActive(false);
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
            playerCamera.Follow = player.transform;
            playerCamera.LookAt = player.transform;
            playerCamera.Priority = 20; // Đảm bảo cao hơn các camera khác nếu có
        }
        if (box1 != null && box2 != null)
        {
            box1.SetActive(false);
            box2.SetActive(false);
        }

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
        Debug.Log("Boss3 destroyed!");
        Destroy(gameObject);

        if (BGMController.Instance != null)
        {
            BGMController.Instance.PlayBGMForScene("Forest");
        }

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.UnlockSkillByBoss("Boss3");
            GameManager.Instance.SaveSkillUnlocks(
                player.canUseSkill1,
                player.canUseSkill2,
                player.canShootBullet
            );
        }
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
            Player playerScript = collision.GetComponent<Player>();
            if (playerScript != null)
            {
                if (hitbox != null && hitbox.activeSelf) // During attack
                {
                    playerScript.TakeDamage(damage); // Deal damage when hitbox is active
                    Debug.Log("Player hit by enemy attack.");
                }
            }
        }
    }
}
