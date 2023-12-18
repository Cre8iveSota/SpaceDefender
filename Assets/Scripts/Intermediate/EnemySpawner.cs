using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnWaveConfig> listOfConfig = new List<SpawnWaveConfig>();
    [SerializeField] private int startingWave = 0;
    private bool waveActive = true;
    private int waveRepeatCount = 0;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        // waveActive = false;
        do { yield return StartCoroutine("SpaewnAllWaves"); waveRepeatCount++; }
        while (waveActive);
    }

    IEnumerator SpaewnAllWaves()
    {
        for (int i = startingWave; i < listOfConfig.Count; i++)
        {
            SpawnWaveConfig currentWave = listOfConfig[i];
            yield return StartCoroutine(SpawnEnemiesInWave(currentWave));
        }
    }

    IEnumerator SpawnEnemiesInWave(SpawnWaveConfig waveConfig)
    {
        //wave Increase the number of enemies with every count.
        for (int i = 0; i < waveConfig.GetNumberOfEnemies() + waveRepeatCount; i++)
        {
            GameObject enemyInstance = Instantiate
            (
                waveConfig.GetEnemyPrefab()
                , waveConfig.GetSpawnPoint()[Random.Range(0, waveConfig.GetSpawnPoint().Count)].position
                , Quaternion.identity);
            // Increases enemy speed by 25% for each additional count
            enemyInstance.GetComponent<Enemy>().speed += waveRepeatCount * 0.25f;
            enemyInstance.GetComponent<Enemy>().enemyScore = enemyInstance.GetComponent<Enemy>().enemyScore * (waveRepeatCount + 1);
            // Reduce the interval between enemies that appear each time the count increases.
            float changedTimeBetweenSpans = Mathf.Max(0, waveConfig.GetTimeBetweenSpawns() - waveRepeatCount * 0.25f);

            yield return new WaitForSeconds(
                Random.Range(
                  changedTimeBetweenSpans
                , changedTimeBetweenSpans + waveConfig.GetSpawnRandomness())
                );
        }

        yield return new WaitForSeconds(Mathf.Max(0, waveConfig.GetTimeBetweenWaves() - waveRepeatCount * 0.25f));
    }

}
