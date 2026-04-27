using UnityEngine;

public class Saw : MonoBehaviour
{
    [Header("Ustawienia Rotacji")]
    public float rotationSpeed = 200f;

    [Header("Ustawienia Ruchu")]
    public bool isMoving = false;
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Transform targetPoint;

    void Start()
    {
        if (isMoving && pointA != null)
        {
            targetPoint = pointA;
        }
    }
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (isMoving && pointA != null && pointB != null)
        {
            MoveSaw();
        }
    }

    void MoveSaw()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;
        }
    }

    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Health health = collision.gameObject.GetComponent<Player_Health>();
            if (health != null)
            {
                health.TakeDamage(damage);

                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 7f);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player_Health health = other.GetComponent<Player_Health>();
            if (health != null) health.TakeDamage(damage);
        }
    }
}