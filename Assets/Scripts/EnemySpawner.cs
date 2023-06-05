using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject smallEnemyPrefab;
    public Enemy test;
    public int spawnTime;

    void SpawnEnemy(GameObject prefab)
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private void Start()
    {
        StartCoroutine(SpawnContinuously());
    }

    IEnumerator SpawnContinuously()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawnTime);
            SpawnEnemy(smallEnemyPrefab);
        }

    }
}
