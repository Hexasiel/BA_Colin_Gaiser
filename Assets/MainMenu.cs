using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int currentMode = 0;

     [SerializeField] TextMeshProUGUI modeDisplay;

    public void SetMode(int mode) {
        currentMode= mode;
        modeDisplay.text = mode.ToString();
    }

    public void LoadGame() {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE_WIN
        Application.Quit();
        #endif
    }
}
