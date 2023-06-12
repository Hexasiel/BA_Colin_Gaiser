using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeEnemy : Enemy
{
    [SerializeField] GameObject bulletPrefab;

    private void Update()
    {
        DetermineTarget();
        Move();
        if (m_targetInRange)
        {
            if (m_canAttack)
            {
                StartCoroutine(AttackCoolDown());
                Attack();
            }
        }
    }

    protected override void Move()
    {
        base.Move();
    }

    protected override void Attack()
    {
        if (target == null)
            return;
        base.Attack();

        Vector3 directon = target.transform.position;
        directon.z = 0;
        directon = ((directon + Vector3.up) - transform.position).normalized;

        Rigidbody2D bulletrb = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletrb.AddForce(directon * 3000);
    }

    IEnumerator AttackCoolDown()
    {
        m_canAttack = false;
        yield return new WaitForSeconds(1f / m_attackSpeed);
        m_canAttack = true;
    }
}
