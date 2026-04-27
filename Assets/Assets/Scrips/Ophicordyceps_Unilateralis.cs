using UnityEngine;
using System.Collections;

public class Ophicordyceps_Unilateralis : MonoBehaviour
{
    [Header("Statystyki Życia")]
    public int maxHealth = 2;
    private int currentHealth;
    private bool isDead = false;

    [Header("Ustawienia Szarży")]
    public float chargeSpeed = 6f;
    public float detectionRange = 7f;
    public float attackCooldown = 2f;
    public float stunDuration = 0.8f;
    public int contactDamage = 1;

    [Header("Wybuch (AOE)")]
    public float explosionRadius = 2.5f;
    public int explosionDamage = 3;
    public LayerMask playerLayer;

    [Header("Efekty")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.15f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip explosionSound;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private bool isWaiting = false;
    private bool isCharging = false;
    private bool isStunned = false;

    private const int STATE_IDLE = 0;
    private const int STATE_WALK = 1;
    private const int STATE_DEATH = 2;
    private const int STATE_BOOM = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHealth = maxHealth;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f;
            audioSource.playOnAwake = false;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null || isWaiting || isCharging || isStunned) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange) StartCoroutine(Charge());
    }

    IEnumerator Charge()
    {
        isCharging = true;
        anim.SetInteger("State", STATE_WALK);

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        while (isCharging && !isDead && !isStunned)
        {
            if (player == null) break;
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * chargeSpeed, rb.linearVelocity.y);

            if (direction.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            else if (direction.x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

            yield return null;
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || isStunned) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Health ph = collision.gameObject.GetComponent<Player_Health>();
            if (ph != null) ph.TakeDamage(contactDamage);
            StopChargeAndRest();
        }
    }

    void StopChargeAndRest()
    {
        isCharging = false;
        rb.linearVelocity = Vector2.zero;
        if (!isDead && !isStunned) StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        isWaiting = true;
        anim.SetInteger("State", STATE_IDLE);
        yield return new WaitForSeconds(attackCooldown);
        isWaiting = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        StopAllCoroutines();
        isCharging = false;
        isWaiting = false;

        StartCoroutine(FlashEffect());

        if (currentHealth <= 0) Die();
        else StartCoroutine(StunRoutine());
    }

    IEnumerator StunRoutine()
    {
        isStunned = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetInteger("State", STATE_IDLE);
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        StartCoroutine(Cooldown());
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        anim.SetInteger("State", STATE_DEATH);
    }

    public void PlayExplosionSFX()
    {
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
    }

    public void TriggerExplosionState()
    {
        anim.SetInteger("State", STATE_BOOM);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (Collider2D p in hitPlayers)
        {
            Player_Health health = p.GetComponent<Player_Health>();
            if (health != null) health.TakeDamage(explosionDamage);
        }

        Destroy(gameObject, 0.5f);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}