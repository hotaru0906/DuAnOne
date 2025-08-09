using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Health Settings")]
    public int Health = 100;
    public int MaxHealth = 100;

    [Header("Damage Settings")]
    public int attack1Damage = 10;
    public int attack2Damage = 15;
    public int attack3Damage = 20;
    public int skill1Damage = 20;

    [Header("Movement Settings")]
    public float Run = 10f;
    public float jumpForce = 10f;

    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDuration = 0.15f;

    [Header("Attack Settings")]
    public float attackTime = 0f;
    public float attackDuration = 0.5f;
    public float comboResetTime = 0.5f;
    public bool canSpawnBullet = false;

    [Header("Ground Check Settings")]
    public bool isGrounded = false;

    [Header("HitBox References")]
    public GameObject Attack1HitBox;
    public GameObject Attack2HitBox;
    public GameObject Attack3HitBox;
    public GameObject Attack4HitBox;
    public GameObject skill1GroundHitBox;
    [Header("Stats Settings")]
    public int str = 0; // Strength: increases attack damage
    public int vit = 0; // Vitality: increases max health
    public int spd = 0; // Speed: increases movement speed
    public int intStat = 0; // Intelligence: increases skill and bullet damage
    public int crt = 0; // Critical Rate: increases critical hit chance

    [Header("Component References")]
    public Rigidbody2D rb;
    public Animator animator;
    public BoxCollider2D boxCollider;
    public GameObject skill3BulletPrefab;
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip attack1, attack2, attack3, attack4, skill1, skill2, fall, dashSound, moveSound, hitSound;
    public AudioClip jumpSound, coinSound;
    private bool isPlayingMoveSound = false;

    [Header("Bullet Settings")]
    public Transform bulletSpawnPoint;

    [Header("UI References")]
    public GameObject inventoryUI;

    [Header("UI Sliders")]
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider expSlider;

    [Header("UI Text References")]
    public TMPro.TMP_Text levelText; // Reference to the UI text for displaying the player's level

    [Header("Mana Settings")]
    public int MaxMana = 100;
    public int Mana = 100;
    public float manaRegenRate = 0.01f; // 1% of total mana per second

    [Header("Experience Settings")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 20;
    public int statPoints = 0; // Points available for stat allocation

    [Header("Skill Availability Flags")]
    public bool canUseSkill1 = true;
    public bool canUseSkill2 = true;
    public bool canShootBullet = true;

    [Header("Currency Settings")]
    public int gold = 0; // Player's gold amount

    [Header("Potion Settings")]
    public int healthPotionCount = 0; // Number of health potions
    public int manaPotionCount = 0; // Number of mana potions
    public int teleportPotionCount = 0; // Number of teleport potions

    [Header("Potion UI References")]
    public TMPro.TMP_Text healthPotionText;
    public TMPro.TMP_Text manaPotionText;
    public TMPro.TMP_Text teleportPotionText;

    // Private State Variables
    private int comboStep = 0;
    private float dashTime;
    private float comboTimer = 0f;
    private bool isDashing = false;
    private bool isAttacking = false;
    private bool isSkill1 = false;
    private bool isSkill2 = false;
    private bool isSkill1InAir = false;
    private bool isHit = false;
    private bool isDeath = false;
    public bool isInvincible = false; // Flag for invincibility
    private Collider2D playerCollider;
    private bool isInventoryOpen = false;
    private bool isPerformingAction = false; // Biến để kiểm tra nếu Player đang thực hiện hành động

    // ==================== UNITY LIFECYCLE ====================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>(); // Lấy Collider của Player

        dashTime = dashDuration;
        isDashing = false;
        Health = MaxHealth;

        // Initialize sliders if assigned
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = Health;
        }
        if (manaSlider != null)
        {
            manaSlider.maxValue = MaxMana;
            manaSlider.value = Mana;
        }
        if (expSlider != null)
        {
            expSlider.maxValue = 100; // Example max value for experience
            expSlider.value = 0; // Example starting value for experience
        }
        MaxHealth = GetMaxHealth();
        Health = Mathf.Min(Health, MaxHealth);

        MaxMana = GetMaxMana();
        Mana = Mathf.Min(Mana, MaxMana);

        Run = GetRunSpeed();
        dashForce = GetDashForce();
        skill1Damage = GetSkill1Damage();

        // Bỏ qua va chạm với Enemy
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, enemyCollider);
            }
        }
        if (GameManager.Instance != null)
        {
            gold = GameManager.Instance.GetPlayerGold();
            var s = GameManager.Instance.GetPlayerStats();
            str = s.str; vit = s.vit; spd = s.spd; intStat = s.intStat; crt = s.crt;
            var l = GameManager.Instance.GetPlayerLevel();
            level = l.level;
            statPoints = l.points;
            healthPotionCount = GameManager.Instance.healthPotion;
            manaPotionCount = GameManager.Instance.manaPotion;
            teleportPotionCount = GameManager.Instance.recallPotion;
            level = GameManager.Instance.playerLevel;
            statPoints = GameManager.Instance.playerStatPoints;
            currentExp = GameManager.Instance.currentExp;

            expToNextLevel = CalculateExpForLevel(level);

            if (expSlider != null)
            {
                expSlider.maxValue = expToNextLevel;
                expSlider.value = currentExp;
            }

            UpdateLevelUI();
        }
        if (GameManager.Instance != null)
        {
            canUseSkill1 = GameManager.Instance.unlockedSkill1;
            canUseSkill2 = GameManager.Instance.unlockedSkill2;
            canShootBullet = GameManager.Instance.unlockedBullet;
        }

        // Start mana and health regeneration coroutine
        StartCoroutine(RegenerateStats());

        UpdateLevelUI();
        UpdatePotionUI();
        UpdateManaUI();
        UpdateHealthUI();
    }

    void Update()
    {
        if (isDeath || isHit || isPerformingAction)
        {
            return; // Không xử lý nếu đang chết, bị đánh, hoặc đang thực hiện hành động
        }

        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboStep = 0;
            }
        }

        // Inventory control
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // Health recovery (Key 1)
        if (Input.GetKeyDown(KeyCode.Alpha1) && healthPotionCount > 0 && !isAttacking && !isDashing && !isSkill1 && !isSkill2 && isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !isPerformingAction)
        {
            StartCoroutine(PerformAction("Health", RecoverHealth));
        }

        // Mana recovery (Key 2)
        if (Input.GetKeyDown(KeyCode.Alpha2) && manaPotionCount > 0 && !isAttacking && !isDashing && !isSkill1 && !isSkill2 && isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !isPerformingAction)
        {
            StartCoroutine(PerformAction("Mana", RecoverMana));
        }

        // Recall (Key 3)
        if (Input.GetKeyDown(KeyCode.Alpha3) && teleportPotionCount > 0 && !isAttacking && !isDashing && !isSkill1 && !isSkill2 && isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !isPerformingAction)
        {
            StartCoroutine(PerformAction("Recall", Recall));
        }

        // Movement control
        if (!isDashing && !isAttacking && !isSkill1 && !isSkill2)
        {
            Control();
        }
        else if (isAttacking && !isSkill1 && !isSkill2)
        {
            ControlDuringAttack();
        }
        else if (isSkill1 || isSkill2)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // Attack input
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!isAttacking && !isDashing && !isSkill1 && !isSkill2)
            {
                if (comboStep == 0)
                {
                    Attack1();
                }
                else if (comboStep == 1)
                {
                    Attack2();
                }
                else if (comboStep == 2)
                {
                    Attack3();
                }
                else if (comboStep == 3)
                {
                    Attack4();
                }
            }
        }

        // Skill logic
        if (!isDashing && !isAttacking && !isSkill1)
        {
            Skill1();
        }
        if (!isDashing && !isAttacking && !isSkill1 && !isSkill2)
        {
            Skill2();
        }

        if (!isSkill1 && !isSkill2)
        {
            Dash();
        }

        // Update sliders dynamically
        if (healthSlider != null)
        {
            healthSlider.value = Health;
        }
        if (manaSlider != null)
        {
            // Update mana value here if you have a mana system
        }
        if (expSlider != null)
        {
            // Update experience value here if you have an experience system
        }

        // Debug stats khi nhấn phím P
        if (Input.GetKeyDown(KeyCode.P))
        {
            int attack1 = 10 + str;
            int attack2 = 15 + str;
            int attack3 = 20 + str;
            int maxHealth = 100 + vit * 5;
            Debug.Log($"==== PLAYER STATS ====");
            Debug.Log($"STR: {str} - Attack1: {attack1}, Attack2: {attack2}, Attack3: {attack3}");
            Debug.Log($"VIT: {vit} - MaxHealth: {maxHealth}, Health: {Health}");
            Debug.Log($"SPD: {spd} - Run: {Run}");
            Debug.Log($"INT: {intStat} - MaxMana: {MaxMana}, Mana: {Mana}, Skill1Damage: {skill1Damage}");
            Debug.Log($"CRT: {crt} - CritRate: {crt * 0.2f}%");
            Debug.Log($"Level: {level}, StatPoints: {statPoints}");
        }

    }

    // ==================== COLLISION DETECTION ====================
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isSkill1InAir)
            {
                isSkill1InAir = false;
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1_Ground")) // Prevent repeated calls
                {
                    animator.Play("Skill1_Ground");
                }
                rb.gravityScale = 1f;
            }
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            audioSource.PlayOneShot(coinSound);
        }
        if (collision.gameObject.CompareTag("DeadEnd"))
        {
            Die();
        }
    }

    // ==================== BASIC FUNCTIONS ====================
    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        // Do not stop the move sound when flipping
    }

    // ==================== MOVEMENT & CONTROL ====================
    public void ControlDuringAttack()
    {
        if (isSkill2) return;
        if (isSkill1) return;

        if (!isGrounded)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(moveHorizontal, 0f);
            rb.velocity = new Vector2(movement.x * Run, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    public void Control()
    {
        if (isSkill2 || isSkill1 || isHit) return;

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal, 0f);
        rb.velocity = new Vector2(movement.x * Run, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.y) > 0.01f)
        {
            animator.Play("Jump");
            StopMoveSound();
        }
        else
        {
            if (Mathf.Abs(movement.x) > 0.1 && !isDashing)
            {
                animator.Play("Run");
                PlayMoveSound();
            }
            else if (Mathf.Abs(movement.x) < 0.1f && !isDashing)
            {
                animator.Play("Idle");
                StopMoveSound();
            }
        }

        if (!isSkill1 && Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.001f)
        {
            isGrounded = false;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            StopMoveSound();

            // Play jump sound
            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }

        if (moveHorizontal > 0 && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
        EndAttackAnimation();
    }

    private void PlayMoveSound()
    {
        if (audioSource != null && moveSound != null && !isPlayingMoveSound && !isDashing)
        {
            audioSource.clip = moveSound;
            audioSource.loop = true;
            audioSource.Play();
            isPlayingMoveSound = true;
        }
    }

    public void StopMoveSound()
    {
        if (audioSource != null && isPlayingMoveSound)
        {
            audioSource.Stop();
            isPlayingMoveSound = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isHit || isInvincible) return; // Prevent taking damage while already hit or invincible
        isHit = true; // Set hit state to true

        // Reset các trạng thái khác để tránh xung đột
        isAttacking = false;
        isSkill1 = false;
        isSkill2 = false;
        rb.velocity = Vector2.zero; // Stop player movement

        StartCoroutine(ActivateInvincibility(0.75f)); // Activate invincibility for 0.75 seconds
        Debug.Log($"Player took {damage} damage. Current Health: {Health}");
        Health -= damage;

        // Update health slider
        if (healthSlider != null)
        {
            healthSlider.value = Health;
        }

        // Play Hurt animation only if it's not already playing
        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt") && !animator.IsInTransition(0))
        {
            Debug.Log("Playing Hurt animation.");
            animator.Play("Hurt");

            // Play hit sound
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }

        if (Health <= 0)
        {
            Debug.Log("Health is 0 or less. Calling Die method.");
            Die();
        }
    }

    public void Die()
    {
        isDeath = true;
        StopMoveSound();
        if (animator != null)
        {
            animator.Play("Die");
        }

        rb.velocity = Vector2.zero; // Dừng chuyển động khi chết
        // Lưu tất cả chỉ số vào GameManager
        GameManager.Instance.playerGold = gold;
        GameManager.Instance.playerStr = str;
        GameManager.Instance.playerVit = vit;
        GameManager.Instance.playerSpd = spd;
        GameManager.Instance.playerInt = intStat;
        GameManager.Instance.playerCrt = crt;
        GameManager.Instance.playerLevel = level;
        GameManager.Instance.playerStatPoints = statPoints;
        GameManager.Instance.healthPotion = healthPotionCount;
        GameManager.Instance.manaPotion = manaPotionCount;
        GameManager.Instance.recallPotion = teleportPotionCount;
        GameManager.Instance.currentExp = currentExp;
        GameManager.Instance.unlockedSkill1 = canUseSkill1;
        GameManager.Instance.unlockedSkill2 = canUseSkill2;
        GameManager.Instance.unlockedBullet = canShootBullet;
        GameManager.Instance.SavePlayerData();
        Debug.Log("Player has died and stats saved.");
    }
    public void EndHit()
    {
        isHit = false;
        isSkill1 = false; // Reset skill1 state
        isSkill2 = false; // Reset skill2 state
        isAttacking = false; // Reset attacking state
        rb.velocity = new Vector2(Run * Input.GetAxisRaw("Horizontal"), rb.velocity.y); // Restore movement speed
    }
    public void EndDeath()
    {
        isDeath = false;
        //Time.timeScale = 0f;
        SceneManager.LoadScene("DeathMenu"); // Load Game Over scene
        // code UI
    }

    // ==================== DASH SYSTEM ====================
    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashing && isGrounded && !isSkill1 && !isSkill2 && !isAttacking && !isInvincible)
        {
            isDashing = true;
            dashTime = dashDuration;
            StartCoroutine(ActivateInvincibility(0.75f)); // Activate invincibility for 0.5 seconds during dash
            float dashDirection = Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(dashDirection * dashForce, rb.velocity.y); // Giữ nguyên vận tốc y
            animator.Play("Dash");
            StopMoveSound();
        }

        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            float dashDirection = Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(dashDirection * dashForce, rb.velocity.y); // Giữ nguyên vận tốc y
            if (dashTime <= 0)
            {
                isDashing = false;
                rb.velocity = Vector2.zero; // Stop movement after dash ends
            }
        }
    }
    void DashSound()
    {
        if (audioSource != null && dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }
    }
    public void EndDash()
    {
        isDashing = false;
    }

    private IEnumerator ActivateInvincibility(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    // ==================== ATTACK SYSTEM ====================
    public void Attack1()
    {
        if (!isAttacking) // Prevent repeated calls
        {
            animator.Play("Attack1");
            comboStep = 1;
            isAttacking = true;
            attackTime = attackDuration;
            comboTimer = comboResetTime;
        }
    }

    public void Attack2()
    {
        animator.Play("Attack2");
        comboStep = 2;
        isAttacking = true;
        attackTime = attackDuration;
        comboTimer = comboResetTime;
    }

    public void Attack3()
    {
        animator.Play("Attack3");
        comboStep = 3;
        isAttacking = true;
        attackTime = attackDuration;
        comboTimer = comboResetTime;
    }

    public void Attack4()
    {
        if (!isAttacking) // Prevent repeated calls
        {
            animator.Play("Attack4");
            comboStep = 0; // Reset combo step
            isAttacking = true;
            attackTime = attackDuration;
            comboTimer = 0f;
        }
    }

    // Animation Events
    public void StartAttack1HitBox()
    {
        if (Attack1HitBox != null)
            Attack1HitBox.SetActive(true);
        if (audioSource != null && attack1 != null)
        {
            audioSource.PlayOneShot(attack1);
        }
    }
    public void StartAttack2HitBox()
    {
        if (Attack2HitBox != null)
            Attack2HitBox.SetActive(true);
        if (audioSource != null && attack2 != null)
        {
            audioSource.PlayOneShot(attack2);
        }
    }
    public void StartAttack3HitBox()
    {
        if (Attack3HitBox != null)
            Attack3HitBox.SetActive(true);
        if (audioSource != null && attack3 != null)
        {
            audioSource.PlayOneShot(attack3);
        }
    }
    public void StartAttack4HitBox()
    {
        if (Attack4HitBox != null)
            Attack4HitBox.SetActive(true);
        if (audioSource != null && attack4 != null)
        {
            audioSource.PlayOneShot(attack4);
        }
    }

    public void EndAttackAnimation()
    {
        isAttacking = false;
        if (Attack1HitBox != null)
            Attack1HitBox.SetActive(false);
        if (Attack2HitBox != null)
            Attack2HitBox.SetActive(false);
        if (Attack3HitBox != null)
            Attack3HitBox.SetActive(false);
        if (Attack4HitBox != null)
            Attack4HitBox.SetActive(false);
    }

    // ==================== SKILL SYSTEM ====================
    private void Skill1()
    {
        if (Input.GetKeyDown(KeyCode.K) && canUseSkill1 && !isSkill1 && !isGrounded && !isAttacking && Mana >= 50)
        {
            Mana -= 50;
            if (manaSlider != null)
            {
                manaSlider.value = Mana;
            }

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1")) // Prevent repeated calls
            {
                animator.Play("Skill1");
                isSkill1 = true;
                isSkill1InAir = true;
                isInvincible = true; // Activate invincibility during skill
                rb.velocity = new Vector2(0f, rb.velocity.y);
                rb.gravityScale = 2f;
                if (audioSource != null && skill1 != null)
                {
                    audioSource.PlayOneShot(skill1);
                }
            }
        }
        else if (Mana < 50)
        {
            Debug.Log("Not enough mana for Skill1.");
        }
    }

    private void Skill2()
    {
        if (Input.GetKeyDown(KeyCode.L) && canUseSkill2 && !isSkill1 && !isSkill2 && !isAttacking && isGrounded && Mana >= 25)
        {
            Mana -= 25;
            if (manaSlider != null)
            {
                manaSlider.value = Mana;
            }

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Skill2")) // Prevent repeated calls
            {
                animator.Play("Skill2");
                isSkill2 = true;
                isInvincible = true; // Activate invincibility during skill
                rb.velocity = new Vector2(0f, rb.velocity.y);
                StopMoveSound();
                if (audioSource != null && skill2 != null)
                {
                    audioSource.PlayOneShot(skill2);
                }
            }
        }
        else if (Mana < 25)
        {
            Debug.Log("Not enough mana for Skill2.");
        }
    }

    public void SpawnBullet()
    {
        if (canSpawnBullet && skill3BulletPrefab != null)
        {
            GameObject bulletObject = Instantiate(skill3BulletPrefab.gameObject, transform.position, Quaternion.identity);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.SetDamage(50); // Set high damage for the bullet
            }
            Destroy(bulletObject, 3f); // Destroy the bullet's GameObject after 3 seconds
        }
        else
        {
            Debug.LogError("Skill3 Bullet Prefab is not assigned in the Inspector.");
        }
    }

    public void StartSkill1GroundHitBox()
    {
        if (skill1GroundHitBox != null)
        {
            skill1GroundHitBox.SetActive(true);
            Debug.Log("Skill1 Ground HitBox activated");
            if (audioSource != null && fall != null)
            {
                audioSource.PlayOneShot(fall);
            }
        }
        else
        {
            Debug.LogError("Skill1 Ground HitBox is not assigned in the Inspector.");
        }
    }

    public void EndSkill1GroundHitBox()
    {
        if (skill1GroundHitBox != null)
        {
            skill1GroundHitBox.SetActive(false);
            Debug.Log("Skill1 Ground HitBox deactivated");
        }
    }

    public void EndSkill1()
    {
        isSkill1 = false;
        isSkill1InAir = false;
        isInvincible = false;
        rb.gravityScale = 1f;
        if (skill1GroundHitBox != null)
            skill1GroundHitBox.SetActive(false);
    }

    public void EndSkill2()
    {
        isSkill2 = false;
        isInvincible = false; // Deactivate invincibility after skill ends
    }

    public void SpawnBulletForAttack4()
    {
        if (!canShootBullet) return;
        if (canSpawnBullet && skill3BulletPrefab != null)
        {
            GameObject bullet = Instantiate(skill3BulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                int damage = GetSkill1Damage();
                if (IsCriticalHit())
                {
                    damage = Mathf.CeilToInt(damage * 1.5f);
                    Debug.Log("Critical Bullet Hit!");
                }

                bulletScript.SetDamage(damage);
                bulletScript.SetDirection(transform.localScale.x);
            }
        }
        else
        {
            Debug.LogError("Skill3 Bullet Prefab is not assigned in the Inspector.");
        }
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(isInventoryOpen);
        }
        if (isInventoryOpen)
        {
            Debug.Log("Inventory opened.");
        }
        else
        {
            Debug.Log("Inventory closed.");
        }
    }

    // Hồi máu và mana mỗi giây
    private IEnumerator RegenerateStats()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            // Hồi 2% máu tối đa mỗi giây
            int healAmount = Mathf.CeilToInt(MaxHealth * 0.02f);
            if (Health < MaxHealth)
            {
                Health = Mathf.Min(Health + healAmount, MaxHealth);
                if (healthSlider != null)
                    healthSlider.value = Health;
            }
            // Hồi mana như cũ
            int manaAmount = Mathf.CeilToInt(MaxMana * manaRegenRate);
            if (Mana < MaxMana)
            {
                Mana = Mathf.Min(Mana + manaAmount, MaxMana);
                if (manaSlider != null)
                    manaSlider.value = Mana;
            }
        }
    }


    public void GainExperience(int exp)
    {
        currentExp += exp;
        Debug.Log($"Gained {exp} EXP. Current EXP: {currentExp}/{expToNextLevel}");

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        // Update exp slider dynamically
        if (expSlider != null)
        {
            expSlider.value = (float)currentExp / expToNextLevel * expSlider.maxValue;
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"Level: {level}";
        }
    }

    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        expToNextLevel += 20; // Increase EXP required for the next level

        statPoints += 3; // Award 3 stat points
        if (statsController != null)
        {
            statsController.points = statPoints; // Synchronize with StatsController
        }

        Debug.Log($"Leveled up to Level {level}! Stat points available: {statPoints}");

        // Update exp slider dynamically
        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }

        UpdateLevelUI(); // Update the level on the UI

        // Update points UI if available
        if (levelText != null)
        {
            levelText.text = $"Level: {level}";
        }

        if (manaSlider != null)
        {
            manaSlider.value = Mana;
        }
    }
    private int CalculateExpForLevel(int lvl)
    {
        return 20 + (lvl - 1) * 20; // Cấp 1: 20, cấp 2: 40, cấp 3: 60,...
    }

    public void UseStatPoint(string stat)
    {
        if (statPoints <= 0)
        {
            Debug.Log("No stat points available to allocate.");
            return;
        }

        switch (stat)
        {
            case "str":
                str++;
                break;
            case "vit":
                vit++;
                break;
            case "spd":
                spd++;
                break;
            case "int":
                intStat++;
                break;
            case "crt":
                crt++;
                break;
            default:
                Debug.LogError("Invalid stat name: " + stat);
                return;
        }

        statPoints--;

        ApplyStats(); // ⚠️ Cập nhật toàn bộ chỉ số sau khi tăng

        if (statsController != null)
        {
            statsController.points = statPoints;
        }

        Debug.Log($"Allocated 1 point to {stat}. Remaining points: {statPoints}");
    }

    // ==================== NEW STATS CONTROLLER REFERENCE ====================
    [Header("Stats Controller Reference")]
    public StatsController statsController; // Reference to StatsController

    private IEnumerator PerformAction(string animationName, System.Action action)
    {
        isPerformingAction = true;
        rb.velocity = Vector2.zero; // Stop movement

        if (animator != null && animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName);
        }

        action?.Invoke(); // Perform the action (e.g., heal, recover mana, etc.)

        // Wait until the animation ends
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isPerformingAction = false;
    }

    public void RecoverHealth()
    {
        Health = Mathf.Min(Health + Mathf.CeilToInt(MaxHealth * 0.25f), MaxHealth);
        healthPotionCount--;
        UpdatePotionUI(); // Update UI after using a potion
        if (healthSlider != null)
        {
            healthSlider.value = Health;
        }
        Debug.Log("Player recovered 25% health.");
    }

    public void RecoverMana()
    {
        Mana = Mathf.Min(Mana + Mathf.CeilToInt(MaxMana * 0.25f), MaxMana);
        if (manaSlider != null)
            manaPotionCount--;
        UpdatePotionUI();
        {
            manaSlider.value = Mana;
        }
        Debug.Log("Player recovered 25% mana.");
    }

    public void Recall()
    {
        teleportPotionCount--;
        UpdatePotionUI();
        // Cập nhật dữ liệu từ Player sang GameManager trước khi lưu
        GameManager.Instance.playerGold = gold;
        GameManager.Instance.playerStr = str;
        GameManager.Instance.playerVit = vit;
        GameManager.Instance.playerSpd = spd;
        GameManager.Instance.playerInt = intStat;
        GameManager.Instance.playerCrt = crt;
        GameManager.Instance.playerLevel = level;
        GameManager.Instance.playerStatPoints = statPoints;
        GameManager.Instance.healthPotion = healthPotionCount;
        GameManager.Instance.manaPotion = manaPotionCount;
        GameManager.Instance.recallPotion = teleportPotionCount;
        GameManager.Instance.currentExp = currentExp;
        GameManager.Instance.unlockedSkill1 = canUseSkill1;
        GameManager.Instance.unlockedSkill2 = canUseSkill2;
        GameManager.Instance.unlockedBullet = canShootBullet;
        GameManager.Instance.SavePlayerData(); // Đảm bảo lưu dữ liệu khi dùng teleport potion
    }
    public void TPVillage()
    {
        //GameManager.Instance.SavePlayerData(); // Save player data before teleporting
        SceneManager.LoadScene("Village");
        Debug.Log("Teleporting to Village...");
    }

    public void UpdatePotionUI()
    {
        if (healthPotionText != null)
            healthPotionText.text = healthPotionCount.ToString();

        if (manaPotionText != null)
            manaPotionText.text = manaPotionCount.ToString();

        if (teleportPotionText != null)
            teleportPotionText.text = teleportPotionCount.ToString();
    }
    public void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = Health;
        }
    }

    public void UpdateManaUI()
    {
        if (manaSlider != null)
        {
            manaSlider.maxValue = MaxMana;
            manaSlider.value = Mana;
        }
    }

    public void UnlockSkillByBoss(string bossName)
    {
        switch (bossName)
        {
            case "Boss1":
                canUseSkill1 = true;
                Debug.Log("Unlocked Skill 1 after defeating Boss1");
                break;
            case "Boss2":
                canUseSkill2 = true;
                Debug.Log("Unlocked Skill 2 after defeating Boss2");
                break;
            case "Boss3":
                canShootBullet = true;
                Debug.Log("Unlocked Bullet after defeating Boss3");
                break;
            default:
                Debug.LogWarning("Unknown boss name: " + bossName);
                break;
        }
    }
    // ====== STAT-BASED DYNAMIC VALUES ======
    public int GetAttack1Damage() => 10 + str;
    public int GetAttack2Damage() => 15 + str;
    public int GetAttack3Damage() => 20 + str;
    public int GetSkill1Damage() => 20 + intStat;

    public float GetRunSpeed() => 2f + spd * 0.1f;
    public float GetDashForce() => 2f + spd * 0.1f;
    public int GetMaxHealth() => 100 + vit * 5;
    public int GetMaxMana() => 100 + intStat * 2;

    public bool IsCriticalHit()
    {
        float critRate = crt * 0.2f; // 0.2% per point
        return Random.Range(0f, 100f) < critRate;
    }
    public void ApplyStats()
    {
        int oldMaxHealth = MaxHealth;
        int oldMaxMana = MaxMana;

        MaxHealth = GetMaxHealth();
        MaxMana = GetMaxMana();
        Run = GetRunSpeed();
        dashForce = GetDashForce();
        skill1Damage = GetSkill1Damage();

        // Tăng máu hiện tại theo phần được mở rộng
        int healthGain = MaxHealth - oldMaxHealth;
        Health += healthGain;
        Health = Mathf.Min(Health, MaxHealth);

        // Tăng mana hiện tại theo phần được mở rộng
        int manaGain = MaxMana - oldMaxMana;
        Mana += manaGain;
        Mana = Mathf.Min(Mana, MaxMana);

        UpdateHealthUI();
        UpdateManaUI();
    }
}