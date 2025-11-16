using UnityEngine;

[CreateAssetMenu(fileName = "SpawnDifficulty", menuName = "Scriptable Objects/SpawnDifficulty")]
public class SpawnDifficulty : ScriptableObject
{
    [Header("Spawn Settings")]
    public float baseSpawnInterval = 3f;
    public float minSpawnInterval = 0.8f;

    public int baseSpawnAmount = 1;
    public int maxSpawnAmount = 6;

    [Header("Difficulty Curve")]
    public AnimationCurve difficultyByTime;
}