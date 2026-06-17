using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;  // ← Добавь для TextMeshPro

public class CutsceneManager : MonoBehaviour
{
    [Header("Картинки для сцен")]
    public Sprite[] cutsceneImages;
    
    [Header("Текст для каждой картинки")]
    public string[] dialogueTexts;  // ← Массив текста (должен совпадать по размеру с картинками!)
    
    [Header("UI")]
    public Image cutsceneImage;
    public TextMeshProUGUI dialogueText;  // ← Ссылка на текст
    public GameObject fadePanel;
    
    [Header("Настройки")]
    public float fadeDuration = 1f;
    public string nextSceneName = "floor1";
    
    private int currentIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        // Показываем первую картинку
        if (cutsceneImages.Length > 0)
            cutsceneImage.sprite = cutsceneImages[0];
        
        // Показываем первый текст
        if (dialogueTexts.Length > 0)
            dialogueText.text = dialogueTexts[0];
        
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Input.GetMouseButtonDown(0))  // ЛКМ → следующая
        {
            NextImage();
        }
        else if (Input.GetMouseButtonDown(1))  // ПКМ → предыдущая
        {
            PreviousImage();
        }
    }

    void NextImage()
    {
        if (currentIndex < cutsceneImages.Length - 1)
        {
            currentIndex++;
            
            // Меняем картинку
            cutsceneImage.sprite = cutsceneImages[currentIndex];
            
            // Меняем текст (если есть текст для этой картинки)
            if (currentIndex < dialogueTexts.Length && dialogueTexts[currentIndex] != null)
            {
                dialogueText.text = dialogueTexts[currentIndex];
            }
        }
        else
        {
            // Это была последняя картинка → переходим в игру
            StartCoroutine(LoadGame());
        }
    }

    void PreviousImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            
            // Меняем картинку
            cutsceneImage.sprite = cutsceneImages[currentIndex];
            
            // Меняем текст
            if (currentIndex < dialogueTexts.Length && dialogueTexts[currentIndex] != null)
            {
                dialogueText.text = dialogueTexts[currentIndex];
            }
        }
    }

    IEnumerator LoadGame()
    {
        isTransitioning = true;
        
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            yield return StartCoroutine(FadeToBlack());
        }
        
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeIn()
    {
        CanvasGroup group = fadePanel.GetComponent<CanvasGroup>();
        if (group == null) group = fadePanel.AddComponent<CanvasGroup>();
        
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
        if (group == null) group = fadePanel.AddComponent<CanvasGroup>();
        
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