using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    private Dictionary<string, Queue<Enemy>> poolDictionary = new Dictionary<string, Queue<Enemy>>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public Enemy Get(Enemy prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary.Add(key, new Queue<Enemy>());
        }

        Enemy result = null;

        if (poolDictionary[key].Count > 0)
        {
            result = poolDictionary[key].Dequeue();

            if (result == null) // safety
                return Get(prefab, position, rotation);

            // set transform BEFORE activating
            result.transform.SetPositionAndRotation(position, rotation);

            // reset all runtime state BEFORE enabling (very important)
            result.ResetState();

            // then activate
            result.gameObject.SetActive(true);

            return result;
        }
        else
        {
            // instantiate new if none in pool
            Enemy newEnemy = Instantiate(prefab, position, rotation);
            newEnemy.name = key;

            // ensure it's initialized properly on first spawn
            newEnemy.ResetState();
            newEnemy.gameObject.SetActive(true);

            return newEnemy;
        }
    }

    public void ReturnToPool(Enemy enemy)
    {
        if (enemy == null) return;

        string key = enemy.name;

        // Disable first (this will call OnDisable/OnDisable routines in enemy if any)
        enemy.gameObject.SetActive(false);

        // Enqueue to pool
        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary.Add(key, new Queue<Enemy>());
        }

        poolDictionary[key].Enqueue(enemy);
    }
}
