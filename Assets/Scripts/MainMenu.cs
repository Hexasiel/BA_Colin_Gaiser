using EmotivUnityPlugin;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    //Cortex Settings
    private string _clientId = "Tk31cS3wUgbgQMVg0eddxvCBTbhd5cqnvzcC8BFR";
    private string _clientSecret = "kMOHbjPtmm39mVUADktwWNV9sAL0DJ4j4ntgieN2cSqgVBcez6MaL3OyZB48ewfR7k6BIsX4X4Cv8okH3Ld54reOW0iqxlZAO2UWXOKw579phmU6moN4hzH0yhmJmmxy";
    private string _appName = "UnityApp";
    private string _appVersion = "3.3.0";
    bool _isDataBufferUsing = false; // default subscribed data will not saved to Data buffer

    //External References
    EmotivUnityItf euITF = EmotivUnityItf.Instance;

    [SerializeField] TextMeshProUGUI modeDisplay;
    [SerializeField] UnityEngine.UIElements.Button createSessionButton;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TMP_InputField headsetID;
    [SerializeField] TMP_Dropdown knownDevicesDropdown;
    [SerializeField] UnityEngine.UIElements.Button connectStreamButton;
    [SerializeField] UnityEngine.UI.Button calibrationMenuButton;
    [SerializeField] TextMeshProUGUI calibrationText;

    //Internal Variables
    int currentMode = 0;

    //BCI values for calibration
    List<float> m_bciStats_eng = new List<float>();
    List<float> m_bciStats_exc = new List<float>();
    List<float> m_bciStats_foc = new List<float>();
    List<float> m_bciStats_int = new List<float>();
    List<float> m_bciStats_rel = new List<float>();
    List<float> m_bciStats_str = new List<float>();


    private void Start() {

        // init EmotivUnityItf without data buffer using
        euITF.Init(_clientId, _clientSecret, _appName, _appVersion, _isDataBufferUsing);

        // Start
        euITF.Start();

    }

    public void SetMode(int mode) {
        currentMode= mode;
        modeDisplay.text = mode.ToString();
        float weight;
        switch (mode){
            case 1: weight = 0f;    break;
            case 2: weight = 0.5f;  break;
            case 3: weight = 0.8f;  break;
            case 4: weight = 1f;    break;
            default: weight = 0f;   break;
        }
        WavePerformance.m_systemWeight = weight;
    }

    public void LoadGame() {
        SceneManager.LoadScene("SampleScene");
    }

    public void ConnectToEEG() {
        if(headsetID.text == "") {
            UnityEngine.Debug.LogError("No HeadsetID selected.");
            return;
        }
        Debug.Log("onCreateSessionBtnClick");
        if (!euITF.IsSessionCreated) {
            euITF.CreateSessionWithHeadset(headsetID.text);
            statusText.text = "Session Created";
        }
        else {
            UnityEngine.Debug.LogError("There is a session created.");
        }
    }

    public void OnDeviceListSelected() {
        headsetID.text = knownDevicesDropdown.options[knownDevicesDropdown.value].text;
        if (knownDevicesDropdown.value == 0)
            headsetID.text = "";
    }

    public void SubscribeToDataStreams() {
        Debug.Log("onSubscribeBtnClick: " + euITF.IsSessionCreated + " .");
        if (euITF.IsSessionCreated) {
            List<string> streams = new List<string>();
            streams.Add("met");
            euITF.SubscribeData(streams);
            statusText.text = "Stream Subscribed";
        }
        else {
            UnityEngine.Debug.LogError("Must create a session first before subscribing data.");
        }
    }

    public void StartCalibration() {
        StartCoroutine(CalibrateBCI());
    }

    IEnumerator CalibrateBCI() {
        if(!euITF.IsSessionCreated) {
            yield return null;
        }
        else {
            calibrationText.text = "Calibrating...";
            calibrationMenuButton.transform.parent.gameObject.SetActive(true);
            EmotivUnityItf.Instance.PerfDataReceived += AddBCIMetrics;
            yield return new WaitForSeconds(30);
            EmotivUnityItf.Instance.PerfDataReceived -= AddBCIMetrics;
            UpdateMinMaxAverages();
            calibrationMenuButton.interactable = true;
            calibrationText.text = "Calibration Done!";
            statusText.text = "BCI Calibrated";
        }
    }

    void UpdateMinMaxAverages() {

        float[] bciValues = new float[] { m_bciStats_eng.Average(), m_bciStats_exc.Average(), m_bciStats_foc.Average(), m_bciStats_int.Average(), m_bciStats_rel.Average(), m_bciStats_str.Average() };
        for (int i = 0; i < WavePerformance.bci_max_averages.Length; i++) {
            WavePerformance.bci_max_averages[i] = bciValues[i];
            WavePerformance.bci_min_averages[i] = bciValues[i];
        }
        Debug.Log("Calibration done: " + bciValues);
    }

    void AddBCIMetrics(object sender, ArrayList metrics) {

        m_bciStats_eng.Add(float.Parse(metrics[2].ToString()));
        m_bciStats_exc.Add(float.Parse(metrics[4].ToString()));
        m_bciStats_foc.Add(float.Parse(metrics[13].ToString()));
        m_bciStats_int.Add(float.Parse(metrics[11].ToString()));
        m_bciStats_rel.Add(float.Parse(metrics[9].ToString()));
        m_bciStats_str.Add(float.Parse(metrics[7].ToString()));
    }

    public void QuitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE_WIN
        Application.Quit();
        #endif
    }

    void OnApplicationQuit() {
        Debug.Log("Application ending after " + Time.time + " seconds");
        euITF.Stop();
    }
}
