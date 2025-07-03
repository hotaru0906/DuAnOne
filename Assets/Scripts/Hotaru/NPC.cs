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

    [Header("Interaction Settings")]
    public bool canInteract = false;

    [Header("Animation Settings")]
    public string walkAnimationName = "Walk";
    public string runAnimationName = "Run";
    public string idleAnimationName = "Idle";
    public Animator bodyAni, clothesAni, pantsAni, hairAni, hatAni, weaponAni;

    [Header("Flip Settings")]
    public bool invertFlip = false;
    //
    private bool movingRight = false;
    private float nextDirectionChangeTime;
    private Rigidbody2D rb;


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
        nextDirectionChangeTime = Time.time + Random.Range(3f, 5f);
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
    // Public method to toggle walk animation
    public void SetCanWalk(bool canWalkState)
    {
        canWalk = canWalkState;
        
        // Update animation immediately based on current state
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

    void OnDrawGizmosSelected()
    {
        // Draw direction indicator
        Gizmos.color = movingRight ? Color.green : Color.red;
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position, direction * 1f);

        // Draw movement range (optional)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
