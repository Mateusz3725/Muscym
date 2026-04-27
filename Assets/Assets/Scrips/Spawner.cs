using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Ustawienia Spawnowania")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float spawnDelay = 2.0f;

    [Header("Wykrywanie Gracza")]
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private LayerMask playerLayer;

    private bool playerInRange;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        playerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (playerInRange)
            {
                SpawnObject();
            }
        }
    }

    private void SpawnObject()
    {
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (playerInRange)
        {
            Gizmos.color = new Color(0, 1, 1, 0.2f);
            Gizmos.DrawSphere(transform.position, detectionRadius);
        }
    }
}