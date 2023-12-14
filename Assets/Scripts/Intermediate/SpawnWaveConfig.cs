using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy Wave Config")]

public class SpawnWaveConfig : ScriptableObject
{
    [SerializeField] private GameObject enemy, spawnPoints;
    [SerializeField] private float timeBetweenSpawns, timeBetweenWaves, spawnRandomness;
    [SerializeField] private int numberOfEnemies;


    public List<Transform> GetSpawnPoint()
    {
        List<Transform> waveSpawnPosition = new List<Transform>();
        foreach (Transform child in spawnPoints.transform)
        {
            waveSpawnPosition.Add(child);
        }
        return waveSpawnPosition;
    }

    public GameObject GetEnemyPrefab() { return enemy; }
    public float GetTimeBetweenSpawns() { return timeBetweenSpawns; }
    public float GetTimeBetweenWaves() { return timeBetweenWaves; }
    public float GetSpawnRandomness() { return spawnRandomness; }
    public float GetNumberOfEnemies() { return numberOfEnemies; }

}
