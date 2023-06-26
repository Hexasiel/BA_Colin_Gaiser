using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfo : MonoBehaviour
{
    public static BuildingInfo Instance;




    [SerializeField] GameObject display;
    [SerializeField] Image imageDisplay;
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] TextMeshProUGUI descriptionDisplay;

    private void Awake() {
        Instance= this;
        BuildingMenu.OnUpdateDisplay += UpdateDisplay;
        BuildingMenu.OnShowDisplay += ShowDisplay;
        BuildingMenu.OnHideDisplay += HideDisplay;
    }

    void ShowDisplay() {
        display.SetActive(true);
    }

    void HideDisplay() {
        display.SetActive(false);
    }

    void UpdateDisplay(Building building) {

        int index = building.m_level;
        if (building.m_isBuilt && building.m_level < building.m_maxLevel) {
            index++;
        }
        //TODO if built level +1
        imageDisplay.sprite = building.m_sprites[index];
        nameDisplay.text = building.m_name;
        descriptionDisplay.text = building.m_description[index];
    }
}
