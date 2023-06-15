using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyWave;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public bool isSpawning;

    public GameObject smallEnemyPrefab;
    public GameObject bigEnemyPrefab;
    public GameObject siegeEnemyPrefab;
    public GameObject flyingEnemyPrefab;
    public GameObject lightningPrefab;

    public GameObject leftPortal;
    public GameObject rightPortal;
    public int spawnTime;



    void SpawnEnemy(GameObject prefab)
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private void Awake()
    {
        //StartCoroutine(SpawnContinuously());
        if(instance == null)instance= this;
        PlayerAnalizer.OnNewWave +=  SpawnWave;

    }

    void SpawnWave(EnemyWave wave) {
        Debug.Log("TEste");
        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    IEnumerator SpawnWaveCoroutine(EnemyWave wave) {
        isSpawning = true;
        float passedTime = 0;

        while(passedTime < wave.m_duration) {
            //Enemies
            List<Spawn> removeFromList = new List<Spawn>();
            int spawnListLength = wave.spawns.Count;
            for (int i = 0; i < spawnListLength; i++){
                if (wave.spawns[i].m_time < passedTime) {
                    SpawnEnemy(wave.spawns[i]);
                    removeFromList.Add(wave.spawns[i]);
                }
            }
            foreach(var rem in removeFromList) {
                wave.spawns.Remove(rem);
            }

            //Lightnings
            List<LightningSpawn> removeFromLightningList = new List<LightningSpawn>();
            int lightningSpawnListLength = wave.lightnings.Count;
            for (int i = 0; i < lightningSpawnListLength; i++) {
                if (wave.lightnings[i].m_time < passedTime) {
                    SpawnLightning(wave.lightnings[i]);
                    removeFromLightningList.Add(wave.lightnings[i]);
                }
            }
            foreach (var rem in removeFromLightningList) {
                wave.lightnings.Remove(rem);
            }

            //Other
            passedTime += Time.deltaTime;
            yield return null;
        }
        isSpawning = false;
    }

    void SpawnLightning(LightningSpawn lightningSpawn) {
        Instantiate(lightningPrefab, lightningSpawn.m_position, Quaternion.identity);
    }


    void SpawnEnemy(EnemyWave.Spawn spawn) {
        GameObject prefab;
        switch(spawn.m_type) {
            case EnemyWave.EnemyType.Small:     prefab = smallEnemyPrefab;  break;
            case EnemyWave.EnemyType.Big:       prefab = bigEnemyPrefab;    break;
            case EnemyWave.EnemyType.Flying:    prefab = flyingEnemyPrefab; break;
            case EnemyWave.EnemyType.Siege:     prefab = siegeEnemyPrefab;  break;
            default: prefab= null; Debug.LogError("Trying to spawn an Enemy without Type"); return;
        }
        if(spawn.m_side) Instantiate(prefab, leftPortal.transform.position, Quaternion.identity);
        else Instantiate(prefab, rightPortal.transform.position, Quaternion.identity);
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
