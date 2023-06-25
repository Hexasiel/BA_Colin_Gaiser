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

    private float systemWeight = 1f;

    private void Start() {
        pastWavePerformanceList = new List<WavePerformance>();
        gameSessionID = GetNewGameSessionID();
        Enemy.OnAllEnemiesDefeated += CheckForClearedWave;
        TownHall.OnTownHallBuilt += NextWave;
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
        int waveNumber = 0;
        if (pastWavePerformanceList.Count > 0) { waveNumber = pastWavePerformanceList.Last<WavePerformance>().m_waveNumber + 1; }
        float predictedDifficulty = (float)PredictDifficulty(waveNumber);
        Debug.Log("Predicted fitting Difficulty: " + predictedDifficulty);

        EnemyWave newWave = new EnemyWave(predictedDifficulty);
        OnNewWave?.Invoke(newWave);
        currentWavePerformance = new WavePerformance();
        currentWavePerformance.Init(gameSessionID, waveNumber, CalculateSpawnDuration(predictedDifficulty), CalculatePauseDuration(predictedDifficulty), predictedDifficulty, systemWeight);
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


    double PredictDifficulty(int waveNumber) {
        if(pastWavePerformanceList.Count == 0) { return 0.4;}

        double rSquared, intercept, slope;
        double[] xValues = new double[pastWavePerformanceList.Count + 2];
        double[] yValues = new double[pastWavePerformanceList.Count + 2];
        foreach (WavePerformance wavePerformance in pastWavePerformanceList) {
            xValues[wavePerformance.m_waveNumber] = wavePerformance.m_waveNumber;
            yValues[wavePerformance.m_waveNumber] = wavePerformance.m_adjustedDifficultyWeighted;
        }
        for(int i = 0; i < 2; i++) {
            xValues[pastWavePerformanceList.Count + i] = i;
            yValues[pastWavePerformanceList.Count + i] = (i+1) * 0.4;
        }
        LinearRegression(xValues, yValues, out rSquared, out intercept, out slope);
        double predictedSkill = (slope * (double)waveNumber) + intercept;
        return predictedSkill;
    }

    /// <summary>
    /// Fits a line to a collection of (x,y) points.
    /// </summary>
    /// <param name="xVals">The x-axis values.</param>
    /// <param name="yVals">The y-axis values.</param>
    /// <param name="rSquared">The r^2 value of the line.</param>
    /// <param name="yIntercept">The y-intercept value of the line (i.e. y = ax + b, yIntercept is b).</param>
    /// <param name="slope">The slop of the line (i.e. y = ax + b, slope is a).</param>
    public static void LinearRegression(
        double[] xVals,
        double[] yVals,
        out double rSquared,
        out double yIntercept,
        out double slope) {
        if (xVals.Length != yVals.Length) {
            throw new Exception("Input values should be with the same length.");
        }

        double sumOfX = 0;
        double sumOfY = 0;
        double sumOfXSq = 0;
        double sumOfYSq = 0;
        double sumCodeviates = 0;

        for (var i = 0; i < xVals.Length; i++) {
            var x = xVals[i];
            var y = yVals[i];
            sumCodeviates += x * y;
            sumOfX += x;
            sumOfY += y;
            sumOfXSq += x * x;
            sumOfYSq += y * y;
        }

        var count = xVals.Length;
        var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
        var ssY = sumOfYSq - ((sumOfY * sumOfY) / count);

        var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
        var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
        var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

        var meanX = sumOfX / count;
        var meanY = sumOfY / count;
        var dblR = rNumerator / Math.Sqrt(rDenom);

        rSquared = dblR * dblR;
        yIntercept = meanY - ((sCo / ssX) * meanX);
        slope = sCo / ssX;
    }
}


