using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

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

    // ==================== UNITY LIFECYCLE ====================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>(); // Lấy Collider của Player

        dashTime = dashDuration;
        isDashing = false;
        Health = MaxHealth;

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
    }

    void Update()
    {
        if (isDeath || isHit)
        {
            return; // Không xử lý nếu đang chết hoặc bị đánh
        }
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboStep = 0;
            }
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
        if (isSkill2 || isSkill1 || isHit) return; // Prevent movement if in skill or hurt state

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

    private void StopMoveSound()
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
        if (animator != null)
        {
            animator.Play("Die");
        }

        rb.velocity = Vector2.zero; // Dừng chuyển động khi chết
        Debug.Log("Player has died.");
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
        Time.timeScale = 0f;
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
        comboTimer = 0f;
    }

    public void Attack4()
    {
        if (!isAttacking) // Prevent repeated calls
        {
            animator.Play("Attack4");
            comboStep = 0; // Reset combo step
            isAttacking = true;
            attackTime = attackDuration;
            comboTimer = comboResetTime;
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

    public void EndAttack1HitBox()
    {
        if (Attack1HitBox != null)
            Attack1HitBox.SetActive(false);
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

    public void EndAttack2HitBox()
    {
        if (Attack2HitBox != null)
            Attack2HitBox.SetActive(false);
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

    public void EndAttack3HitBox()
    {
        if (Attack3HitBox != null)
            Attack3HitBox.SetActive(false);
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
    public void EndAttack4HitBox()
    {
        if (Attack4HitBox != null)
            Attack4HitBox.SetActive(false);
    }

    public void EndAttackAnimation()
    {
        isAttacking = false;
    }

    // ==================== SKILL SYSTEM ====================
    private void Skill1()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isSkill1 && !isGrounded && !isAttacking)
        {
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
    }

    private void Skill2()
    {
        if (Input.GetKeyDown(KeyCode.L) && !isSkill1 && !isSkill2 && !isAttacking && isGrounded)
        {
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
        if (canSpawnBullet && skill3BulletPrefab != null)
        {
            GameObject bullet = Instantiate(skill3BulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(skill1Damage); // Gán sát thương cho viên đạn
                bulletScript.SetDirection(transform.localScale.x); // Gán hướng bay của viên đạn
            }
        }
        else
        {
            Debug.LogError("Skill3 Bullet Prefab is not assigned in the Inspector.");
        }
    }
}

