using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnWaveConfig> listOfConfig = new List<SpawnWaveConfig>();
    [SerializeField] private int startingWave = 0;
    public bool waveActive = true;
    private int waveRepeatCount = 0;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject smallBoss;

    // [SerializeField] private GameObject bossSpawnPoint;
    bool isBossExist = false;
    bool isSmallBossExist = false;

    [SerializeField] private GameObject bossHealthUI;

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
            if (GameManager.totalamount > 10000 && !isBossExist)
            {
                isBossExist = true;
                waveActive = false;
                boss.SetActive(true);
                // Instantiate(bossPrefab, bossSpawnPoint.transform.position, Quaternion.identity);
                bossHealthUI.SetActive(true);
                yield return new WaitForSeconds(5f);
                waveActive = true;
                waveRepeatCount = 2;
            }
            if (GameManager.totalamount > 5000 && !isSmallBossExist)
            {
                isSmallBossExist = true;
                waveActive = false;
                smallBoss.SetActive(true);
                // Instantiate(bossPrefab, bossSpawnPoint.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(10f);
                waveActive = true;
                waveRepeatCount = 5;
            }
            SpawnWaveConfig currentWave = listOfConfig[i];
            yield return StartCoroutine(SpawnEnemiesInWave(currentWave));
            if (!waveActive) break;
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
