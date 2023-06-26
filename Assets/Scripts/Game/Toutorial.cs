using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Toutorial : MonoBehaviour
{
    [SerializeField] string[] m_ObjectiveName;
    [SerializeField] string[] m_ObjectiveDescription;

    [SerializeField] TextMeshProUGUI m_ObjectiveNameDisplay;
    [SerializeField] TextMeshProUGUI m_ObjectiveDescriptionDisplay;

    bool m_ObjectiveAchieved = false;

    private void Start() {
        StartCoroutine(Tutorial());
    }

    void NewObjective() {
        m_ObjectiveAchieved = true;
    }

    IEnumerator Tutorial() {
        int i = 0;

        //Objective 1: Build Townhall
        TownHall.OnTownHallBuilt += NewObjective;
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
        while(!m_ObjectiveAchieved) {
            yield return null;
        }
        m_ObjectiveAchieved= false;
        TownHall.OnTownHallBuilt -= NewObjective;
        i++;

        //Objective 2: Switch to Battle Mode
        PlayerController.OnPlayerSwitchMode += NewObjective;
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
        while (!m_ObjectiveAchieved) {
            yield return null;
        }
        m_ObjectiveAchieved = false;
        PlayerController.OnPlayerSwitchMode -= NewObjective;
        i++;

        //Objective 3: Dash
        PlayerController.OnPlayerDash += NewObjective;
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
        while (!m_ObjectiveAchieved) {
            yield return null;
        }
        m_ObjectiveAchieved = false;
        PlayerController.OnPlayerDash -= NewObjective;
        i++;

        //Objective 4: Attack
        PlayerController.OnPlayerAttack += NewObjective;
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
        while (!m_ObjectiveAchieved) {
            yield return null;
        }
        m_ObjectiveAchieved = false;
        PlayerController.OnPlayerAttack -= NewObjective;
        i++;

        //Objective 5: Build a Wall
        BuildingMenu.OnWallBuilt += NewObjective;
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
        while (!m_ObjectiveAchieved) {
            yield return null;
        }
        m_ObjectiveAchieved = false;
        BuildingMenu.OnWallBuilt -= NewObjective;
        i++;

        //Objective 6: Build a Archer Tower
        BuildingMenu.OnArcherTowerBuilt += NewObjective;
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
        while (!m_ObjectiveAchieved) {
            yield return null;
        }
        m_ObjectiveAchieved = false;
        BuildingMenu.OnArcherTowerBuilt -= NewObjective;
        i++;

        //Objective 7: Upgrade a Building
        BuildingMenu.OnBuildingUpgraded += NewObjective;
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
        while (!m_ObjectiveAchieved) {
            yield return null;
        }
        m_ObjectiveAchieved = false;
        BuildingMenu.OnBuildingUpgraded -= NewObjective;
        i++;

        //Objective 8: Repair a building
        PlayerController.OnPlayerRepair += NewObjective;
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
        while (!m_ObjectiveAchieved) {
            yield return null;
        }
        m_ObjectiveAchieved = false;
        PlayerController.OnPlayerRepair -= NewObjective;
        i++;

        //Objective 9: Defend your Town
        m_ObjectiveNameDisplay.text = m_ObjectiveName[i];
        m_ObjectiveDescriptionDisplay.text = m_ObjectiveDescription[i];
    }
}
