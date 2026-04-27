using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Settings")]
    public int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Health playerHealth = collision.GetComponent<Player_Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
    private void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        if (box != null)
        {
            Gizmos.color = new Color(0.7f, 0f, 1f, 0.4f);

            Vector3 center = transform.position + (Vector3)box.offset;
            Gizmos.DrawCube(center, box.size);

            Gizmos.color = new Color(0.7f, 0f, 1f, 1f);
            Gizmos.DrawWireCube(center, box.size);
        }
    }
}