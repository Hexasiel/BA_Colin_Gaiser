using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuElement : MonoBehaviour
{
    BuildingMenu parentMenu;

    private void OnMouseEnter()
    {
        transform.parent.GetComponent<BuildingMenu>().SetActiveElement(transform.GetSiblingIndex());
        Debug.Log(transform.GetSiblingIndex());
    }
}
