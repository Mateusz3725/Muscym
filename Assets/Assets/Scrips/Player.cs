using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Wall Interaction")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public float wallSlidingSpeed = 2f;
    public LayerMask wallLayer;

    [Header("Wall Jump")]
    public Vector2 wallJumpPower = new Vector2(10f, 16f);
    public float wallJumpDuration = 0.2f;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 1;
    private bool canAttack = true;

    [Header("Jump Kill")]
    public Transform stompPoint;
    public float stompRadius = 0.3f;
    public float bounceForce = 15f;
    public int stompDamage = 3;
    public LayerMask enemyLayerMask;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip attackSound;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float moveInput;
    private int state = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDashing || isWallJumping) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySound(jumpSound);
            }
            else if (isWallSliding)
            {
                StartCoroutine(WallJump());
            }
        }

        if (Input.GetButtonDown("Dash") && canDash)
            StartCoroutine(Dash());

        if (Input.GetButtonDown("Attack") && canAttack)
            Attack();

        HandleJumpKill();
        UpdateState();
    }

    void UpdateState()
    {
        if (isDashing)
        {
            state = 4;
            anim.SetInteger("State", state);
            return;
        }

        if (isWallJumping) { state = 2; anim.SetInteger("State", state); return; }
        if (isWallSliding) { state = 5; anim.SetInteger("State", state); return; }

        if (!canAttack) return;

        if (!isGrounded) state = 2;
        else if (moveInput != 0) state = 1;
        else state = 0;

        anim.SetInteger("State", state);
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * transform.localScale.x, wallCheckDistance, wallLayer);

        isWallSliding = isTouchingWall && !isGrounded && rb.linearVelocity.y <= 0 && !isWallJumping;

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        canAttack = true;

        PlaySound(dashSound);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);

        anim.SetInteger("State", 4);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator WallJump()
    {
        isWallJumping = true;
        isWallSliding = false;
        canAttack = true;
        PlaySound(jumpSound);

        float dir = -transform.localScale.x;
        rb.linearVelocity = new Vector2(wallJumpPower.x * dir, wallJumpPower.y);
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }

    void Attack()
    {
        canAttack = false;
        state = 3;
        anim.SetInteger("State", state);
        PlaySound(attackSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip);
    }

    void HandleJumpKill()
    {
        if (rb.linearVelocity.y >= 0) return;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(stompPoint.position, stompRadius, enemyLayerMask);

        if (hitEnemies.Length > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.SendMessage("TakeDamage", stompDamage, SendMessageOptions.DontRequireReceiver);
            }
            isWallJumping = false;
            isDashing = false;
            canAttack = true;
        }
    }

    public void DoDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemyHit in hitEnemies)
        {
            enemyHit.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ResetAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null) { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
        if (attackPoint != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
        if (stompPoint != null) { Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(stompPoint.position, stompRadius); }
        if (wallCheck != null) { Gizmos.color = Color.blue; Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * transform.localScale.x * wallCheckDistance); }
    }
}