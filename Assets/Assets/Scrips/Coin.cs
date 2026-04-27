using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Ustawienia")]
    public int scoreValue = 1;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;

    private SpriteRenderer spriteRenderer;
    private Collider2D coinCollider;
    private bool isCollected = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coinCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Collect();
        }
    }

    private void Collect()
    {
        isCollected = true;

        if (UIManager.instance != null)
        {
            UIManager.instance.AddCoin(scoreValue);
        }

        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);

            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (coinCollider != null) coinCollider.enabled = false;

            Destroy(gameObject, collectSound.length);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}