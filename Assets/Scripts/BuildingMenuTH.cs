using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingMenuTH : MonoBehaviour
{
    [SerializeField] GameObject banner;
    Transform childButton;
    [SerializeField] TextMeshPro costText;
    public Camera myCam;

    TownHall currentBuilding = null;
    Transform hoveredElement;

    public GameObject thPrefab;


    //
    int activeElement = 0;
    private LayerMask _layerMask;
    public Color defaultColor = new Color(1, 1, 1, 0.5f);
    public Color hoveredColor = new Color(1, 1, 1, 1f);

    private void Awake()
    {
        _layerMask = LayerMask.GetMask("UI");
        myCam = Camera.main;
        childButton = transform.GetChild(0);
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

    private void OnMouseEnter()
    {
        if (PlayerController.instance.battleMode)
            return;
        SetActiveElement(0);
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SetActiveElement(int elementIndex)
    {
        childButton.GetComponent<SpriteRenderer>().color = hoveredColor;
        activeElement = elementIndex;
    }

    private void OnMouseUpAsButton()
    {
        if (PlayerController.instance.battleMode)
            return;
        if (currentBuilding == null)
            BuildTownHall();
        else
            UpgradeBuilding();
    }

    void BuildTownHall()
    {
        if (currentBuilding)
        {
            Debug.LogWarning("Trying to build, but there is alredy a building in this slot!");
            return;
        }
        currentBuilding = Instantiate(thPrefab, transform.parent).GetComponent<TownHall>();
        PlayerController.instance.gold -= currentBuilding.m_upgradeCost[0];
        PlayerController.instance.gameUI.UpdateUI();
        ShowBanner(false);
        UpdateUI();
    }

    void UpgradeBuilding()
    {
        if (currentBuilding == null)
        {
            Debug.LogWarning("Trying to upgrade a building, but these is nothing built in this slot!");
            return;
        }
        if (currentBuilding.m_level == currentBuilding.m_maxLevel)
        {
            Debug.LogWarning("Trying to upgrade a building, but the Building is at max Level already!");
            return;
        }
        if (currentBuilding.m_upgradeCost[currentBuilding.m_level+1] <= PlayerController.instance.gold)
        {
            PlayerController.instance.gold -= currentBuilding.m_upgradeCost[currentBuilding.m_level +1];
            currentBuilding.Upgrade();
            PlayerController.instance.gameUI.UpdateUI();
        }
        else
        {
            Debug.Log("Trying to upgrade a building, but player has not enough gold!");
            return;
        }
        UpdateUI();
    }


    private void OnMouseExit()
    {
        foreach (Transform t in transform)
        {
            if (t.name == "BuildMenu0 (1)")
                continue;
            t.gameObject.SetActive(false);
        }
    }

    void UpdateUI()
    {
        costText.text = string.Empty;
        if (currentBuilding == null)
        {
             costText.text = thPrefab.GetComponent<TownHall>().m_upgradeCost[0].ToString();
        }
        else
        {
            if (currentBuilding.m_level != currentBuilding.m_maxLevel)
            {
                costText.text = currentBuilding.m_upgradeCost[currentBuilding.m_level + 1].ToString();
                return;
            }
            else
            {

            }
        }


    }
}
