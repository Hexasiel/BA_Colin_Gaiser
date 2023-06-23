using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class PlayerAnalizer : MonoBehaviour
{

    public static event Action<EnemyWave> OnNewWave;

    int gameSessionID;
    WavePerformance currentWavePerformance;
    List<WavePerformance> pastWavePerformanceList;

    private float systemWeight;

    private void Start() {
        pastWavePerformanceList = new List<WavePerformance>();
        gameSessionID = GetNewGameSessionID();
        Enemy.OnAllEnemiesDefeated += CheckForClearedWave;
        NextWave();
    }

    IEnumerator SwitchtoPause() {
        currentWavePerformance.SwitchToPauseMode();
        yield return new WaitForSeconds(currentWavePerformance.m_wavePauseDuration);
        currentWavePerformance.WrapUp();
        pastWavePerformanceList.Add(currentWavePerformance);
        NextWave();
    }

    void CheckForClearedWave() {
        if(EnemySpawner.instance.isSpawning == false) {
            StartCoroutine(SwitchtoPause());
        }
    }

    void NextWave(){
        float difficulty = CalculateNewDifficulty();
        EnemyWave newWave = new EnemyWave(difficulty);
        OnNewWave?.Invoke(newWave);
        int waveNumber = 0;
        if(pastWavePerformanceList.Count > 0) { waveNumber = pastWavePerformanceList.Last<WavePerformance>().m_waveNumber + 1; }
        currentWavePerformance = new WavePerformance();
        currentWavePerformance.Init(gameSessionID, waveNumber, CalculateSpawnDuration(difficulty), CalculatePauseDuration(difficulty), difficulty);
    }


    float CalculateNewDifficulty(){
        float difficulty = 0;

        //Placeholder
        if(pastWavePerformanceList.Count > 0) {
            difficulty = pastWavePerformanceList.Last<WavePerformance>().m_waveDifficulty + 0.4f;
        }
        else {
            difficulty = 1.4f;
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

    float CalculatePauseDuration(float difficulty) {
        //Placeholder
        float duration = 20;
        return duration;
    }

    float CalculateSpawnDuration(float difficulty) {
        //Placeholder
        float duration = 20;
        return duration;
    }


    //--------------------------------------------------------------------------
    //Setup
    int GetNewGameSessionID() {
        int i = 0;
        while (Directory.Exists("saveFiles/" + i)) {
            i++;
        }
        return i;
    }
}
