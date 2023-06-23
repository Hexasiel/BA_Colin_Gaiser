using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    //External References
    [SerializeField] GameObject banner;
    Transform[] childButton = new Transform[9];
    TextMeshPro[] costTexts = new TextMeshPro[7];
    public Camera myCam;

    Building currentBuilding = null;
    Transform hoveredElement;

    public GameObject[] buildingPrefabs;


    //Properties
    bool[] buttonStates = new bool[] { true, true, true, true, true, true, false, false};
    int activeElement = 0;
    private LayerMask _layerMask;
    public Color defaultColor = new Color(1, 1, 1, 0.5f);
    public Color hoveredColor = new Color(1, 1, 1, 1f);
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private void Awake()
    {
        _layerMask = LayerMask.GetMask("UI");
        myCam = Camera.main;
        for(int i = 0; i < 9; i++)
        {
            childButton[i] = transform.GetChild(i);
        }

        for (int i = 0; i < 7; i++)
        {
            costTexts[i] = transform.GetChild(i+1).GetComponentInChildren<TextMeshPro>();
        }
        
        UpdateUI();
    }

    public void ShowBanner(bool b)
    {
        banner.SetActive(b);
    }

    void GetMouseInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity, _layerMask);
        foreach (var hit in hits)
        {
            if (hit.collider.name != transform.name)
            {
                //Debug.Log("Mouse is over " + hit.collider.transform.name + ".");
                hoveredElement = hit.transform;
                SetActiveElement(hit.collider.transform.GetSiblingIndex());
            }
        }
    }

    private void OnMouseOver()
    {
        GetMouseInfo();
    }

    private void OnMouseEnter()
    {
        if (PlayerController.instance.battleMode)
            return;
        SetActiveElement(0);
      foreach( Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }  
    }

    public void SetButtonsEnabled(bool[] newStates) {
        if(newStates.Length != buttonStates.Length) {
            Debug.LogError("Trying to edit button states but array length does not match!");
            return;
        }
        buttonStates = newStates;
    }

    public void SetActiveElement(int elementIndex)
    {
        for (int i = 0; i < buttonStates.Length; i++) {
            if (buttonStates[i]) {
                childButton[i+1].GetComponent<SpriteRenderer>().color = defaultColor;
            }
            else {
                childButton[i+1].GetComponent<SpriteRenderer>().color = disabledColor;
            }
        }
        childButton[elementIndex].GetComponent<SpriteRenderer>().color = hoveredColor;
        activeElement = elementIndex;
    }

    private void OnMouseUpAsButton()
    {
        if (PlayerController.instance.battleMode)
            return;
        if(activeElement == 8) { DemolishBuilding();return; }
            
        if(activeElement == 7) { UpgradeBuilding(); return; }
            
        BuildBuilding(activeElement);
    }

    void BuildBuilding(int buildingIndex)
    {
        if (currentBuilding){
            Debug.LogWarning("Trying to build, but there is alredy a building in this slot!");
            return;
        }
        if (buildingPrefabs[buildingIndex -1].GetComponent<Building>().m_upgradeCost[0] <= PlayerController.instance.gold)
        {
            currentBuilding = Instantiate(buildingPrefabs[buildingIndex - 1], transform.parent).GetComponent<Building>();
            currentBuilding.parentMenu = this;
            ShowBanner(false);
            PlayerController.instance.gold -= currentBuilding.m_upgradeCost[0];
            PlayerController.instance.gameUI.UpdateUI();
        }
        else
        {
            Debug.Log("Trying to build a building, but player has not enough gold!");
            return;
        }
        SetButtonsEnabled(new bool[] { false, false, false, false, false, false, true, true }) ;
        UpdateUI();
    }

    void UpgradeBuilding()
    {
        if(currentBuilding == null)
        {
            Debug.LogWarning("Trying to upgrade a building, but these is nothing built in this slot!");
            return;
        }
        if (currentBuilding.m_level == currentBuilding.m_maxLevel)
        {
            Debug.LogWarning("Trying to upgrade a building, but the Building is at max Level already!");
            return;
        }
        if (currentBuilding.m_upgradeCost[currentBuilding.m_level +1] <= PlayerController.instance.gold)
        {
            PlayerController.instance.gold -= currentBuilding.m_upgradeCost[currentBuilding.m_level + 1];
            PlayerController.instance.gameUI.UpdateUI();
            currentBuilding.Upgrade();
        }
        else
        {
            Debug.Log("Trying to upgrade a building, but player has not enough gold!");
            return;
        }
        if(currentBuilding.m_level == currentBuilding.m_maxLevel) SetButtonsEnabled(new bool[] { false, false, false, false, false, false, false, true });
        UpdateUI();
    }

    void DemolishBuilding()
    {
        if (!currentBuilding){
            Debug.LogWarning("Trying to destroy a building, but these is nothing built in this slot!");
            return;
        }
        PlayerController.instance.gold += (int)(currentBuilding.m_upgradeCost[currentBuilding.m_level] * ((float)currentBuilding.m_health / (float)currentBuilding.m_maxHealth[currentBuilding.m_level]) * 0.9);
        PlayerController.instance.gameUI.UpdateUI();
        Destroy(currentBuilding.gameObject);
        currentBuilding = null;
        ShowBanner(true);
        SetButtonsEnabled(new bool[] { true, true, true, true, true, true, false, false });
        UpdateUI();
    }

    private void OnMouseExit()
    {
        foreach (Transform t in transform)
        {
            if (t.name == "BuildMenu0")
                continue;
            t.gameObject.SetActive(false);
        }
    }

    void UpdateUI(){
        costTexts[6].text = string.Empty;
        if(currentBuilding == null){
            for (int i = 0; i < 6; i++) {
                Building building = buildingPrefabs[i].GetComponent<Building>();
                costTexts[i].text = building.m_upgradeCost[0].ToString();
            }
        }
        else{
            if (currentBuilding.m_level != currentBuilding.m_maxLevel){

                costTexts[6].text = currentBuilding.m_upgradeCost[currentBuilding.m_level + 1].ToString();
                return;
            }
        }
    }
}
