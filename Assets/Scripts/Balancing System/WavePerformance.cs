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
    public static float[] bci_max_averages = new float[6];
    public static float[] bci_min_averages = new float[6];

    public static int[] ref_damageDealtInCloseCombat = new int[3] { 0, 80, 800 };
    public static int[] ref_unneccessaryActions = new int[3] { 0, 3, 30 };
    public static float[] ref_actionFrequency = new float[3] { 0f, 2f, 20f };

    public static int[] ref_failedActions = new int[3] { 0, 5, 30 };


    public static float[] ref_lostHPPercentage = new float[3] { 0f, 0.25f, 1f};
    public static float[] ref_lostBuildingHPPercentage = new float[3] { 0f, 0.25f, 1f };
    public static float[] ref_lostBuildingsPercentage = new float[3] { 0f, 0.12f, 1f };
    public static int[] ref_goldProfit = new int[3] { -90, 10, 110 };

    //--------------------------------------------------------------------------
    //General
    public int m_gameSession;
    public int m_waveNumber;
    public static float m_systemWeight;

    public float m_waveDifficulty;
    public float m_waveSpawntime;
    public float m_waveSpawnDuration;
    public float m_waveMaxClearDuration;
    public float m_wavePauseDuration;

    public float m_adjustedDifficultyGM;
    public float m_adjustedDifficultyBCI;
    public float m_adjustedDifficultyWeighted;

    public bool m_Pause;

    //--------------------------------------------------------------------------
    //Game Metrics
    public float m_waveClearDuration;


    public int m_damageDealtInCloseCombat;
    public int m_unneccessaryActions;
    public float m_actionFrequency; 
    public List<float> m_actionTimes = new List<float>();

    public int m_failedActions;

    //Performance
    public float m_performance;
    public int m_lostHP;
    public int m_healedHP;
    public int m_healedHPPause;
    public float m_lostHPPercentage;
    public int m_lostBuildingHP;
    public int m_lostBuildings;
    public int m_healedBuildingHP;
    public int m_healedBuildingHPPause;
    public float m_lostBuildingHPPercentage;
    public float m_lostBuildingsPercentage;
    public int m_goldCollected;
    public int m_goldLost;
    public int m_goldProfit;

    public float m_boredom_GM;
    public float m_frustration_GM;


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

    public float m_boredom_BCI;
    public float m_frustration_BCI;


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
        PlayerController.OnPlayerHealed += AddPlayerHealing;
        GoldPiece.OnGoldPieceCollected += AddGoldPieceCollected;
        PlayerController.OnPlayerUnneccessaryAction += AddUnneccessaryAction;
        PlayerController.OnPlayerFailedAction += AddFailedAction;

        PlayerController.OnPlayerAttack += AddPlayerAction;
        PlayerController.OnPlayerDash += AddPlayerAction;
        PlayerController.OnPlayerRepair+= AddPlayerAction;
    }

    void AddBCIMetrics(object sender, ArrayList metrics ) {

        if (m_Pause) {
            m_bciStats_Pause_eng.Add(float.Parse(metrics[2].ToString()));
            m_bciStats_Pause_exc.Add(float.Parse(metrics[4].ToString()));
            m_bciStats_Pause_foc.Add(float.Parse(metrics[13].ToString()));
            m_bciStats_Pause_int.Add(float.Parse(metrics[11].ToString()));
            m_bciStats_Pause_rel.Add(float.Parse(metrics[9].ToString()));
            m_bciStats_Pause_str.Add(float.Parse(metrics[7].ToString()));
        }
        else {
            m_bciStats_eng.Add(float.Parse(metrics[2].ToString()));
            m_bciStats_exc.Add(float.Parse(metrics[4].ToString()));
            m_bciStats_foc.Add(float.Parse(metrics[13].ToString()));
            m_bciStats_int.Add(float.Parse(metrics[11].ToString()));
            m_bciStats_rel.Add(float.Parse(metrics[9].ToString()));
            m_bciStats_str.Add(float.Parse(metrics[7].ToString()));
        }
    }

    void AddPlayerHealing(int heal) {
        if (!m_Pause) {
            m_healedHP += heal;
        }
        else {
            m_healedHPPause += heal;
        }
    }

    void AddUnneccessaryAction() {
        m_unneccessaryActions++;
    }

    void AddFailedAction() {
        m_failedActions++;
    }

    void AddPlayerAction() {
        m_actionTimes.Add(Time.realtimeSinceStartup - m_waveSpawntime);
    }

    void AddBuildingDamage(int dmg) {
        m_lostBuildingHP += dmg;
    }

    void AddGoldPieceCollected() {
        m_goldCollected++;
    }

    void AddPlayerMeleeDamageDealt(int dmg) {
        m_damageDealtInCloseCombat+= dmg;
    }

    void AddBuildingHeal(int heal) {
        if (!m_Pause) {
            m_healedBuildingHP += heal;
        }
        else {
            m_healedBuildingHPPause += heal;
        }
    }

    void AddDestroyedBuilding(int value) {
        m_lostBuildings++;
        m_goldLost += value;
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
        EvaluatePredictedSkillLevel();
        SaveToJSON("saveFiles/" + m_gameSession);
    }

    float EvaluateSingleStatINT(int stat, int reference, int min, int max) {
        if(stat == reference) return 0;
        else if(stat < reference) {
            return -1 * Mathf.InverseLerp(reference, min, stat);
        }
        else{
            return Mathf.InverseLerp(reference, max, stat);
        }
    }

    float EvaluateSingleStatFLOAT(float stat, float reference, float min, float max) {
        Debug.Log(stat + "       " + reference + "        " + min + "         " + max);
        if (stat == reference) return 0;
        else if (stat < reference) {
            return -1 * Mathf.InverseLerp(reference, min, stat);
        }
        else {
            return Mathf.InverseLerp(reference, max, stat);
        }
    }

    void EvaluatePredictedSkillLevelGM() {

        m_lostHPPercentage = (float)(m_lostHP - m_healedHP) / PlayerController.instance.maxHealth; 
        m_lostBuildingsPercentage = (float)m_lostBuildings / (float)Building.count;
        m_lostBuildingHPPercentage = ((float)m_lostBuildingHP - m_healedBuildingHP) / ((float)Building.count * 500);
        m_actionFrequency = m_waveClearDuration / m_actionTimes.Count;
        m_goldProfit = m_goldCollected - m_goldLost;

        float a = EvaluateSingleStatFLOAT(m_lostHPPercentage, ref_lostHPPercentage[1], ref_lostHPPercentage[0], ref_lostHPPercentage[2]);
        float b = EvaluateSingleStatFLOAT(m_lostBuildingHPPercentage, ref_lostBuildingHPPercentage[1], ref_lostBuildingHPPercentage[0], ref_lostBuildingHPPercentage[2]);
        float c = EvaluateSingleStatFLOAT(m_lostBuildingsPercentage, ref_lostBuildingsPercentage[1], ref_lostBuildingsPercentage[0], ref_lostBuildingsPercentage[2]);
        float d = EvaluateSingleStatINT(m_goldProfit, ref_goldProfit[1], ref_goldProfit[0], ref_goldProfit[2]);

        m_performance = 
                (-1 * EvaluateSingleStatFLOAT(m_lostHPPercentage, ref_lostHPPercentage[1], ref_lostHPPercentage[0], ref_lostHPPercentage[2])
            -   EvaluateSingleStatFLOAT(m_lostBuildingHPPercentage, ref_lostBuildingHPPercentage[1], ref_lostBuildingHPPercentage[0], ref_lostBuildingHPPercentage[2])
            -   EvaluateSingleStatFLOAT(m_lostBuildingsPercentage, ref_lostBuildingsPercentage[1], ref_lostBuildingsPercentage[0], ref_lostBuildingsPercentage[2])
            +   EvaluateSingleStatINT(m_goldProfit, ref_goldProfit[1], ref_goldProfit[0], ref_goldProfit[2]))
            /   4f;

        m_boredom_GM = 
                (-1 * EvaluateSingleStatFLOAT(m_actionFrequency, ref_actionFrequency[1], ref_actionFrequency[0], ref_actionFrequency[2])
            +   EvaluateSingleStatINT(m_unneccessaryActions, ref_unneccessaryActions[1], ref_unneccessaryActions[0], ref_unneccessaryActions[2])
            -   EvaluateSingleStatFLOAT(m_damageDealtInCloseCombat, ref_damageDealtInCloseCombat[1] * m_waveDifficulty, ref_damageDealtInCloseCombat[0] * m_waveDifficulty, ref_damageDealtInCloseCombat[2] * m_waveDifficulty)
            +   m_performance
            +   m_performance)
            /   5f;

        m_frustration_GM = 
                (EvaluateSingleStatINT(m_failedActions, ref_failedActions[1], ref_failedActions[0], ref_failedActions[2])
            -   m_performance
            -   m_performance) 
            /   3;

        if (m_boredom_GM > 0 && m_frustration_GM < 0) {
            m_adjustedDifficultyGM = m_waveDifficulty * (1 + m_boredom_GM);
        }
        else if (m_boredom_GM < 0 && m_frustration_BCI > 0) {
            m_adjustedDifficultyGM = m_waveDifficulty * (1 - m_frustration_GM);
        }
        else if (m_boredom_GM > 0 && m_frustration_BCI > 0) {
            m_adjustedDifficultyGM = m_waveDifficulty * (1 - m_frustration_GM + m_boredom_GM);
        }
        else {
            m_adjustedDifficultyGM = m_waveDifficulty;
        }
    }

    void EvaluatePredictedSkillLevelBCI() {
        m_boredom_BCI = 
                ((bci_max_averages[0] - m_bciStats_eng.Average()) 
            +   (bci_max_averages[1] - m_bciStats_exc.Average()) 
            +   (bci_max_averages[3] - m_bciStats_int.Average() ) 
            +   (m_bciStats_rel.Average() - bci_min_averages[4])) 
            /   4f;

        m_frustration_BCI = 
                ((m_bciStats_str.Average() - bci_min_averages[5]) 
            +   (bci_max_averages[3] - m_bciStats_int.Average())) 
            /   2f;

        if(m_boredom_BCI > 0 && m_frustration_BCI < 0) {
            m_adjustedDifficultyBCI = m_waveDifficulty * (1 + m_boredom_BCI);
        }
        else if (m_boredom_BCI < 0 && m_frustration_BCI > 0) {
            m_adjustedDifficultyBCI = m_waveDifficulty * (1 - m_frustration_BCI);
        }
        else if(m_boredom_BCI > 0 && m_frustration_BCI > 0){
            m_adjustedDifficultyBCI = m_waveDifficulty * (1 - m_frustration_BCI + m_boredom_BCI);
        }
        else {
            m_adjustedDifficultyBCI = m_waveDifficulty;
        }
        UpdateMinMaxAverages();
    }

    float EvaluatePredictedSkillLevel() {
        //BCI
        //EvaluatePredictedSkillLevelBCI();

        //Game Metrics
        EvaluatePredictedSkillLevelGM();

        //Combination
        m_adjustedDifficultyWeighted = (m_adjustedDifficultyGM * (1 - m_systemWeight)) + (m_adjustedDifficultyBCI * m_systemWeight);
        return m_adjustedDifficultyWeighted;
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
