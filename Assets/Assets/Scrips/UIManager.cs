using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Zdrowie (Serduszka)")]
    public GameObject[] hearts;

    [Header("Monety")]
    public TextMeshProUGUI coinText;
    private int totalCoins = 0;

    [Header("Menu Pauzy")]
    public GameObject pauseMenu;
    private bool isPaused = false;

    [Header("Ekran Œmierci")]
    public GameObject deathMenu;

    [Header("Muzyka")]
    public AudioSource backgroundMusic;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        UpdateCoinUI();
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (deathMenu != null) deathMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            if (deathMenu != null && deathMenu.activeSelf) return;

            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth) hearts[i].SetActive(true);
            else hearts[i].SetActive(false);
        }
    }

    public void AddCoin(int amount)
    {
        totalCoins += amount;

        if (totalCoins >= 50)
        {
            Player_Health playerHealth = FindObjectOfType<Player_Health>();

            if (playerHealth != null && playerHealth.CanBeHealed())
            {
                playerHealth.Heal(1);
                totalCoins -= 50;
            }
        }

        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = totalCoins.ToString();
    }

    public void PauseGame()
    {
        if (pauseMenu == null) return;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenu == null) return;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowDeathScreen()
    {
        if (deathMenu == null) return;

        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        deathMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }
}