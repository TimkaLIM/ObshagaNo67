using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApparitionManager : MonoBehaviour
{
    [Header("Настройки")]
    public Apparition[] apparitionPrefabs;
    public Transform[] spawnPoints;          // ← РУЧНЫЕ ТОЧКИ!
    public float minSpawnInterval = 8f;
    public float maxSpawnInterval = 20f;
    public int maxActiveApparitions = 2;

    [Header("Ссылка на EscapeDoor")]
    public EscapeDoor escapeDoor;

    private bool isSystemActive = false;
    private List<Apparition> activeApparitions = new List<Apparition>();
    private Coroutine spawnRoutine;

    void Start()
    {
        if (escapeDoor == null)
            escapeDoor = FindObjectOfType<EscapeDoor>();

        if (apparitionPrefabs == null || apparitionPrefabs.Length == 0)
            Debug.LogWarning("[Manager] Нет префабов силуэтов!");

        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning("[Manager] Нет точек спавна!");
    }

    void Update()
    {
        if (escapeDoor == null) return;

        int collected = escapeDoor.collectedNotes;

        if (collected >= 2 && !isSystemActive)
        {
            ActivateSystem();
        }

        if (isSystemActive && collected < 2)
        {
            DeactivateSystem();
        }
    }

    void ActivateSystem()
    {
        isSystemActive = true;
        Debug.Log("[Manager] Система активирована!");

        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    void DeactivateSystem()
    {
        isSystemActive = false;
        Debug.Log("[Manager] Система деактивирована.");

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        foreach (Apparition app in activeApparitions)
        {
            if (app != null)
                Destroy(app.gameObject);
        }
        activeApparitions.Clear();
    }

    IEnumerator SpawnRoutine()
    {
        while (isSystemActive)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            CleanupApparitions();

            if (activeApparitions.Count >= maxActiveApparitions)
                continue;

            if (apparitionPrefabs == null || apparitionPrefabs.Length == 0)
                continue;

            if (spawnPoints == null || spawnPoints.Length == 0)
                continue;

            // Выбираем случайную точку из списка
            int pointIndex = Random.Range(0, spawnPoints.Length);
            Transform chosenPoint = spawnPoints[pointIndex];

            // Выбираем случайный префаб
            int prefabIndex = Random.Range(0, apparitionPrefabs.Length);
            Apparition prefab = apparitionPrefabs[prefabIndex];

            // Создаём силуэт в выбранной точке
            Apparition newApparition = Instantiate(prefab, chosenPoint.position, chosenPoint.rotation);
            newApparition.Activate();
            activeApparitions.Add(newApparition);

            Debug.Log($"[Manager] Силуэт появился в точке {pointIndex}: {chosenPoint.position}");
        }
    }

    void CleanupApparitions()
    {
        for (int i = activeApparitions.Count - 1; i >= 0; i--)
        {
            if (activeApparitions[i] == null || activeApparitions[i].hasDisappeared)
            {
                if (activeApparitions[i] != null)
                    Destroy(activeApparitions[i].gameObject);
                activeApparitions.RemoveAt(i);
            }
        }
    }
}