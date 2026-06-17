using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    // Ссылка на слайдер стамины
    public GameObject staminaSlider;  // ← Добавь это поле

    private PlayerHealth playerHealth;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pausePanel.SetActive(false);
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead())
        {
            if (isPaused)
            {
                ResumeGame();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // ПОКАЗЫВАЕМ СЛАЙДЕР
        if (staminaSlider != null)
            staminaSlider.SetActive(true);
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // СКРЫВАЕМ СЛАЙДЕР
        if (staminaSlider != null)
            staminaSlider.SetActive(false);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}