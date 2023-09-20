using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int m_damage = 2;

    [Header("Wwise")] 
    public AK.Wwise.Event wwEvent_Fired;
    public AK.Wwise.Event wwEvent_Hit;

    private void Awake() {
        wwEvent_Fired.Post(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        wwEvent_Hit.Post(gameObject);
        if (collision.transform.tag == "Building" || collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<IAttackable>().GetDamage(m_damage);
        }
        GameObject.Destroy(this.gameObject);
    }
}
