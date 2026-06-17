using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{


    [Header("Health Settings")]
    public int maxHealth = 2;
    private int currentHealth;
    
    [Header("Death Screen")]
    public GameObject deathScreenUI;
    
    private bool isDead = false;
    private Driver playerMovement;
    private PlayerFootsteps playerFootsteps;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        // Находим компоненты игрока
        playerMovement = GetComponent<Driver>();
        playerFootsteps = GetComponent<PlayerFootsteps>();
        
        // Скрываем экран смерти в начале
        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        Debug.Log("Player took damage! Health: " + currentHealth + "/" + maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public Animator animator;
    void Die()
    {
        if (animator != null)
        animator.SetTrigger("Die");

        isDead = true;
        Debug.Log("Player died!");
        
        // Отключаем управление игроком
        if (playerMovement != null)
            playerMovement.enabled = false;
        
        if (playerFootsteps != null)
            playerFootsteps.enabled = false;
        
        // Показываем экран смерти
        if (deathScreenUI != null)
            deathScreenUI.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameMusicManager musicManager = FindObjectOfType<GameMusicManager>();
        if (musicManager != null)
            musicManager.StopMusic();
        
        GameObject staminaSlider = GameObject.Find("StaminaSlider");
        if (staminaSlider != null)
            staminaSlider.SetActive(false);

    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
    public bool IsDead()
    {
        return isDead;
    }
}