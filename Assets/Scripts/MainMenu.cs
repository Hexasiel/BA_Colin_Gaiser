using EmotivUnityPlugin;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
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

    //Internal Variables
    int currentMode = 0;


    private void Start() {

        // init EmotivUnityItf without data buffer using
        euITF.Init(_clientId, _clientSecret, _appName, _appVersion, _isDataBufferUsing);

        // Start
        euITF.Start();

    }

    public void SetMode(int mode) {
        currentMode= mode;
        modeDisplay.text = mode.ToString();
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
            
        }
        else {
            UnityEngine.Debug.LogError("Must create a session first before subscribing data.");
        }
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
