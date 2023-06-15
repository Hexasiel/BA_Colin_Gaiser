using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    [SerializeField] Light2D globalLight;
    [SerializeField] SpriteRenderer[] backGround;

    [SerializeField] Color nightColor;
    [SerializeField] Color dayColor;
    [SerializeField] float dayIntensity;
    [SerializeField] float nightIntensity;
    [SerializeField] float nightDayFadeDuration;

    void SpawnEnemy(GameObject prefab)
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private void Awake()
    {
        //StartCoroutine(SpawnContinuously());
        if(instance == null)instance= this;
        PlayerAnalizer.OnNewWave +=  SpawnWave;
        Enemy.OnAllEnemiesDefeated += FadeToDay;
    }

    void SpawnWave(EnemyWave wave) {
        Debug.Log("TEste");
        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    void FadeToDay() {
        StartCoroutine(FadeDaytime(dayColor, dayIntensity, nightDayFadeDuration));
    }

    IEnumerator SpawnWaveCoroutine(EnemyWave wave) {
        isSpawning = true;
        float passedTime = 0;

        StartCoroutine(FadeDaytime(nightColor, nightIntensity, nightDayFadeDuration));

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

    IEnumerator FadeDaytime(Color newColor, float newIntesity, float fadeDuration) {
        float passedTime = 0f;
        Color currentColor = backGround[0].color;
        float currentIntensity = globalLight.intensity;
        while(passedTime < fadeDuration) {

            globalLight.intensity = Mathf.Lerp(currentIntensity, newIntesity, passedTime / fadeDuration);
            foreach(SpriteRenderer bg in backGround)
                bg.color = Color.Lerp(currentColor, newColor, passedTime / fadeDuration);
            passedTime += Time.deltaTime;
            yield return null;
        }
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

}
