using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private FloodWarningUI floodUI;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDistance = 6f;   // ใกล้สุดจาก player
    [SerializeField] private float maxSpawnDistance = 12f;  // ไกลสุดจาก player
    [SerializeField] private float spawnYOffset = 0.1f;     // ยกเหนือพื้นเล็กน้อย

    [Header("Enemy Prefabs")]
    [SerializeField] private Enemy[] enemyPrefabs;

    [Header("Difficulty Settings")]
    [SerializeField] private SpawnDifficulty difficulty;
    [SerializeField] private Timer timer;

    [Header("Elite Settings")]
    [Range(0f, 1f)][SerializeField] private float baseEliteChance = 0.05f;
    [SerializeField] private float eliteSizeMultiplier = 1.5f;

    [Header("Boss Events")]
    [SerializeField] private List<BossSpawnEvent> bossEvents;

    [Header("Flood Events")]
    [SerializeField] private List<FloodEvent> floodEvents;
    private Enemy floodOverridePrefab;

    [Header("Overtime Scaling")]
    [SerializeField] private float overtimeSpawnIncreaseRate = 0.5f;
    [SerializeField] private float minOvertimeSpawnInterval = 0.1f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleMask; // for obstacle checks

    private float currentSpawnInterval;
    private int currentSpawnAmount;

    private float elapsedTime;
    private bool isWarningShown = false;
    public bool IsFlooding { get; private set; }

    void Start()
    {
        if (difficulty != null)
        {
            currentSpawnInterval = difficulty.baseSpawnInterval;
            currentSpawnAmount = difficulty.baseSpawnAmount;
        }

        if (player == null)
        {
            var p = FindFirstObjectByType<Prince>();
            if (p != null) player = p.transform;
        }

        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        if (timer == null) return;

        elapsedTime = timer.GetTotalElapsedTime();

        UpdateDifficulty();
        CheckBossSpawns();
    }

    private void UpdateDifficulty()
    {
        if (difficulty == null) return;

        IsFlooding = CheckFloodStatus();

        if (IsFlooding)
        {
            if (!isWarningShown)
            {
                floodUI?.ShowWarning("FLOODED!!");
                isWarningShown = true;
            }
            return;
        }
        else
        {
            if (isWarningShown)
            {
                floodUI?.HideWarning();
                isWarningShown = false;
            }
        }

        floodOverridePrefab = null;

        if (!timer.IsOvertime)
        {
            float t = 1f - (timer.GetRemainingTime() / timer.MaxTime);
            float curve = difficulty.difficultyByTime.Evaluate(t);

            currentSpawnInterval = Mathf.Lerp(difficulty.baseSpawnInterval, difficulty.minSpawnInterval, curve);
            currentSpawnAmount = Mathf.RoundToInt(Mathf.Lerp(difficulty.baseSpawnAmount, difficulty.maxSpawnAmount, curve));
        }
        else
        {
            float extraDifficulty = timer.OvertimeDuration * overtimeSpawnIncreaseRate;

            currentSpawnAmount = difficulty.maxSpawnAmount + Mathf.FloorToInt(extraDifficulty);
            currentSpawnInterval = Mathf.Max(minOvertimeSpawnInterval, difficulty.minSpawnInterval - (timer.OvertimeDuration * 0.001f));
        }
    }

    private bool CheckFloodStatus()
    {
        foreach (var flood in floodEvents)
        {
            if (elapsedTime >= flood.startTime && elapsedTime < flood.startTime + flood.duration)
            {
                currentSpawnInterval = flood.spawnInterval;
                currentSpawnAmount = flood.spawnAmount;
                floodOverridePrefab = flood.specificEnemy;
                return true;
            }
        }
        return false;
    }

    private void CheckBossSpawns()
    {
        for (int i = 0; i < bossEvents.Count; i++)
        {
            var evt = bossEvents[i];
            if (!evt.hasSpawned && elapsedTime >= evt.spawnTime)
            {
                SpawnBoss(evt.bossPrefab);

                evt.hasSpawned = true;
                bossEvents[i] = evt;

                Debug.Log($"Spawned Boss: {evt.bossName} at {elapsedTime}s");
            }
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            for (int i = 0; i < currentSpawnAmount; i++)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        if (player == null || enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        Vector2 spawnPos = GetSpawnPosition();
        if (spawnPos == Vector2.zero) return; // failed to find

        Enemy prefab = floodOverridePrefab != null
            ? floodOverridePrefab
            : enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Enemy newEnemy = EnemyPool.Instance.Get(prefab, (Vector3)spawnPos, Quaternion.identity);

        if (Random.value < baseEliteChance)
        {
            // apply elite after spawn (since ResetState was called)
            newEnemy.transform.localScale *= eliteSizeMultiplier;
            newEnemy.MaxHealth *= 3;
            newEnemy.Health = newEnemy.MaxHealth;
            newEnemy.DamageHit *= 2;
            newEnemy.xpDrop = 30;
            newEnemy.SetCharacterColor(new Color(1f, 0.84f, 0f));
        }
    }

    private void SpawnBoss(GameObject bossPrefab)
    {
        Vector2 spawnPos = GetSpawnPosition();
        if (spawnPos == Vector2.zero) return;
        Instantiate(bossPrefab, spawnPos, Quaternion.identity);
    }

    // find spawn point in ring around player, ensure ground exists below, avoid obstacles
    private Vector2 GetSpawnPosition()
    {
        for (int i = 0; i < 20; i++)
        {
            float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
            float angle = Random.Range(0f, 360f);
            Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * dist;

            Vector2 candidate = (Vector2)player.position + offset;

            // check obstacles (small circle)
            if (Physics2D.OverlapCircle(candidate, 0.4f, obstacleMask)) continue;

            // raycast downwards to find ground
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(candidate.x, player.position.y + 10f), Vector2.down, 30f, groundLayer);
            if (hit.collider != null)
            {
                return new Vector2(candidate.x, hit.point.y + spawnYOffset);
            }
        }

        // fallback: try a simple offset to right if nothing found
        RaycastHit2D fallbackHit = Physics2D.Raycast(new Vector2(player.position.x + minSpawnDistance, player.position.y + 10f), Vector2.down, 30f, groundLayer);
        if (fallbackHit.collider != null)
            return new Vector2(player.position.x + minSpawnDistance, fallbackHit.point.y + spawnYOffset);

        return Vector2.zero;
    }

    [System.Serializable]
    public struct BossSpawnEvent
    {
        public string bossName;
        public GameObject bossPrefab;
        public float spawnTime;
        [HideInInspector] public bool hasSpawned;
    }

    [System.Serializable]
    public struct FloodEvent
    {
        public string eventName;
        public float startTime;
        public float duration;
        public float spawnInterval;
        public int spawnAmount;
        public Enemy specificEnemy;
    }
}
