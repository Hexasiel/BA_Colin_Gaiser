using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Workshop : Building
{
    public static List<Workshop> workshopList;
    public static int workshopHighestLevel = 0;
    public static event Action OnWorkshopLevelUpdated;

    private void Start()
    {
        if(workshopList == null)
            workshopList = new List<Workshop>();
        workshopList.Add(this);
        UpdateWorkshopHighestLevel();
    }

    public static void UpdateWorkshopHighestLevel()
    {
        workshopHighestLevel = 0;
        foreach (var workshop in workshopList)
        {
            if (workshop.m_level+1 > workshopHighestLevel){
                workshopHighestLevel = workshop.m_level +1;
            }
        }
        OnWorkshopLevelUpdated?.Invoke();
    }

    public override void Upgrade()
    {
        base.Upgrade();
        UpdateWorkshopHighestLevel();
    }

    protected override void Die()
    {
        base.Die();
        workshopList.Remove(this);
        UpdateWorkshopHighestLevel();
    }
}
