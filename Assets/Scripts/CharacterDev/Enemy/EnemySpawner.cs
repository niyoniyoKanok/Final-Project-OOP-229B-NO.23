using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private float minDistanceFromPlayer = 2f;

    [Header("Enemy Prefabs")]
    [SerializeField] private Enemy[] enemyPrefabs;

    [Header("Difficulty & Progression")]
    [SerializeField] private SpawnDifficulty difficulty;
    [SerializeField] private Timer timer;

    [Header("Elite Settings")]
    [Range(0f, 1f)][SerializeField] private float baseEliteChance = 0.05f;
    [SerializeField] private float eliteSizeMultiplier = 1.5f;

    [Header("Boss Events")]
    [SerializeField] private List<BossSpawnEvent> bossEvents;

   
    [Header("Overtime Scaling")]
    [Tooltip("ทุกๆ 10 วินาทีใน Overtime จะเพิ่มจำนวนมอนสเตอร์กี่ตัว")]
    [SerializeField] private float overtimeSpawnIncreaseRate = 0.5f;
    [Tooltip("เวลาเกิดต่ำสุดที่เป็นไปได้ในช่วง Overtime")]
    [SerializeField] private float minOvertimeSpawnInterval = 0.1f;

    private float currentSpawnInterval;
    private int currentSpawnAmount;
    private float elapsedTime;

    void Start()
    {
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
    private void CheckBossSpawns()
    {
        for (int i = 0; i < bossEvents.Count; i++)
        {
            var bossEvent = bossEvents[i];

            
            if (!bossEvent.hasSpawned && elapsedTime >= bossEvent.spawnTime)
            {
                SpawnBoss(bossEvent.bossPrefab);

                // Mark as spawned
                bossEvent.hasSpawned = true;
                bossEvents[i] = bossEvent;

                Debug.Log($"Spawned Boss: {bossEvent.bossName} at {elapsedTime}s");
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
        if (player == null || enemyPrefabs.Length == 0) return;

        Vector2 spawnPos = GetSpawnPosition();

       
        Enemy prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

       
        Enemy newEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);

       
        if (Random.value < baseEliteChance)
        {
            MakeElite(newEnemy);
        }
    }

    private void SpawnBoss(GameObject bossPrefab)
    {
        Vector2 spawnPos = GetSpawnPosition();
        Instantiate(bossPrefab, spawnPos, Quaternion.identity);
    }

    private void MakeElite(Enemy enemy)
    {
        enemy.transform.localScale *= eliteSizeMultiplier;
        enemy.MaxHealth *= 3;
        enemy.Health = enemy.MaxHealth;
        enemy.DamageHit *= 2;

        enemy.xpDrop = 30;

     
        Color goldColor = new Color(1f, 0.84f, 0f);
        enemy.SetCharacterColor(goldColor);
    }

    [SerializeField] private LayerMask groundLayer;

    private Vector2 GetSpawnPosition()
    {
        Vector2 finalPos = player.position;
        int safety = 20;

        while (safety-- > 0)
        {
           
            Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector2 candidatePos = (Vector2)player.position + new Vector2(offset.x, 0);

           
            RaycastHit2D hit = Physics2D.Raycast(candidatePos + Vector2.up * 3f, Vector2.down, 10f, groundLayer);

            if (hit.collider != null)
            {
                finalPos = hit.point;

                if (Vector2.Distance(finalPos, player.position) >= minDistanceFromPlayer)
                    return finalPos;
            }
        }

       
        return player.position + Vector3.right * (minDistanceFromPlayer + 1f);
    }

    [System.Serializable]
    public struct BossSpawnEvent
    {
        public string bossName;
        public GameObject bossPrefab;
        public float spawnTime;
        [HideInInspector] public bool hasSpawned;
    }

}