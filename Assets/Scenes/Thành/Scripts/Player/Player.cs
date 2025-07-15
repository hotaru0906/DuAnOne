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
    
    [Header("Ground Check Settings")]
    public bool isGrounded = false;
    
    [Header("HitBox References")]
    public GameObject dashHitBox;
    public GameObject Attack1HitBox;
    public GameObject Attack2HitBox;
    public GameObject Attack3HitBox;
    public GameObject skill1GroundHitBox;
    
    [Header("Component References")]
    public Rigidbody2D rb;
    public Animator animator;
    public BoxCollider2D boxCollider;
    
    // Private State Variables
    private int comboStep = 0;
    private float dashTime;
    private float comboTimer = 0f;
    private bool isDashing = false;
    private bool isAttacking = false;
    private bool isSkill1 = false;
    private bool isSkill2 = false;
    private bool isSkill1InAir = false;

    // ==================== UNITY LIFECYCLE ====================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dashTime = dashDuration;
        isDashing = false;
        Health = MaxHealth;
    }

    void Update()
    {
        // Cập nhật combo timer
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
    
    // ==================== BASIC FUNCTIONS ====================
    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
        if (isSkill2) return;
        if (isSkill1) return;
        
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal, 0f);
        rb.velocity = new Vector2(movement.x * Run, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.y) > 0.01f)
        {
            animator.Play("Jump");
        }
        else
        {
            if (Mathf.Abs(movement.x) > 0.1 && !isDashing)
            {
                animator.Play("Run");
            }
            else if (Mathf.Abs(movement.x) < 0.1f && !isDashing)
            {
                animator.Play("Idle");
            }
        }
        
        if (!isSkill1 && Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.001f)
        {
            isGrounded = false;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
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

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            animator.Play("Die");
        }
        else
        {
            animator.Play("Hurt");
        }
    }

    public void Die()
    {
        Debug.Log("Player has died.");
    }
    
    // ==================== DASH SYSTEM ====================
    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashing && isGrounded && !isSkill1 && !isSkill2 && !isAttacking)
        {
            isDashing = true;
            dashTime = dashDuration;
            float dashDirection = Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(dashDirection * dashForce, 0f);
            animator.Play("Dash");
        }
        
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            float dashDirection = Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(dashDirection * dashForce, 0f);
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
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
        animator.Play("Attack2");  // ✅ Khôi phục về Play thay vì SetTrigger
        comboStep = 2;
        isAttacking = true;
        attackTime = attackDuration;
        comboTimer = comboResetTime;
    }

    public void Attack3()
    {
        animator.Play("Attack3");  // ✅ Khôi phục về Play thay vì SetTrigger
        comboStep = 0;
        isAttacking = true;
        attackTime = attackDuration;
        comboTimer = 0f;
    }

    // Animation Events
    public void StartAttack1HitBox()
    {
        if (Attack1HitBox != null)
            Attack1HitBox.SetActive(true);
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
    }

    public void EndAttack3HitBox()
    {
        if (Attack3HitBox != null)
            Attack3HitBox.SetActive(false);
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
            animator.SetTrigger("Skill1");
            isSkill1 = true;
            isSkill1InAir = true;
            rb.velocity = new Vector2(0f, rb.velocity.y);
            rb.gravityScale = 2f; // Increase gravity scale for faster descent
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
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
    }

    public void StartSkill1GroundHitBox()
    {
        if (skill1GroundHitBox != null)
            skill1GroundHitBox.SetActive(true);
    }

    public void EndSkill1GroundHitBox()
    {
        if (skill1GroundHitBox != null)
            skill1GroundHitBox.SetActive(false);
    }

    public void EndSkill1()
    {
        isSkill1 = false;
        isSkill1InAir = false;
        rb.gravityScale = 1f;
        if (skill1GroundHitBox != null)
            skill1GroundHitBox.SetActive(false);
    }

    public void EndSkill2()
    {
        isSkill2 = false;
    }
}

