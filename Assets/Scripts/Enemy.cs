using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //External References
    [SerializeField] protected GameObject target;
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] protected GameObject goldPiece;

    //Properties
    [SerializeField] protected float m_speed = 10f;
    [SerializeField] protected float m_attackSpeed = 1f;
    [SerializeField] protected int m_health = 10;
    [SerializeField] protected int m_damage = 1;
    [SerializeField] protected float m_attackRange = 10f;
    [SerializeField] protected bool m_canAttack = true;
    [SerializeField] protected bool m_targetInRange = false;

    protected virtual void Move()
    {
        //TBI
    }

    protected virtual void Attack()
    {
        //TBI
    }

    public void GetDamage(int dmg)
    {
        m_health -= dmg;
        if (m_health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        Instantiate(goldPiece, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected void DetermineTarget()
    {
        target = null;

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (GameObject building in buildings)
        {
            if(target == null || (building.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude)
            {
                target = building;
            }
        }
    }
}
