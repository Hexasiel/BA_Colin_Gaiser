using EmotivUnityPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

[Serializable]
public class WavePerformance 
{
    //General
    public int m_gameSession;
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


    public void Init(int gameSession, int waveNumber,float waveSpawnDuration, float wavePauseDuration, float waveDifficulty) {
        m_gameSession = gameSession;
        m_waveNumber = waveNumber;
        m_waveSpawntime = Time.realtimeSinceStartup;
        m_waveSpawnDuration = waveSpawnDuration;
        m_wavePauseDuration = wavePauseDuration;
        m_waveDifficulty = waveDifficulty;
        m_waveClearDuration = -1;
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
        EmotivUnityItf.Instance.PerfDataReceived += AddBCIMetrics;
        PlayerController.OnPlayerDealtDamage += AddPlayerMeleeDamageDealt;
        PlayerController.OnPlayerGetDamage += AddPlayerDamage;
    }

    void AddBCIMetrics(object sender ,ArrayList metrics ) {

        m_bciStats_eng.Add(float.Parse(metrics[2].ToString()));
        m_bciStats_exc.Add(float.Parse(metrics[4].ToString()));
        m_bciStats_foc.Add(float.Parse(metrics[7].ToString()));
        m_bciStats_int.Add(float.Parse(metrics[9].ToString()));
        m_bciStats_rel.Add(float.Parse(metrics[11].ToString()));
        m_bciStats_str.Add(float.Parse(metrics[13].ToString()));

    }

    void AddBuildingDamage(int dmg) {
        m_lostBuildingHP += dmg;
    }

    void AddPlayerMeleeDamageDealt(int dmg) {
        m_damageDealtInCloseCombat+= dmg;
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
        Building.OnBuildingDestroyed -= AddDestroyedBuilding;
        Building.OnBuildingHealed -= AddBuildingHeal;
        EmotivUnityItf.Instance.PerfDataReceived -= AddBCIMetrics;
        PlayerController.OnPlayerDealtDamage -= AddPlayerMeleeDamageDealt;
        PlayerController.OnPlayerGetDamage -= AddPlayerDamage;

        m_waveClearDuration = Time.realtimeSinceStartup - m_waveSpawntime;

        SaveToJSON("saveFiles/" + m_gameSession);

    }

    public void SaveToJSON(string filePath) {

        if (!Directory.Exists(filePath)) {Directory.CreateDirectory(filePath); }
        string jsonString = JsonUtility.ToJson(this, true);
        string saveFile =  filePath + "/" + "wavePerformance_" + m_waveNumber + ".json";
        File.WriteAllText(saveFile, jsonString);
    }
}
