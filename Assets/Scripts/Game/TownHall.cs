using EmotivUnityPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHall : Building
{
    public static event Action OnTownHallBuilt;

    [SerializeField] GameObject[] buildslots;

    private void Awake()
    {
        buildslots[0] = Instantiate(buildslots[0]);
        buildslots[1] = Instantiate(buildslots[1]);
        buildslots[2] = Instantiate(buildslots[2]);
        buildslots[m_level].SetActive(true);
        Shrine.OnHealingTriggered += ReceiveHeal;
        OnTownHallBuilt?.Invoke();
    }

    protected override void Die()
    {
        Time.timeScale = 0;
        PlayerController.instance.gameUI.ShowDeathScreen();
    }

    public override void Upgrade()
    {
        base.Upgrade();
        buildslots[m_level].SetActive(true);
    }

    void OnApplicationQuit() {
        Debug.Log("Application ending after " + Time.time + " seconds");
        EmotivUnityItf.Instance.Stop();
    }
}
