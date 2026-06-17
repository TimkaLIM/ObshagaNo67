using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EscapeDoor : MonoBehaviour
{
    [Header("Настройки побега")]
    public int totalNotesNeeded = 5;
    public string victorySceneName = "VictoryScene";

    [Header("Интерфейс")]
    public TextMeshProUGUI counterText;

    [Header("Настройки сложности")]
    public AudioSource strangeSoundSource;
    public AudioClip[] strangeSoundClips;
    public GameObject monsterPrefab;
    public Transform[] monsterSpawnPoints;
    public AlarmSystem alarmSystem;

    [HideInInspector]
    public int collectedNotes = 0;

    private bool strangeSoundPlayed = false;
    private bool monsterSpawned = false;
    private bool alarmTriggered = false;
    private int lastNoteCount = 0;

    void Start()
    {
        UpdateCounterUI();
        lastNoteCount = collectedNotes;
    }

    void Update()
    {
        if (collectedNotes != lastNoteCount)
        {
            OnNotesCollected();
            lastNoteCount = collectedNotes;
        }
    }

    public void AddNote()
    {
        collectedNotes++;
        UpdateCounterUI();
        Debug.Log($"[Сложность] Собрано частей: {collectedNotes} / {totalNotesNeeded}");
    }

    void UpdateCounterUI()
    {
        if (counterText != null)
        {
            counterText.text = $"Частей студенческого: {collectedNotes} / {totalNotesNeeded}";
        }
    }

    void OnNotesCollected()
    {
        /*
        if (collectedNotes >= 2 && !strangeSoundPlayed)
        {
            PlayStrangeSounds();
            strangeSoundPlayed = true;
        }
        */

        if (collectedNotes >= 3 && !monsterSpawned)
        {
            SpawnMonster();
            monsterSpawned = true;
        }

        /*
        if (collectedNotes >= 4 && !alarmTriggered)
        {
            TriggerAlarm();
            alarmTriggered = true;
        }
        */
        if (collectedNotes >= 5)
        {
            MaxDifficulty();
        }
    }

    void PlayStrangeSounds()
    {
        Debug.Log("[Сложность] Странные звуки!");

        if (strangeSoundSource != null && strangeSoundClips != null && strangeSoundClips.Length > 0)
        {
            AudioClip randomClip = strangeSoundClips[Random.Range(0, strangeSoundClips.Length)];
            strangeSoundSource.PlayOneShot(randomClip, 0.7f);
        }
    }

    void SpawnMonster()
    {
        Debug.Log("[Сложность] Появление монстра!");

        if (monsterPrefab == null)
        {
            Debug.LogError("Monster Prefab не назначен!");
            return;
        }

        if (monsterSpawnPoints == null || monsterSpawnPoints.Length == 0)
        {
            Debug.LogError("Нет точек спавна монстра! Добавьте точки в массив Monster Spawn Points");
            return;
        }

        // ВЫБИРАЕМ СЛУЧАЙНУЮ ТОЧКУ ИЗ МАССИВА
        int randomIndex = Random.Range(0, monsterSpawnPoints.Length);
        Transform chosenSpawnPoint = monsterSpawnPoints[randomIndex];

        Debug.Log($"[Сложность] Монстр появляется в точке {randomIndex}: {chosenSpawnPoint.position}");

        // СОЗДАЁМ МОНСТРА В ВЫБРАННОЙ ТОЧКЕ
        Instantiate(monsterPrefab, chosenSpawnPoint.position, chosenSpawnPoint.rotation);
    }

    void TriggerAlarm()
    {
        Debug.Log("[Сложность] Включение всех сирен!");

        AlarmSystem[] allAlarms = FindObjectsByType<AlarmSystem>(FindObjectsSortMode.None);

        if (allAlarms.Length > 0)
        {
            foreach (AlarmSystem alarm in allAlarms)
            {
                alarm.ForceActivateAlarm();
            }
            Debug.Log($"[Сложность] Включено сирен: {allAlarms.Length}");
        }
        else
        {
            Debug.LogWarning("[Сложность] Сирены не найдены!");
        }
    }

    void MaxDifficulty()
    {
        Debug.Log("[Сложность] Максимальная сложность!");
    }

    public void TryEscape()
    {
        if (collectedNotes >= totalNotesNeeded)
        {
            Debug.Log("Победа! Останавливаем музыку...");
            
            GameMusicManager musicManager = FindObjectOfType<GameMusicManager>();
            if (musicManager != null)
                musicManager.StopMusic();
            
            SceneManager.LoadScene("VictoryCutscene");
        }
        else
        {
            Debug.Log($"Собрано: {collectedNotes} из {totalNotesNeeded}");
        }
    }
}