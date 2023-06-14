using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePerformance 
{
    //General
    public int waveNumber;
    public float waveSpawntime;
    public float waveSpawnDuration;
    public float wavePauseDuration;
    public float waveDifficulty;


    //--------------------------------------------------------------------------
    //Game Metrics
    public float waveClearDuration;
    public int lostHP;
    public int lostBuildingHP;
    public int lostBuildings;
    public int damageDealtInCloseCombat;


    //--------------------------------------------------------------------------
    //BCI Metrics


    public void WrapUp() {
        //TODO
    }
}
