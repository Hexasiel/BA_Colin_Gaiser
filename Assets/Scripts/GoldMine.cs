using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMine : Building
{
    [SerializeField] float[] productionTimes;
    [SerializeField] protected GameObject goldPiece;

    private void Start()
    {
        StartCoroutine(ProduceGold());
    }

    IEnumerator ProduceGold()
    {
        while(true){
           yield return new WaitForSeconds(productionTimes[m_level]);
            DropGold();
        }
    }

    void DropGold()
    {
        Instantiate(goldPiece, transform.position, Quaternion.identity);
    }

}
