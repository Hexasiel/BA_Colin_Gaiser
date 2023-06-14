using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //Events
    static int count;
    public static event Action OnAllEnemiesDefeated;

    //External References
    [SerializeField] protected GameObject target;
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] protected GameObject goldPiece;
    [SerializeField] protected Slider healthbar;

    //Properties
    [SerializeField] protected float m_speed = 10f;
    [SerializeField] protected float m_attackSpeed = 1f;
    [SerializeField] protected int m_maxHealth = 10;
    [SerializeField] protected int m_health = 10;
    [SerializeField] protected int m_damage = 1;
    [SerializeField] protected float m_attackRange = 10f;
    [SerializeField] protected bool m_canAttack = true;
    [SerializeField] protected bool m_targetInRange = false;
    [SerializeField] protected LayerMask attackLayer;

    //Internal Variables
    protected bool isStunned = false;

    private void Awake() {
        count++;
    }

    protected virtual void Move()
    {
        if (target == null)
            return;

        if (!m_targetInRange && !isStunned)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target.transform.position, m_speed * Time.deltaTime);
            newPos.y = transform.position.y;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            transform.position = newPos;
        }

        Vector3 targetVector = (target.transform.position - transform.position);
        if (targetVector.x >= 0)
            sprite.flipX = false;
        else
            sprite.flipX = true;
        if (targetVector.magnitude < m_attackRange)
            m_targetInRange = true;
        else
            m_targetInRange = false;
    }

    protected virtual void Attack()
    {
        //TBI
    }

    public void GetDamage(int dmg)
    {
        m_health -= dmg;
        StartCoroutine(DamageEffect());
        healthbar.gameObject.SetActive(true);
        healthbar.value = (float)m_health / (float)m_maxHealth;
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
        healthbar.gameObject.SetActive(true);
        healthbar.value = (float)m_health / (float)m_maxHealth;
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
        count--;
        if(count == 0) OnAllEnemiesDefeated?.Invoke();

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
