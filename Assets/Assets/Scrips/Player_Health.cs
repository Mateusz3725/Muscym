using UnityEngine;
using System.Collections;

public class Player_Health : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Invulnerability & Visuals")]
    public float invulnerabilityDuration = 1.0f;
    public Color damageColor = Color.red;
    private Color normalColor = Color.white;
    private bool isInvulnerable = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip healSound;
    public AudioClip deathSound;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        PlaySound(damageSound);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeTemporarilyInvulnerable());
        }
    }

    public bool CanBeHealed()
    {
        return currentHealth < maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        PlaySound(healSound);
    }

    private void UpdateUI()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateHealth(currentHealth);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator BecomeTemporarilyInvulnerable()
    {
        isInvulnerable = true;
        if (spriteRenderer != null) spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(invulnerabilityDuration);

        if (spriteRenderer != null) spriteRenderer.color = normalColor;
        isInvulnerable = false;
    }

    void Die()
    {
        PlaySound(deathSound);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; 
        }

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;

        Player playerController = GetComponent<Player>();
        if (playerController != null) playerController.enabled = false;

        if (UIManager.instance != null) UIManager.instance.ShowDeathScreen();
    }
}