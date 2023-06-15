using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] Image buildModeSprite;
    [SerializeField] Image battleModeSprite;
    [SerializeField] Slider healthSlider;

    private void Start()
    {
        PlayerController.instance.gameUI = this;
        UpdateUI();
    }

    public void UpdateUI()
    {
        goldText.text = PlayerController.instance.gold.ToString();
        healthSlider.value = (float)PlayerController.instance.m_health / (float)PlayerController.instance.maxHealth;
    }

    public void SwitchBattleMode()
    {
        UnityEngine.Color c = buildModeSprite.color;
        buildModeSprite.color = battleModeSprite.color;
        battleModeSprite.color = c;
    }

}
