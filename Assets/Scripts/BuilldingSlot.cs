using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilldingSlot : MonoBehaviour
{
    [SerializeField] GameObject buildingWindow;

    private void OnMouseEnter()
    {
       ShowBuildingWindow(true);
    }

    void ShowBuildingWindow(bool b)
    {
        buildingWindow.SetActive(b);
    }
}
