using System.Collections;
using UnityEngine;

public class StrangeSoundsController : MonoBehaviour
{
    [Header("Настройки звуков")]
    public AudioClip[] strangeClips;          // Массив странных звуков
    public float minInterval = 5f;            // Минимальный интервал между звуками
    public float maxInterval = 20f;           // Максимальный интервал между звуками
    public float soundRange = 30f;            // Радиус, в котором может появиться звук

    [Header("Громкость")]
    [Range(0f, 1f)]
    public float soundVolume = 0.8f;

    [Header("Ссылка на EscapeDoor")]
    public EscapeDoor escapeDoor;

    private bool isActive = false;
    private Coroutine soundRoutine;

    void Start()
    {
        if (escapeDoor == null)
            escapeDoor = FindObjectOfType<EscapeDoor>();

        if (strangeClips == null || strangeClips.Length == 0)
        {
            Debug.LogWarning("[StrangeSounds] Нет звуков! Добавьте AudioClip.");
            return;
        }

        Debug.Log("[StrangeSounds] Система инициализирована. Ожидаем сбора 2-й части...");
    }

    void Update()
    {
        if (escapeDoor == null) return;

        int collected = escapeDoor.collectedNotes;

        // Если собрали >= 2 части И система ещё не активна
        if (collected >= 2 && !isActive)
        {
            ActivateStrangeSounds();
        }

        // Если система активна, но мы почему-то потеряли части (откат)
        if (isActive && collected < 2)
        {
            DeactivateStrangeSounds();
        }
    }

    void ActivateStrangeSounds()
    {
        isActive = true;
        Debug.Log("[StrangeSounds] АКТИВИРОВАНЫ! Собрано 2+ частей. Начинаем странные звуки...");

        if (soundRoutine == null)
            soundRoutine = StartCoroutine(StrangeSoundRoutine());
    }

    void DeactivateStrangeSounds()
    {
        isActive = false;
        Debug.Log("[StrangeSounds] ДЕАКТИВИРОВАНЫ.");

        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
            soundRoutine = null;
        }
    }

    IEnumerator StrangeSoundRoutine()
    {
        while (isActive)
        {
            // Ждём случайный интервал
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Проигрываем случайный звук в случайной точке
            PlayRandomSoundAtRandomPosition();

            // Небольшая задержка между звуками (чтобы не накладывались)
            yield return new WaitForSeconds(0.5f);
        }
    }

    void PlayRandomSoundAtRandomPosition()
    {
        if (strangeClips == null || strangeClips.Length == 0)
        {
            Debug.LogWarning("[StrangeSounds] Нет звуков!");
            return;
        }

        // Выбираем случайный звук из массива
        int randomIndex = Random.Range(0, strangeClips.Length);
        AudioClip selectedClip = strangeClips[randomIndex];

        if (selectedClip == null)
        {
            Debug.LogWarning("[StrangeSounds] Выбранный звук null!");
            return;
        }

        // Выбираем случайную позицию на карте (вокруг центра сцены)
        Vector3 randomPosition = GetRandomPositionOnMap();

        // Проигрываем звук в этой точке (3D звук)
        AudioSource.PlayClipAtPoint(selectedClip, randomPosition, soundVolume);

        Debug.Log($"[StrangeSounds] Звук #{randomIndex} в точке {randomPosition}");
    }

    Vector3 GetRandomPositionOnMap()
    {
        // Случайно выбираем: далеко или близко к игроку
        bool isClose = Random.value > 0.5f;

        if (isClose && escapeDoor != null)
        {
            // Поиск игрока по тегу
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // 70% шанс, что звук будет в пределах 5-15 метров от игрока
                Vector3 playerPos = player.transform.position;
                float distance = Random.Range(5f, 15f);
                float angle = Random.Range(0f, Mathf.PI * 2);
                return playerPos + new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
            }
        }

        // Иначе случайная точка на карте
        float range = soundRange;
        return new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
    }

    // Если хотите визуализировать зону звуков в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(soundRange * 2, 0, soundRange * 2));
    }
}