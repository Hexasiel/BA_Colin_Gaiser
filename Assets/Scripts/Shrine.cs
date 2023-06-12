using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Shrine : Building
{
    [SerializeField] int[] healingAmount;

    public static event Action<int> OnHealingTriggered;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HealEverySecond());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Heal()
    {
        OnHealingTriggered?.Invoke(healingAmount[m_level]);
    }

    IEnumerator HealEverySecond(){
        while (gameObject){
            Heal();
            yield return new WaitForSeconds(1);
        }
    }
}
