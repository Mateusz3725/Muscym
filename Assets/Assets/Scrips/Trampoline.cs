using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Trampoline : MonoBehaviour
{
    [Header("Ustawienia Odbicia")]
    public float bounceForce = 15f;
    public float delayBeforeLaunch = 0.15f;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                StartCoroutine(BounceSequence(playerRb));
            }
        }
    }

    private IEnumerator BounceSequence(Rigidbody2D rb)
    {
        if (_animator != null) _animator.SetInteger("State", 1);

        yield return new WaitForSeconds(delayBeforeLaunch);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);

        if (_animator != null) _animator.SetInteger("State", 0);
    }
}