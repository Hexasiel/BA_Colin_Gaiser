using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] protected LayerMask attackLayer;

    //Internal Variables
    protected bool isStunned = false;

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
        StartCoroutine(DamageEffect());
        if (m_health <= 0)
        {
            Die();
        }
    }

    public void GetDamage(int dmg, Vector2 knockBack)
    {
        Debug.Log(knockBack);
        m_health -= dmg;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(knockBack);
        StartCoroutine(DamageEffect());
        if (m_health <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageEffect(){
        sprite.color = Color.red;
        isStunned = true;
        yield return new WaitForSeconds(0.5f);
        isStunned = false;
        sprite.color = Color.white;
    }

    protected void Die()
    {
        Instantiate(goldPiece, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected void DetermineTarget()
    {
        target = null;

        List<GameObject> buildings = GameObject.FindGameObjectsWithTag("Building").ToList<GameObject>();
        buildings.Add(PlayerController.instance.gameObject);
        foreach (GameObject building in buildings)
        {
            if(target == null || (building.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude)
            {
                target = building;
            }
        }
    }
}
