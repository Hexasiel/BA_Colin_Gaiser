using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHall : Building
{
    protected override void Die()
    {
        Time.timeScale = 0;
    }
}
