using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int enemyCount;
        public float spawnRate;
        public float timeBeforeWave;
    }

    [Header("Настройки префаба")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("Настройки волн")]
    public List<Wave> waves;

    [Header("Настройки UI")]
    public TextMeshProUGUI enemyCounterText;

    [Header("Статистика (Только для чтения)")]
    public int enemiesRemainingToSpawn;
    public int enemiesAliveNow;

    private int _currentWaveIndex = 0;

    private void Start()
    {
        UpdateUI();
        if (waves != null && waves.Count > 0)
        {
            StartCoroutine(SpawnWaveRoutine());
        }
    }

    IEnumerator SpawnWaveRoutine()
    {
        while (_currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[_currentWaveIndex];
            enemiesRemainingToSpawn = currentWave.enemyCount;

            yield return new WaitForSeconds(currentWave.timeBeforeWave);

            for (int i = 0; i < currentWave.enemyCount; i++)
            {
                SpawnOneEnemy();
                enemiesRemainingToSpawn--;
                yield return new WaitForSeconds(currentWave.spawnRate);
            }

            while (enemiesAliveNow > 0)
            {
                yield return null;
            }

            _currentWaveIndex++;
        }

        if (enemyCounterText != null) enemyCounterText.text = "ВСЕ ВОЛНЫ ПРОЙДЕНЫ!";
    }

    void SpawnOneEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, sp.position, sp.rotation);

        enemiesAliveNow++;
        UpdateUI();

        if (enemy.TryGetComponent<HealthSystem>(out var health))
        {
            health.OnDeath.AddListener(() => {
                enemiesAliveNow--;
                UpdateUI();
            });
        }
    }

    void UpdateUI()
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = $"Врагов на карте: {enemiesAliveNow}";
        }
    }

    public void KillAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            if (e.TryGetComponent<HealthSystem>(out var health))
                health.TakeDamage(999999);
            else
                Destroy(e);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemySpawner spawner = (EnemySpawner)target;

        GUILayout.Space(15);
        GUI.backgroundColor = Color.red;

        if (GUILayout.Button("УБИТЬ ВСЕХ ВРАГОВ (KILL ALL)", GUILayout.Height(40)))
        {
            spawner.KillAllEnemies();
        }

        GUI.backgroundColor = Color.white;
    }
}
#endif