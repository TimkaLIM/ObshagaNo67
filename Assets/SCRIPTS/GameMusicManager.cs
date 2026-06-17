using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    [Header("Музыка")]
    public AudioSource normalMusicSource;   // Тихая музыка (до 5 частей)
    public AudioSource intenseMusicSource;  // Эпичная музыка (при 5 частях)
    
    [Header("Настройки громкости")]
    public float normalVolume = 0.15f;
    public float intenseVolume = 0.7f;
    public float fadeSpeed = 0.5f;
    
    [Header("Ссылка на EscapeDoor")]
    public EscapeDoor escapeDoor;
    
    private bool isIntenseMusicPlaying = false;
    private bool isMusicPlaying = false;

    void Start()
    {
        if (normalMusicSource == null)
            normalMusicSource = GetComponent<AudioSource>();
        
        if (intenseMusicSource == null)
        {
            // Если нет второго AudioSource — создаём
            intenseMusicSource = gameObject.AddComponent<AudioSource>();
            intenseMusicSource.loop = true;
            intenseMusicSource.spatialBlend = 0;
            intenseMusicSource.playOnAwake = false;
        }
        
        if (escapeDoor == null)
            escapeDoor = FindObjectOfType<EscapeDoor>();
        
        // Настройка обычной музыки
        normalMusicSource.volume = normalVolume;
        normalMusicSource.loop = true;
        normalMusicSource.spatialBlend = 0;
        normalMusicSource.Play();
        isMusicPlaying = true;
        
        // Настройка интенсивной музыки (пока выключена)
        intenseMusicSource.volume = 0f;
        intenseMusicSource.loop = true;
        intenseMusicSource.spatialBlend = 0;
        // НЕ запускаем пока
        
        Debug.Log("[Музыка] Обычная музыка начала играть (тихо)");
    }

    void Update()
    {
        if (escapeDoor == null) return;
        
        int collected = escapeDoor.collectedNotes;
        int total = escapeDoor.totalNotesNeeded;
        
        // Если собрано 5 частей — переключаем на интенсивную музыку
        if (collected >= total && !isIntenseMusicPlaying && isMusicPlaying)
        {
            StartIntenseMusic();
        }
        // Если собрано меньше 5 — играет обычная музыка
        else if (collected < total && isMusicPlaying)
        {
            // Поддерживаем громкость обычной музыки
            if (normalMusicSource.volume < normalVolume)
            {
                normalMusicSource.volume = Mathf.Lerp(normalMusicSource.volume, normalVolume, Time.deltaTime * fadeSpeed);
            }
        }
    }

    void StartIntenseMusic()
    {
        Debug.Log("[Музыка] Переключение на интенсивную музыку!");
        
        isIntenseMusicPlaying = true;
        
        // Запускаем интенсивную музыку с нулевой громкостью
        intenseMusicSource.volume = 0f;
        intenseMusicSource.Play();
        
        // Начинаем плавное затухание обычной музыки и нарастание интенсивной
        StartCoroutine(FadeMusic());
    }

    System.Collections.IEnumerator FadeMusic()
    {
        float elapsed = 0f;
        float duration = 2f; // Длительность перехода
        
        float startNormalVolume = normalMusicSource.volume;
        float startIntenseVolume = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Плавное затухание обычной музыки
            normalMusicSource.volume = Mathf.Lerp(startNormalVolume, 0f, t);
            
            // Плавное нарастание интенсивной музыки
            intenseMusicSource.volume = Mathf.Lerp(startIntenseVolume, intenseVolume, t);
            
            yield return null;
        }
        
        // Фиксируем финальные значения
        normalMusicSource.volume = 0f;
        normalMusicSource.Stop();
        
        intenseMusicSource.volume = intenseVolume;
        
        Debug.Log("[Музыка] Интенсивная музыка заиграла на полную громкость!");
    }

    // Остановить всю музыку (при смерти или победе)
    public void StopMusic()
    {
        if (normalMusicSource != null)
        {
            normalMusicSource.Stop();
            normalMusicSource.volume = 0f;
        }
        
        if (intenseMusicSource != null)
        {
            intenseMusicSource.Stop();
            intenseMusicSource.volume = 0f;
        }
        
        isMusicPlaying = false;
        isIntenseMusicPlaying = false;
        Debug.Log("[Музыка] Вся музыка остановлена.");
    }

    // Запустить музыку заново (если остановлена)
    public void StartMusic()
    {
        if (!isMusicPlaying)
        {
            normalMusicSource.volume = normalVolume;
            normalMusicSource.Play();
            isMusicPlaying = true;
            isIntenseMusicPlaying = false;
            Debug.Log("[Музыка] Обычная музыка запущена.");
        }
    }
}