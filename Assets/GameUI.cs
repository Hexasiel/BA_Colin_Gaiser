using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goldText;

    private void Start()
    {
        PlayerController.instance.gameUI = this;
        SetGoldText();
    }

    public void SetGoldText()
    {
        if (goldText == null)
            Debug.LogWarning("NULL");
        goldText.text = PlayerController.instance.gold.ToString();
    }

}
