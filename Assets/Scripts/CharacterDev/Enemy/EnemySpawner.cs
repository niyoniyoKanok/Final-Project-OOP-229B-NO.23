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
    [SerializeField] private float minSpawnDistance = 6f;   
    [SerializeField] private float maxSpawnDistance = 12f;  
    [SerializeField] private float spawnYOffset = 0.1f;     

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

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleMask; 
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

       
        if (timer.GetRemainingTime() <= 0) return;

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

        float t = 1f - (timer.GetRemainingTime() / timer.MaxTime);
        t = Mathf.Clamp01(t);

        float curve = difficulty.difficultyByTime.Evaluate(t);

        currentSpawnInterval = Mathf.Lerp(difficulty.baseSpawnInterval, difficulty.minSpawnInterval, curve);
        currentSpawnAmount = Mathf.RoundToInt(Mathf.Lerp(difficulty.baseSpawnAmount, difficulty.maxSpawnAmount, curve));
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
        if (spawnPos == Vector2.zero) return;

        Enemy prefab = floodOverridePrefab != null
            ? floodOverridePrefab
            : enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Enemy newEnemy = EnemyPool.Instance.Get(prefab, (Vector3)spawnPos, Quaternion.identity);

        if (Random.value < baseEliteChance)
        {
            
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


    private Vector2 GetSpawnPosition()
    {
        float safeZone = 4f;

        for (int i = 0; i < 25; i++)
        {
            float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
            float angle = Random.Range(0f, 360f);

            Vector2 offset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * dist;

            Vector2 candidate = (Vector2)player.position + offset;

          
            if (Vector2.Distance(candidate, player.position) < safeZone)
                continue;

           
            if (Physics2D.OverlapCircle(candidate, 0.5f, obstacleMask))
                continue;

            Vector2 rayStart = new Vector2(candidate.x, candidate.y + 8f);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 20f, groundLayer);

            if (hit.collider != null)
            {
                float y = hit.point.y + spawnYOffset;
                return new Vector2(candidate.x, y);
            }
        }

        // fallback
        return new Vector2(player.position.x + minSpawnDistance, player.position.y + 1f);
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
