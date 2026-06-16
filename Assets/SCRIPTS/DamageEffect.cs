using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageEffect : MonoBehaviour
{
    [Header("Настройки эффекта")]
    public Image damageOverlay;           // Затемнение (UI Image)
    public float fadeInDuration = 0.3f;   // Время появления затемнения
    public float fadeOutDuration = 1.5f;  // Время исчезновения затемнения
    public Color damageColor = Color.red; // Цвет затемнения
    
    private Coroutine currentEffect = null;
    
    void Start()
    {
        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);
        }
    }
    
    public void TriggerDamageEffect()
    {
        if (damageOverlay == null)
        {
            Debug.LogWarning("[DamageEffect] damageOverlay не назначен!");
            return;
        }
        
        // Останавливаем текущий эффект, если он идёт
        if (currentEffect != null)
            StopCoroutine(currentEffect);
        
        // Запускаем новый эффект
        currentEffect = StartCoroutine(DamageEffectRoutine());
    }
    
    IEnumerator DamageEffectRoutine()
    {
        float elapsed = 0f;
        Color targetColor = new Color(damageColor.r, damageColor.g, damageColor.b, 0.5f); // 50% прозрачности
        
        // 1. Появляем затемнение (плавно)
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 0.5f, elapsed / fadeInDuration);
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, alpha);
            yield return null;
        }
        
        // Фиксируем максимальное затемнение
        damageOverlay.color = targetColor;
        
        // Небольшая пауза (чтобы эффект ощущался)
        yield return new WaitForSeconds(0.2f);
        
        // 2. Плавно убираем затемнение
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0f, elapsed / fadeOutDuration);
            damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, alpha);
            yield return null;
        }
        
        // Убеждаемся, что прозрачность 0
        damageOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);
        
        currentEffect = null;
    }
}