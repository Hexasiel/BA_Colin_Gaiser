using EmotivUnityPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WavePerformance 
{
    //General
    public int m_waveNumber;
    public float m_waveSpawntime;
    public float m_waveSpawnDuration;
    public float m_wavePauseDuration;
    public float m_waveDifficulty;


    //--------------------------------------------------------------------------
    //Game Metrics
    public float m_waveClearDuration;
    public int m_lostHP;
    public int m_lostBuildingHP;
    public int m_lostBuildings;
    public int m_healedtBuildingHP;
    public int m_damageDealtInCloseCombat;


    //--------------------------------------------------------------------------
    //BCI Metrics
    public List<float> m_bciStats_eng;
    public List<float> m_bciStats_exc;
    public List<float> m_bciStats_foc;
    public List<float> m_bciStats_int;
    public List<float> m_bciStats_rel;
    public List<float> m_bciStats_str;


    public void Init(int waveNumber, float waveSpawnTime,float waveSpawnDuration, float wavePauseDuration, float waveDifficulty) {
        m_waveNumber = waveNumber;
        m_waveSpawntime = waveSpawnTime;
        m_waveSpawnDuration = waveSpawnDuration;
        m_wavePauseDuration = wavePauseDuration;
        m_waveDifficulty = waveDifficulty;
        m_waveClearDuration = 0;
        m_lostHP = 0;
        m_lostBuildingHP = 0;
        m_lostBuildings = 0;
        m_damageDealtInCloseCombat = 0;

        m_bciStats_eng = new List<float>();
        m_bciStats_exc = new List<float>();
        m_bciStats_foc = new List<float>();  
        m_bciStats_int = new List<float>();
        m_bciStats_rel = new List<float>();
        m_bciStats_str = new List<float>();

        Building.OnBuildingDamaged += AddBuildingDamage;
        Building.OnBuildingDestroyed += AddDestroyedBuilding;
        Building.OnBuildingHealed += AddBuildingHeal;
        //EmotivUnityItf.Instance.PerfDataReceived += AddBCIMetrics;
    }

    void AddBCIMetrics(float eng, float exc, float foc, float intel, float rel, float str ) {

        m_bciStats_eng.Add( eng );
        m_bciStats_exc.Add( exc );
        m_bciStats_foc.Add( foc );
        m_bciStats_int.Add(intel);
        m_bciStats_rel.Add( rel );
        m_bciStats_str.Add( str );
    }

    void AddBuildingDamage(int dmg) {
        m_lostBuildingHP += dmg;
    }

    void AddBuildingHeal(int heal) {
        m_healedtBuildingHP += heal;
    }

    void AddDestroyedBuilding() {
        m_lostBuildings++;
    }

    void AddPlayerDamage(int dmg) {
        m_lostHP += dmg;
    }

    public void WrapUp() {
        //TODO
        Building.OnBuildingDamaged -= AddBuildingDamage;

    }
}
