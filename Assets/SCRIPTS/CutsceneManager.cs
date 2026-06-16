using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("Текст для сцен")]
    public string[] dialogueLines;  // Массив строк (каждая строка = одна сцена)
    
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public GameObject fadePanel;    // Чёрная панель для затемнения
    
    [Header("Настройки")]
    public float fadeDuration = 1f; // Длительность затемнения
    public string nextSceneName = "floor1"; // Сцена после катсцен
    
    private int currentIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        // Показываем первую сцену
        if (dialogueLines.Length > 0)
        {
            dialogueText.text = dialogueLines[0];
        }
        
        // Затемнение в начале
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    void Update()
    {
        if (isTransitioning) return;

        // Левая кнопка мыши — следующая сцена
        if (Input.GetMouseButtonDown(0))
        {
            NextScene();
        }
        // Правая кнопка мыши — предыдущая сцена
        else if (Input.GetMouseButtonDown(1))
        {
            PreviousScene();
        }
    }

    void NextScene()
    {
        if (currentIndex < dialogueLines.Length - 1)
        {
            currentIndex++;
            dialogueText.text = dialogueLines[currentIndex];
        }
        else
        {
            // Это была последняя сцена → переходим в игру
            StartCoroutine(LoadGame());
        }
    }

    void PreviousScene()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            dialogueText.text = dialogueLines[currentIndex];
        }
    }

    IEnumerator LoadGame()
    {
        isTransitioning = true;
        
        // Затемнение
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            // Плавно появляется чёрный фон
            yield return StartCoroutine(FadeToBlack());
        }
        
        // Загружаем игровую сцену
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeIn()
    {
        CanvasGroup group = fadePanel.GetComponent<CanvasGroup>();
        if (group == null)
            group = fadePanel.AddComponent<CanvasGroup>();
        
        group.alpha = 1f;
        
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        
        group.alpha = 0f;
        fadePanel.SetActive(false);
    }

    IEnumerator FadeToBlack()
    {
        CanvasGroup group = fadePanel.GetComponent<CanvasGroup>();
        if (group == null)
            group = fadePanel.AddComponent<CanvasGroup>();
        
        fadePanel.SetActive(true);
        group.alpha = 0f;
        
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        
        group.alpha = 1f;
        yield return new WaitForSeconds(0.3f);
    }
}