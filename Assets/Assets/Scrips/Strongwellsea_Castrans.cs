using UnityEngine;
using System.Collections;

public class Strongwellsea_Castrans : MonoBehaviour
{
    [Header("Statystyki Życia")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Ustawienia Ruchu")]
    public float speed = 3f;
    public float detectionRange = 8f;
    public Transform player;

    [Header("Atak Przeciwnika (na Gracza)")]
    public int damageToPlayer = 1;
    public float attackCooldown = 3f;
    public float hitCooldown = 0.8f;
    public LayerMask playerLayer;
    private bool isWaiting = false;
    private Coroutine cooldownCoroutine;

    [Header("Efekty i Fizyka")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.15f;
    private bool isBeingKnockedBack = false;

    [Header("Referencje")]
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private const int IDLE_STATE = 0;
    private const int ATTACK_STATE = 1;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Update()
    {
        if (player == null || isDead || isBeingKnockedBack) return;

        if (isWaiting)
        {
            UpdateAnimation(IDLE_STATE);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
            MoveToPlayer();
        else
            UpdateAnimation(IDLE_STATE);
    }

    void MoveToPlayer()
    {
        UpdateAnimation(ATTACK_STATE);
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        if (spriteRenderer != null)
            spriteRenderer.flipX = (player.position.x < transform.position.x) ? false : true;
    }

    void UpdateAnimation(int state)
    {
        if (anim != null) anim.SetInteger("State", state);
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
        else
        {
            StartNewCooldown(hitCooldown);
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
        isDead = true;
        Destroy(gameObject);
    }

    void StartNewCooldown(float duration)
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        cooldownCoroutine = StartCoroutine(StartCooldownRoutine(duration));
    }

    IEnumerator StartCooldownRoutine(float duration)
    {
        isWaiting = true;
        yield return new WaitForSeconds(duration);
        isWaiting = false;
        cooldownCoroutine = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || isWaiting) return;

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            if (other.transform.position.y > transform.position.y + 0.3f) return;

            Player_Health playerHealth = other.gameObject.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
                StartNewCooldown(attackCooldown);
            }
        }
    }
}