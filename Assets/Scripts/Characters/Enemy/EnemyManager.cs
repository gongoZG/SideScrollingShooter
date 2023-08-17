using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人管理器，控制敌人的生成与波数
/// </summary>
public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject RandomEnemy => enemyList.Count == 0 ? 
        null : enemyList[Random.Range(0, enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;
    
    [Header("Wave")]
    [SerializeField] bool spawnEnemy = true;
    [SerializeField] GameObject waveUI;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] float timeBetweenSpawns = 1f;
    [SerializeField] float timeBetweenWaves = 1f;
    [SerializeField] int minEnemyAmount = 4;
    [SerializeField] int maxEnemyAmount = 10;

    [Header("Boss")]
    [SerializeField] GameObject bossPrefab;
    [SerializeField] int bossWaveNumber;

    int waveNumber = 1;
    int enemyAmount;
    List<GameObject> enemyList;
    WaitForSeconds waitTimeBetweenSpawns;
    WaitForSeconds waitTimeBetweenWaves;
    WaitUntil waitUntilNoEnemy;

    protected override void Awake() {
        base.Awake();
        enemyList = new List<GameObject>();
        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
        waitUntilNoEnemy = new WaitUntil(() => enemyList.Count == 0);
    }

    private IEnumerator Start() {
        // 使敌人持续生成的方法：start中写一个死循环，为了完成间隔一定时间，将start改为携程
        while (spawnEnemy && GameManager.GameState != GameState.GameOver) {
            // yield return waitUntilNoEnemy; 
            // 若在此处挂起等待，有可能会出现生成了一个敌人的瞬间被玩家秒掉，从而直接进入下一波的情况 
            waveUI.gameObject.SetActive(true);
            yield return waitTimeBetweenWaves;
            waveUI.gameObject.SetActive(false);
            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
    }

    /// <summary>
    /// 每一波中随机生成敌人或 Boss
    /// </summary>
    /// <returns></returns>
    IEnumerator RandomlySpawnCoroutine() {
        if (waveNumber % bossWaveNumber == 0) {
            var boss = PoolManager.Release(bossPrefab);
            enemyList.Add(boss);
        }
        else {
            enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / bossWaveNumber, maxEnemyAmount);
            for (int i = 0; i < enemyAmount; i++) {
                enemyList.Add(PoolManager.Release(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
                yield return waitTimeBetweenSpawns;
            }
        }
        yield return waitUntilNoEnemy;  // 生成完毕后才挂起等待
        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);
    
}
