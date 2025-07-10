using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int comboStep = 0;
    public float dashForce = 10f;
    public float dashDuration = 0.15f;
    public float groundCheckRadius = 0.2f;
    private float dashTime;
    public float attackTime = 0f;
    public float attackDuration = 0.5f;
    public float Walk = 5f;
    public float jumpForce = 10f;
    public float Run = 10f;
    private float comboTimer = 0f;
    public float comboResetTime = 0.5f;
    private bool isDashing = false;
    private bool isAttacking = false;
    private bool isSkill1 = false;
    private bool isSkill2 = false;
    public bool isGrounded = false;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject dashHitBox;
    public GameObject skill1HitBox;
    public Rigidbody2D rb;
    public Animator animator;
    public BoxCollider2D boxCollider;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dashTime = dashDuration; // Khởi tạo thời gian dash
        isDashing = false; // Bắt đầu không trong trạng thái dash

    }


    void Update()
    {
        // Reset combo nếu không nhấn tiếp sau khi kết thúc Attack1 hoặc Attack2
        if (!isAttacking && (comboStep == 1 || comboStep == 2))
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboStep = 0;
            }
        }

        // Không kiểm tra grounded mỗi frame nữa, sẽ kiểm tra bằng OnCollision


        if (!isDashing && !isAttacking)
        {
            Control();
        }
        else if (isAttacking)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        else if (isSkill1)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            // Chỉ cho phép thực hiện attack tiếp theo khi không đang attack và không đang dash
            if (!isAttacking && !isDashing)
            {
                if (comboStep == 0)
                {
                    Attack1();
                    Debug.Log("Attack1");
                }
                else if (comboStep == 1)
                {
                    Attack2();
                    Debug.Log("Attack2");
                }
                else if (comboStep == 2)
                {
                    Attack3();
                    Debug.Log("Attack3");
                }
            }
        }
        // Ưu tiên kiểm tra trạng thái skill1 trước khi cho phép gọi Skill1
        if (!isDashing && !isAttacking && !isSkill1)
        {
            Skill1();
        }
        if(!isDashing && !isAttacking && !isSkill1 && !isSkill2)
        {
            Skill2();
        }

        Dash();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
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
    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }
    public void Control()
    {
        if(isSkill2) return;   
        if (isSkill1) return;
        // Di chuyển nhân vật
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        Vector2 movement = new Vector2(moveHorizontal, 0f);
        rb.velocity = new Vector2(movement.x * Walk, rb.velocity.y);

        // Kiểm tra nếu nhân vật đang trên không thì play Jump
        if (Mathf.Abs(rb.velocity.y) > 0.01f)
        {
            animator.Play("Jump");
        }
        else
        {
            if (Mathf.Abs(movement.x) > 0.1 && !isDashing)
            {
                // Nếu đang di chuyển thì play Run
                animator.Play("Run");
            }
            else if (Mathf.Abs(movement.x) < 0.1f && !isDashing)
            {
                // Nếu không di chuyển thì play Idle
                animator.Play("Idle");
            }

        }// Nhảy
        if (!isSkill1 && Input.GetKey(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.001f)
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
        // Dash timer
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }

    }
    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashing && isGrounded)
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
            // Giữ nguyên vận tốc dash trong suốt thời gian dash
            float dashDirection = Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(dashDirection * dashForce, 0f);
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
    }

    private void StartDashHitBox()
    {
        boxCollider.enabled = false;
        if (dashHitBox != null)
            dashHitBox.SetActive(true);
    }
    private void EndDashHitBox()
    {
        boxCollider.enabled = true;
        if (dashHitBox != null)
            dashHitBox.SetActive(false);
    }
    private void JumpSkill1()
    {
        // if (rb.mass != 2.5f)
        //     rb.mass = 2.5f;

    }
    private void EndSkill1()
    {
        if (rb.mass != 1f)
            rb.mass = 1f;
        rb.WakeUp();
        Debug.Log("Skill 1 ended, mass = " + rb.mass);
        if (isGrounded)
        {
            rb.velocity = new Vector2(0f, 0f); // Dừng chuyển động khi kết thúc skill
        }
        if (skill1HitBox != null)
            skill1HitBox.SetActive(true);
    }
    private void EndSkill1HitBox()
    {
        if (skill1HitBox != null)
            skill1HitBox.SetActive(false);
    }
    public void Attack1()
    {
        animator.SetTrigger("Attack1");
        comboStep = 1;
        isAttacking = true;
        comboTimer = comboResetTime;
        StartCoroutine(WaitForAttack1End());
    }

    private IEnumerator WaitForAttack1End()
    {
        // Đợi cho đến khi animation Attack1 kết thúc
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack1") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }
        // Khi animation Attack1 kết thúc, cho phép combo tiếp
        isAttacking = false;
        float wait = comboResetTime;
        while (comboStep == 1 && wait > 0)
        {
            wait -= Time.deltaTime;
            yield return null;
        }
        if (comboStep == 1) comboStep = 0;
        // Không reset comboStep ở đây, giữ nguyên để nhận Attack2
    }

    public void Attack2()
    {
        animator.ResetTrigger("Attack1");
        animator.SetTrigger("Attack2");
        comboStep = 2;
        isAttacking = true;
        comboTimer = comboResetTime;
        StartCoroutine(WaitForAttack2End());
    }

    private IEnumerator WaitForAttack2End()
    {
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack2") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }
        float wait = comboResetTime;
        isAttacking = false;
        while (comboStep == 2 && wait > 0)
        {
            wait -= Time.deltaTime;
            yield return null;
        }
        if (comboStep == 2) comboStep = 0;
    }

    public void Attack3()
    {
        animator.ResetTrigger("Attack2");
        animator.SetTrigger("Attack3");
        comboStep = 3;
        isAttacking = true;
        comboTimer = comboResetTime;
        StartCoroutine(WaitForAttack3End());
    }

    private IEnumerator WaitForAttack3End()
    {
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack3") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }
        isAttacking = false;
        // Reset comboStep về 0 sau khi kết thúc Attack3
        comboStep = 0;
    }
    private void Skill1()
    {
        // Chỉ cho phép dùng skill1 khi đang ở trên không và chưa active skill1
        if (Input.GetKeyDown(KeyCode.K) && !isSkill1 && !isGrounded)
        {
            Debug.Log("Skill 1 activated");
            animator.SetTrigger("Skill1");
            isSkill1 = true;
            rb.velocity = new Vector2(0f, rb.velocity.y);
            StartCoroutine(WaitForSkill1End());
        }
    }
    private IEnumerator WaitForSkill1End()
    {
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Skill1") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }
        isSkill1 = false;
    }
    private void Skill2()
    {
        if (Input.GetKeyDown(KeyCode.L) && !isSkill1 && isGrounded)
        {

            Debug.Log("Skill 2 activated");
            animator.SetTrigger("Skill2");
            isSkill2 = true;
            rb.velocity = new Vector2(0f, rb.velocity.y);
            StartCoroutine(WaitForSkill2End());
        }
    }
    private IEnumerator WaitForSkill2End()
    {
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Skill2") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }
        isSkill2 = false;
    }
}

