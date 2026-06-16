using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    // Ссылка на здоровье игрока
    private PlayerHealth playerHealth;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pausePanel.SetActive(false);

        // Находим скрипт здоровья игрока
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        // ЕСЛИ ИГРОК МЁРТВ — НЕ РАЗРЕШАТЬ ПАУЗУ
        if (playerHealth != null && playerHealth.IsDead())
        {
            // Если пауза была включена — выключаем её
            if (isPaused)
            {
                ResumeGame();
            }
            return; // Выходим, не давая открыть паузу
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
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}