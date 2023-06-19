using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAnalizer : MonoBehaviour
{
    WavePerformance currentWavePerformance;
    List<WavePerformance> pastWavePerformanceList;
    public static event Action<EnemyWave> OnNewWave;

    private void Start() {
        pastWavePerformanceList = new List<WavePerformance>();
        Enemy.OnAllEnemiesDefeated += CheckForClearedWave;
        NextWave();
    }

    IEnumerator WrapUpWave() {
        currentWavePerformance.WrapUp();
        pastWavePerformanceList.Add(currentWavePerformance);
        yield return new WaitForSeconds(currentWavePerformance.m_wavePauseDuration);
        NextWave();
    }

    void CheckForClearedWave() {
        if(EnemySpawner.instance.isSpawning == false) {
            StartCoroutine(WrapUpWave());
        }
    }

    void NextWave(){
        float difficulty = CalculateNewDifficulty();
        EnemyWave newWave = new EnemyWave(difficulty);
        OnNewWave?.Invoke(newWave);
        currentWavePerformance = new WavePerformance();
        currentWavePerformance.m_wavePauseDuration = CalculatePauseDuration();
        currentWavePerformance.m_waveDifficulty = difficulty;
    }

    float CalculateNewDifficulty(){
        float difficulty = 0;

        //Placeholder
        if(pastWavePerformanceList.Count > 0) {
            difficulty = pastWavePerformanceList.Last<WavePerformance>().m_waveDifficulty + 0.4f;
        }
        else {
            difficulty = 3.4f;
        }
        return difficulty;
    }

    float CalculateNewDifficultyBCI(){
        float difficulty = 0;

        return difficulty;
    }

    float CalculateNewDifficultyGameMetrics(){
        float difficulty = 0;

        return difficulty;
    }

    float CalculatePauseDuration() {
        //Placeholder
        float duration = 20;
        return duration;
    }
}
