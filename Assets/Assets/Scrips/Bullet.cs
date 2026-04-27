using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 1;
    public float lifetime = 3f;
    public LayerMask collisionLayers;

    private Vector2 direction;
    private float speed;
    private Rigidbody2D rb;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 dir, float s)
    {
        direction = dir;
        speed = s;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            rb.linearVelocity = direction * speed;
        }


        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & collisionLayers) != 0)
        {
            Player_Health playerHealth = other.gameObject.GetComponent<Player_Health>();

            if (playerHealth == null)
            {
                playerHealth = other.gameObject.GetComponentInParent<Player_Health>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}