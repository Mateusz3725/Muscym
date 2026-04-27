using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Pilobolus_Crystal : MonoBehaviour
{
    private const int STATE_IDLE = 0;
    private const int STATE_ATTACK = 1;

    [Header("Detection & Shooting")]
    public float detectionRange = 10f;
    public float shootingInterval = 2f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip chargeSound;
    public AudioClip shootSound;

    private Transform player;
    private Animator anim;
    private float timeSinceLastShot;
    private bool isPlayerInSight;
    private int currentState = STATE_IDLE;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        anim = GetComponent<Animator>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        timeSinceLastShot = shootingInterval;
        SetAnimationState(STATE_IDLE);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isPlayerInSight = distanceToPlayer <= detectionRange;

        if (!isPlayerInSight)
        {
            timeSinceLastShot = shootingInterval;
            SetAnimationState(STATE_IDLE);
            return;
        }

        Vector3 targetDir = player.position - transform.position;
        transform.localScale = new Vector3(targetDir.x > 0 ? 1 : -1, 1, 1);

        timeSinceLastShot += Time.deltaTime;

        if (timeSinceLastShot >= shootingInterval)
        {
            if (currentState != STATE_ATTACK)
            {
                timeSinceLastShot = 0f;
                SetAnimationState(STATE_ATTACK);
            }
        }
    }

    private void SetAnimationState(int newState)
    {
        if (currentState != newState && anim != null)
        {
            anim.SetInteger("State", newState);
            currentState = newState;

            if (newState == STATE_ATTACK)
            {
                PlaySound(chargeSound);
            }
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void Shoot()
    {
        if (player == null || !isPlayerInSight || bulletPrefab == null || firePoint == null) return;

        PlaySound(shootSound);

        Vector2 direction = (player.position - firePoint.position).normalized;
        GameObject proj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bulletScript = proj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, bulletSpeed);
        }
    }

    public void EndAttack()
    {
        SetAnimationState(STATE_IDLE);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}