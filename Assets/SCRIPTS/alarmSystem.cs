using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    [Header("--- Настройки Таймера ---")]
    public float baseInterval = 60f;
    public float minInterval = 5f;
    public int unlockThreshold = 3;

    [Header("--- Настройки Звука ---")]
    public AudioSource alarmAudioSource;
    [Range(0f, 1f)]
    public float alarmVolume = 1f;

    [Header("--- Настройки Света ---")]
    public Color alarmColor = Color.red;
    public float blinkSpeed = 4f;
    public float alarmDuration = 10f;

    [Header("--- Ссылка на студенческий билет ---")]
    public EscapeDoor escapeDoor;

    [Header("--- Настройки мигания ---")]
    public GameObject flashingLightObject;  // Специальный объект с Light для мигания

    private Light alarmLight;
    private Color originalLightColor;
    private Color originalAmbientColor;
    private bool hasSavedAmbient = false;
    private bool isAlarmActive = false;
    private float timer = 0f;
    private bool isUnlocked = false;

    void Start()
    {
        if (alarmAudioSource == null)
            alarmAudioSource = GetComponent<AudioSource>();

        if (alarmAudioSource != null)
            alarmAudioSource.volume = alarmVolume;

        if (escapeDoor == null)
            escapeDoor = FindObjectOfType<EscapeDoor>();

        if (flashingLightObject != null)
        {
            alarmLight = flashingLightObject.GetComponent<Light>();
            if (alarmLight != null)
            {
                originalLightColor = alarmLight.color;
            }
        }

        if (!hasSavedAmbient)
        {
            originalAmbientColor = RenderSettings.ambientLight;
            hasSavedAmbient = true;
        }

        CheckUnlockStatus();
    }

    void Update()
    {
        CheckUnlockStatus();

        if (!isUnlocked || isAlarmActive) return;

        float currentInterval = GetCurrentInterval();
        timer += Time.deltaTime;

        if (timer >= currentInterval)
        {
            StartCoroutine(ActivateAlarm());
            timer = 0f;
        }
    }

    void CheckUnlockStatus()
    {
        if (escapeDoor == null) return;

        int collected = escapeDoor.collectedNotes;

        if (collected >= unlockThreshold && !isUnlocked)
        {
            isUnlocked = true;
            Debug.Log($"[Сирена] РАЗБЛОКИРОВАНА! Собрано {collected} частей.");
            timer = GetCurrentInterval();
        }
    }

    float GetCurrentInterval()
    {
        if (escapeDoor == null) return baseInterval;

        int collected = escapeDoor.collectedNotes;
        int total = escapeDoor.totalNotesNeeded;

        int effectiveCollected = Mathf.Max(0, collected - unlockThreshold);
        int effectiveTotal = Mathf.Max(1, total - unlockThreshold);

        float progress = Mathf.Clamp01((float)effectiveCollected / effectiveTotal);
        return Mathf.Lerp(baseInterval, minInterval, progress);
    }

    IEnumerator ActivateAlarm()
    {
        isAlarmActive = true;

        // 1. Включаем звук
        if (alarmAudioSource != null && alarmAudioSource.clip != null)
        {
            alarmAudioSource.volume = alarmVolume;
            alarmAudioSource.loop = true;
            alarmAudioSource.Play();
            Debug.Log("[Сирена] Звук включен!");
        }
        else
        {
            Debug.LogWarning("[Сирена] Нет AudioSource или AudioClip!");
        }

        // 2. Мигание света
        float elapsed = 0f;
        float currentDuration = GetAlarmDuration();
        bool isFlashingLight = alarmLight != null;

        while (elapsed < currentDuration)
        {
            elapsed += Time.deltaTime;

            // Волна мигания
            float wave = Mathf.PingPong(Time.time * blinkSpeed, 1f);

            // Мигаем специальным источником света
            if (isFlashingLight)
            {
                alarmLight.color = Color.Lerp(originalLightColor, alarmColor, wave);
            }

            // Мигаем ambient светом (окружение)
            RenderSettings.ambientLight = Color.Lerp(originalAmbientColor, alarmColor, wave);

            yield return null;
        }

        // 3. ВЫКЛЮЧАЕМ звук
        if (alarmAudioSource != null)
        {
            alarmAudioSource.Stop();
            alarmAudioSource.loop = false;
            Debug.Log("[Сирена] Звук выключен!");
        }

        // 4. ВОЗВРАЩАЕМ оригинальные цвета
        if (isFlashingLight)
        {
            alarmLight.color = originalLightColor;
        }

        RenderSettings.ambientLight = originalAmbientColor;

        isAlarmActive = false;
        Debug.Log("[Сирена] ЗАВЕРШЕНА! Цвета восстановлены.");
    }

    float GetAlarmDuration()
    {
        if (escapeDoor == null) return alarmDuration;

        int collected = escapeDoor.collectedNotes;
        int total = escapeDoor.totalNotesNeeded;

        int effectiveCollected = Mathf.Max(0, collected - unlockThreshold);
        int effectiveTotal = Mathf.Max(1, total - unlockThreshold);

        float progress = Mathf.Clamp01((float)effectiveCollected / effectiveTotal);
        return Mathf.Lerp(10f, 3f, progress);
    }

    public void ForceActivateAlarm()
    {
        CheckUnlockStatus();

        if (!isUnlocked)
        {
            Debug.Log("[Сирена] Нельзя включить! Собрано меньше 3 частей.");
            return;
        }

        if (!isAlarmActive)
        {
            StartCoroutine(ActivateAlarm());
        }
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }

    // Аварийный сброс (можно вызвать по нажатию клавиши для отладки)
    public void EmergencyReset()
    {
        if (alarmLight != null)
        {
            alarmLight.color = originalLightColor;
        }
        RenderSettings.ambientLight = originalAmbientColor;
        isAlarmActive = false;

        if (alarmAudioSource != null)
        {
            alarmAudioSource.Stop();
            alarmAudioSource.loop = false;
        }

        Debug.Log("[Сирена] АВАРИЙНЫЙ СБРОС!");
    }
}