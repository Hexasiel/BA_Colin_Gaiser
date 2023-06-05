using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int m_damage = 2;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Building")
        {
            collision.gameObject.GetComponent<Building>().GetDamage(m_damage);
        }
        GameObject.Destroy(this.gameObject);
    }
}
