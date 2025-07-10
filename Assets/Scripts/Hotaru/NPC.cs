using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    public bool canMove = true;
    public bool canWalk = true; // Toggle để bật/tắt animation walk
    public bool canRun = false;
    
    [Header("Movement Timing Settings")]
    public float minDirectionChangeTime = 5f; // Thời gian tối thiểu trước khi đổi hướng
    public float maxDirectionChangeTime = 10f; // Thời gian tối đa trước khi đổi hướng
    public float runDirectionChangeTime = 10f; // Thời gian đổi hướng khi chạy
    public float interactionDirectionChangeTime = 8f; // Thời gian đổi hướng khi player tương tác (5s + 3s buffer)

    [Header("Interaction Settings")]
    public bool canInteract = false;

    [Header("Animation Settings")]
    public string walkAnimationName = "Walk";
    public string runAnimationName = "Run";
    public string idleAnimationName = "Idle";
    public string jumpAnimationName = "Jump";
    public string fallAnimationName = "Fall";
    public Animator bodyAni, clothesAni, pantsAni, hairAni, hatAni, weaponAni;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public LayerMask groundLayerMask = 1; // Layer của ground để check grounded
    public float groundCheckDistance = 0.1f; // Khoảng cách check ground

    [Header("Flip Settings")]
    public bool invertFlip = false;
    private bool movingRight = false;
    private float nextDirectionChangeTime;
    private Rigidbody2D rb;
    
    // Jump variables
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool canJumpTrigger = false; // Có thể nhảy khi chạm CanJump trigger


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }
        if (bodyAni == null || clothesAni == null || pantsAni == null || hairAni == null || hatAni == null || weaponAni == null)
        {
            Debug.LogWarning($"One or more Animator components not found on {gameObject.name}. Animation will not work.");
        }

        if (canMove)
        {
            SetNextDirectionChangeTime();
            SetMovementAnimation(true);
        }
        else
        {
            SetMovementAnimation(false);
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleJumpAndFall();
        
        if (canMove)
            HandleMovement();
    }

    void HandleMovement()
    {
        if (!canRun && Time.time >= nextDirectionChangeTime)
        {
            movingRight = !movingRight;
            SetNextDirectionChangeTime();
            FlipSprite();
        }
        else if (canRun)
        {
            if (Time.time >= nextDirectionChangeTime)
            {
                movingRight = !movingRight;
                nextDirectionChangeTime = Time.time + 2f;
                FlipSprite();
            }
        }

        float currentSpeed = canRun ? runSpeed : moveSpeed;
        float moveDirection = movingRight ? 1f : -1f;
        Vector2 movement = Vector2.right * moveDirection * currentSpeed;
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }

    void SetNextDirectionChangeTime()
    {
        nextDirectionChangeTime = Time.time + Random.Range(5f, 10f);
    }

    void FlipSprite()
    {
        if (!invertFlip)
        {
            if (movingRight && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (!movingRight && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            if (movingRight && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (!movingRight && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        if (canMove)
        {
            SetMovementAnimation(true);
        }
    }

    void SetMovementAnimation(bool isMoving)
    {
        string stateName;

        if (!isMoving || !canWalk)
        {
            // If not moving or canWalk is false, use idle animation
            stateName = idleAnimationName;
        }
        else if (canRun)
        {
            stateName = runAnimationName;
        }
        else
        {
            stateName = walkAnimationName;
        }

        if (bodyAni != null)
            bodyAni.Play(stateName);
        if (clothesAni != null)
            clothesAni.Play(stateName);
        if (pantsAni != null)
            pantsAni.Play(stateName);
        if (hairAni != null)
            hairAni.Play(stateName);
        if (hatAni != null)
            hatAni.Play(stateName);
        if (weaponAni != null)
            weaponAni.Play(stateName);
    }
    void SetWalkAnimation(bool isWalking)
    {
        SetMovementAnimation(isWalking);
    }

    public void SetCanMove(bool canMoveState)
    {
        canMove = canMoveState;
        if (!canMove && rb != null)
        {
            rb.velocity = Vector2.zero;
            SetMovementAnimation(false);
        }
        else if (canMove)
        {
            SetNextDirectionChangeTime();
            SetMovementAnimation(true);
        }
    }

    public void SetCanRun(bool canRunState)
    {
        canRun = canRunState;
        if (canMove)
        {
            SetMovementAnimation(true);

            if (canRun)
            {
                nextDirectionChangeTime = Time.time;
            }
            else
            {
                SetNextDirectionChangeTime();
            }
        }
    }
    public void SetCanWalk(bool canWalkState)
    {
        canWalk = canWalkState;
        
        if (canMove)
        {
            SetMovementAnimation(true);
        }
        else
        {
            SetMovementAnimation(false);
        }
        
        Debug.Log($"NPC {gameObject.name}: canWalk set to {canWalk}");
    }

    public void Interact()
    {
        if (canInteract)
        {
            Debug.Log("Interacting with NPC!");
            // Dialogue logic here
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canInteract)
        {
            Debug.Log($"Player is near {gameObject.name} - Can interact!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CanJump"))
        {
            canJumpTrigger = true;
            Debug.Log($"NPC {gameObject.name}: Entered CanJump trigger - {other.gameObject.name}");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CanJump"))
        {
            canJumpTrigger = false;
            Debug.Log($"NPC {gameObject.name}: Exited CanJump trigger - {other.gameObject.name}");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw direction indicator
        Gizmos.color = movingRight ? Color.green : Color.red;
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position, direction * 1f);

        // Draw movement range (optional)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position;
        Vector3 rayEnd = rayStart + Vector3.down * groundCheckDistance;
        Gizmos.DrawLine(rayStart, rayEnd);
        
        // Draw jump status
        if (isJumping)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one * 0.3f);
        }
        else if (isFalling)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position + Vector3.down * 0.5f, Vector3.one * 0.3f);
        }
    }

    void CheckGrounded()
    {
        // Raycast xuống dưới để check xem có đang ở trên ground không
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayerMask);
        
        bool wasGrounded = isGrounded;
        isGrounded = hit.collider != null;
        
        // Debug ground check
        if (wasGrounded != isGrounded)
        {
            Debug.Log($"NPC {gameObject.name}: Grounded = {isGrounded}");
        }
    }

    void HandleJumpAndFall()
    {
        // Kiểm tra nếu có trigger CanJump và đang ở trên ground
        if (canJumpTrigger && isGrounded && !isJumping)
        {
            PerformJump();
            canJumpTrigger = false; // Reset trigger
        }
        
        // Kiểm tra trạng thái rơi
        if (!isGrounded && rb.velocity.y < 0 && !isFalling)
        {
            // Bắt đầu rơi
            isFalling = true;
            isJumping = false;
            SetJumpAnimation("fall");
            Debug.Log($"NPC {gameObject.name}: Started falling");
        }
        else if (isGrounded && (isJumping || isFalling))
        {
            // Landed on ground
            isJumping = false;
            isFalling = false;
            SetMovementAnimation(canMove);
            Debug.Log($"NPC {gameObject.name}: Landed on ground");
        }
    }
    
    void PerformJump()
    {
        if (rb != null)
        {
            // Apply jump force
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            isFalling = false;
            
            // Play jump animation
            SetJumpAnimation("jump");
            
            Debug.Log($"NPC {gameObject.name}: Jumped with force {jumpForce}");
        }
    }
    
    void SetJumpAnimation(string animationType)
    {
        string stateName;
        
        if (animationType == "jump")
        {
            stateName = jumpAnimationName;
        }
        else if (animationType == "fall")
        {
            stateName = fallAnimationName;
        }
        else
        {
            return;
        }
        
        // Play animation on all animators
        if (bodyAni != null)
            bodyAni.Play(stateName);
        if (clothesAni != null)
            clothesAni.Play(stateName);
        if (pantsAni != null)
            pantsAni.Play(stateName);
        if (hairAni != null)
            hairAni.Play(stateName);
        if (hatAni != null)
            hatAni.Play(stateName);
        if (weaponAni != null)
            weaponAni.Play(stateName);
        
        Debug.Log($"NPC {gameObject.name}: Playing {stateName} animation");
    }
}
