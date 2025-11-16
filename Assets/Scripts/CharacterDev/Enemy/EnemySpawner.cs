using System.Collections;
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

    [Header("Difficulty Settings")]
    [SerializeField] private SpawnDifficulty difficulty;
    [SerializeField] private Timer timer;

    private float currentSpawnInterval;
    private int currentSpawnAmount;

    private float timerMaxTime;

    void Start()
    {
        if (timer != null)
            timerMaxTime = timer.GetRemainingTime();

        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        if (timer == null || difficulty == null) return;

        float t = 1f - (timer.GetRemainingTime() / timerMaxTime);
        float curve = difficulty.difficultyByTime.Evaluate(t);

        currentSpawnInterval =
            Mathf.Lerp(difficulty.baseSpawnInterval, difficulty.minSpawnInterval, curve);

        currentSpawnAmount =
            Mathf.RoundToInt(Mathf.Lerp(difficulty.baseSpawnAmount, difficulty.maxSpawnAmount, curve));
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
        Instantiate(prefab, spawnPos, Quaternion.identity);
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

}