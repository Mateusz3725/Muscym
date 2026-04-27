using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenePortal : MonoBehaviour
{
    [Header("Ustawienia Portalu")]
    public string sceneToLoad;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip teleportSound;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    IEnumerator TeleportRoutine()
    {
        isTeleporting = true;

        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);

            yield return new WaitForSeconds(teleportSound.length);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            isTeleporting = false;
        }
    }
}