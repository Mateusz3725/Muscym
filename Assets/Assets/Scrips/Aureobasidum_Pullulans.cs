using UnityEngine;
using System.Collections;

public class Aureobasidum_Pullulans : MonoBehaviour
{
    [Header("Statystyki Życia")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Efekty Wizualne")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.15f;

    [Header("Ustawienia Ruchu")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float patrolRange = 5f;

    [Header("Wykrywanie")]
    public Vector2 viewSize = new Vector2(5f, 2f);
    public float viewOffset = 2.5f;
    public LayerMask playerLayer;

    [Header("Atak i Obrażenia (dla Gracza)")]
    public Transform attackPoint;
    public float attackRange = 1.2f;
    public float damageRadius = 0.6f;
    public float attackCooldown = 1.5f;
    public int damageAmount = 1;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    private float lastAttackTime;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private bool movingRight = true;
    private float startPosX;

    private const int STATE_IDLE = 0;
    private const int STATE_WALK = 1;
    private const int STATE_ATTACK = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        startPosX = transform.position.x;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        int currentState = anim.GetInteger("State");

        float direction = movingRight ? 1 : -1;
        Vector2 boxOrigin = (Vector2)transform.position + new Vector2(viewOffset * direction, 0);
        bool canSeePlayer = Physics2D.OverlapBox(boxOrigin, viewSize, 0, playerLayer);
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (currentState == STATE_ATTACK)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (canSeePlayer && distanceToPlayer <= attackRange)
        {
            HandleAttackLogic();
        }
        else if (canSeePlayer)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 0.1f);
    }

    void HandleAttackLogic()
    {
        rb.linearVelocity = Vector2.zero;
        FacePlayer();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetInteger("State", STATE_ATTACK);
            lastAttackTime = Time.time;
        }
        else
        {
            if (anim.GetInteger("State") != STATE_ATTACK)
            {
                anim.SetInteger("State", STATE_IDLE);
            }
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    void Patrol()
    {
        anim.SetInteger("State", STATE_WALK);
        float leftLimit = startPosX - patrolRange;
        float rightLimit = startPosX + patrolRange;

        if (movingRight)
        {
            rb.linearVelocity = new Vector2(walkSpeed, rb.linearVelocity.y);
            if (transform.position.x >= rightLimit) Flip();
        }
        else
        {
            rb.linearVelocity = new Vector2(-walkSpeed, rb.linearVelocity.y);
            if (transform.position.x <= leftLimit) Flip();
        }
    }

    void ChasePlayer()
    {
        anim.SetInteger("State", STATE_WALK);
        float dir = player.position.x - transform.position.x;
        rb.linearVelocity = new Vector2(Mathf.Sign(dir) * chaseSpeed, rb.linearVelocity.y);

        if (dir > 0 && !movingRight) Flip();
        else if (dir < 0 && movingRight) Flip();
    }

    void FacePlayer()
    {
        float dir = player.position.x - transform.position.x;
        if (dir > 0 && !movingRight) Flip();
        else if (dir < 0 && movingRight) Flip();
    }

    public void PerformDamage()
    {
        if (attackPoint == null) return;
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, damageRadius, playerLayer);
        foreach (Collider2D p in hitPlayers)
        {
            Player_Health health = p.GetComponent<Player_Health>();
            if (health != null) health.TakeDamage(damageAmount);
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnDrawGizmosSelected()
    {
        float dir = movingRight ? 1 : -1;
        if (!Application.isPlaying) dir = 1;
        Vector2 boxOrigin = (Vector2)transform.position + new Vector2(viewOffset * dir, 0);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxOrigin, viewSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, damageRadius);
        }
    }
}