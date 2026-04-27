using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Tryby Aktywacji")]
    [Tooltip("Jeœli zaznaczone, winda ruszy tylko gdy gracz na niej stoi.")]
    public bool moveOnlyWhenPlayerOnBoard = false;

    [Tooltip("Jeœli zaznaczone, winda ruszy gdy gracz na ni¹ wejdzie i bêdzie jechaæ ju¿ zawsze.")]
    public bool startOnTouch = false;

    [Header("Ustawienia Patrolowania (Opcjonalne)")]
    [Tooltip("Przypisz oba punkty, aby platforma lata³a miêdzy nimi.")]
    public Transform pointA;
    public Transform pointB;

    [Header("Ustawienia Ruchu Ci¹g³ego")]
    [Tooltip("Kierunek ruchu, jeœli nie przypisano punktów A i B.")]
    public Vector2 constantDirection = Vector2.up;

    [Header("Parametry Fizyczne")]
    public float moveSpeed = 2f;

    private Transform targetPoint;
    private bool isPlayerOnBoard = false;
    private bool isActivated = false;

    void Start()
    {
        if (pointA != null && pointB != null)
        {
            targetPoint = pointA;
        }
    }

    void FixedUpdate()
    {
        if (moveOnlyWhenPlayerOnBoard && !isPlayerOnBoard)
            return;

        if (startOnTouch && !isActivated)
            return;

        HandleMovement();
    }

    void HandleMovement()
    {
        if (pointA != null && pointB != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.fixedDeltaTime);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
            {
                targetPoint = (targetPoint == pointA) ? pointB : pointA;
            }
        }
        else
        {
            transform.Translate(constantDirection.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnBoard = true;
            isActivated = true;

            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnBoard = false;

            collision.transform.SetParent(null);
        }
    }
}