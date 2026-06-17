using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryCutscene : MonoBehaviour
{
    [Header("Текст")]
    public string[] victoryLines;
    public TextMeshProUGUI dialogueText;
    public GameObject fadePanel;
    public float fadeDuration = 1f;

    private int currentIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        if (victoryLines.Length > 0)
            dialogueText.text = victoryLines[0];
        
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentIndex < victoryLines.Length - 1)
            {
                currentIndex++;
                dialogueText.text = victoryLines[currentIndex];
            }
            else
            {
                StartCoroutine(LoadVictoryScreen());
            }
        }
        else if (Input.GetMouseButtonDown(1) && currentIndex > 0)
        {
            currentIndex--;
            dialogueText.text = victoryLines[currentIndex];
        }
    }

    IEnumerator LoadVictoryScreen()
    {
        isTransitioning = true;
        
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            yield return StartCoroutine(FadeToBlack());
        }
        
        SceneManager.LoadScene("VictoryScene");
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