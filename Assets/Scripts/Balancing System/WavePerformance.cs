using EmotivUnityPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System.Linq;

[Serializable]
public class WavePerformance 
{
    static float[] bci_max_averages = new float[6];
    static float[] bci_min_averages = new float[6];

    //General
    public int m_gameSession;
    public int m_waveNumber;
    public float estimatedPlayerSkillLevel;
    public float systemWeight;

    public float m_waveDifficulty;
    public float m_waveSpawntime;
    public float m_waveSpawnDuration;
    public float m_waveMaxClearDuration;
    public float m_wavePauseDuration;

    public float m_adjustedPlayerSkillLevelGM;
    public float m_adjustedPlayerSkillLevelBCI;
    public float m_adjustedPlayerSkillLevelWeighted;

    public bool m_Pause;

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

    public List<float> m_bciStats_Pause_eng;
    public List<float> m_bciStats_Pause_exc;
    public List<float> m_bciStats_Pause_foc;
    public List<float> m_bciStats_Pause_int;
    public List<float> m_bciStats_Pause_rel;
    public List<float> m_bciStats_Pause_str;


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
        m_Pause = false;

        m_bciStats_eng = new List<float>();
        m_bciStats_exc = new List<float>();
        m_bciStats_foc = new List<float>();  
        m_bciStats_int = new List<float>();
        m_bciStats_rel = new List<float>();
        m_bciStats_str = new List<float>();

        m_bciStats_Pause_eng = new List<float>();
        m_bciStats_Pause_exc = new List<float>();
        m_bciStats_Pause_foc = new List<float>();
        m_bciStats_Pause_int = new List<float>();
        m_bciStats_Pause_rel = new List<float>();
        m_bciStats_Pause_str = new List<float>();

        Building.OnBuildingDamaged += AddBuildingDamage;
        Building.OnBuildingDestroyed += AddDestroyedBuilding;
        Building.OnBuildingHealed += AddBuildingHeal;
        EmotivUnityItf.Instance.PerfDataReceived += AddBCIMetrics;
        PlayerController.OnPlayerDealtDamage += AddPlayerMeleeDamageDealt;
        PlayerController.OnPlayerGetDamage += AddPlayerDamage;
    }

    void AddBCIMetrics(object sender ,ArrayList metrics ) {

        if (m_Pause) {
            m_bciStats_Pause_eng.Add(float.Parse(metrics[2].ToString()));
            m_bciStats_Pause_exc.Add(float.Parse(metrics[4].ToString()));
            m_bciStats_Pause_foc.Add(float.Parse(metrics[7].ToString()));
            m_bciStats_Pause_int.Add(float.Parse(metrics[9].ToString()));
            m_bciStats_Pause_rel.Add(float.Parse(metrics[11].ToString()));
            m_bciStats_Pause_str.Add(float.Parse(metrics[13].ToString()));
        }
        else {
            m_bciStats_eng.Add(float.Parse(metrics[2].ToString()));
            m_bciStats_exc.Add(float.Parse(metrics[4].ToString()));
            m_bciStats_foc.Add(float.Parse(metrics[7].ToString()));
            m_bciStats_int.Add(float.Parse(metrics[9].ToString()));
            m_bciStats_rel.Add(float.Parse(metrics[11].ToString()));
            m_bciStats_str.Add(float.Parse(metrics[13].ToString()));
        }
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

    public void SwitchToPauseMode() {
        m_Pause = true;
    }

    public void WrapUp() {
        Building.OnBuildingDamaged -= AddBuildingDamage;
        Building.OnBuildingDestroyed -= AddDestroyedBuilding;
        Building.OnBuildingHealed -= AddBuildingHeal;
        EmotivUnityItf.Instance.PerfDataReceived -= AddBCIMetrics;
        PlayerController.OnPlayerDealtDamage -= AddPlayerMeleeDamageDealt;
        PlayerController.OnPlayerGetDamage -= AddPlayerDamage;

        m_waveClearDuration = Time.realtimeSinceStartup - m_waveSpawntime;
        SaveToJSON("saveFiles/" + m_gameSession);

    }

    float EvaluatePredictedSkillLevel() {
        //BCI
        m_adjustedPlayerSkillLevelBCI = 0;

        //Game Metrics
        m_adjustedPlayerSkillLevelGM = 0;

        //Combination
        m_adjustedPlayerSkillLevelWeighted = (m_adjustedPlayerSkillLevelGM * (1-systemWeight)) + (m_adjustedPlayerSkillLevelBCI * systemWeight);
        return m_adjustedPlayerSkillLevelWeighted;
    }

    void EvaluatePredictedSkillLevelBCI() {
       float boredom = 
                ((bci_max_averages[0] - m_bciStats_eng.Average()) 
            +   (bci_max_averages[1] - m_bciStats_exc.Average()) 
            +   (bci_max_averages[3] - m_bciStats_int.Average() ) 
            +   (m_bciStats_rel.Average() - bci_min_averages[4])) 
            /   4f;

        float frustration = 
                ((m_bciStats_str.Average() - bci_min_averages[5]) 
            +   (bci_max_averages[3] - m_bciStats_int.Average())) 
            /   2f;
    }

    void UpdateMinMaxAverages() {

        float[] bciValues = new float[] { m_bciStats_eng.Average(), m_bciStats_exc.Average(), m_bciStats_foc.Average(), m_bciStats_int.Average(), m_bciStats_rel.Average(), m_bciStats_str.Average() };
        for(int i = 0; i < bci_max_averages.Length; i++) {
            if (bciValues[i] > bci_max_averages[i]) { 
                bci_max_averages[i] = bciValues[i]; 
            }
            if (bciValues[i] < bci_min_averages[i]) {
                bci_min_averages[i] = bciValues[i];
            }
        }
    }

    public void SaveToJSON(string filePath) {

        if (!Directory.Exists(filePath)) {Directory.CreateDirectory(filePath); }
        string jsonString = JsonUtility.ToJson(this, true);
        string saveFile =  filePath + "/" + "wavePerformance_" + m_waveNumber + ".json";
        File.WriteAllText(saveFile, jsonString);
    }
}
