using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageTower : Building
{
    //References
    protected GameObject target;
    [SerializeField] GameObject bulletPrefab;

    //Attributes
    [SerializeField] bool m_targetInRange = false;
    [SerializeField] float[] m_attackRange;
    [SerializeField] float[] m_attackSpeed;
    [SerializeField] bool m_canAttack = true;

    private void Update()
    {
        FindTarget();
        if (m_targetInRange){
            if (m_canAttack){
                StartCoroutine(AttackCoolDown());
                Attack();
            }
        }
    }

    void FindTarget()
    {
        target = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length < 1) {
            m_targetInRange = false;
            return;
        }

        foreach (GameObject enemy in enemies){
            if (target == null || (enemy.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude){
                target = enemy;
            }
        }
        Vector3 targetVector = (target.transform.position - transform.position);
        if (targetVector.magnitude < m_attackRange[m_level])
            m_targetInRange = true;
        else
            m_targetInRange = false;
    }

    IEnumerator AttackCoolDown()
    {
        m_canAttack = false;
        yield return new WaitForSeconds(1f / m_attackSpeed[m_level]);
        m_canAttack = true;
    }

    void Attack()
    {
        Vector3 directon = target.transform.position;
        directon.z = 0;
        directon = (directon - transform.position).normalized;


        Rigidbody2D bulletrb = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletrb.AddForce(directon * 2000);
    }
}
