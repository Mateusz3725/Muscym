using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleMenuManager : MonoBehaviour
{
    [Header("Ustawienia Sceny")]
    public string sceneToLoad;

    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}