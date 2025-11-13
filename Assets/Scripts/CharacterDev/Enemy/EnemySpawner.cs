using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameDurationInMinutes = 15f;
    private float gameTimer; 

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform; 

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs; 

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnRadius = 10f; 
    [SerializeField] private float maxSpawnRadius = 15f; 

 
    [Header("Difficulty Curves")]
   
    [SerializeField] private AnimationCurve spawnIntervalCurve;

    [SerializeField] private AnimationCurve spawnQuantityCurve;

    private float spawnTimer;

    void Start()
    {
        
        gameTimer = gameDurationInMinutes * 60f;

      
        if (playerTransform == null)
        {
         
            playerTransform = FindFirstObjectByType<Prince>().transform;
        }
    }

    void Update()
    {
       
        if (gameTimer <= 0f)
        {
            Debug.Log("Game Over! You Survived!");
           
            enabled = false;
            return;
        }

      
        gameTimer -= Time.deltaTime;

       
        spawnTimer -= Time.deltaTime;

       
        if (spawnTimer <= 0f)
        {
         
            SpawnEnemies();

          
            float gameProgress = 1f - (gameTimer / (gameDurationInMinutes * 60f));

        
            spawnTimer = spawnIntervalCurve.Evaluate(gameProgress);
        }
    }

    void SpawnEnemies()
    {
        
        float gameProgress = 1f - (gameTimer / (gameDurationInMinutes * 60f));

  
        int quantity = Mathf.RoundToInt(spawnQuantityCurve.Evaluate(gameProgress));

       
        for (int i = 0; i < quantity; i++)
        {
            
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 spawnPosition = playerTransform.position + (Vector3)(randomDirection * randomDistance);

           
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}