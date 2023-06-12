using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemy : Enemy
{
    [SerializeField] float attackDelay;
    [SerializeField] Animator animator;

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
        StartCoroutine(DelayedAttack());
    }

    IEnumerator DelayedAttack(){
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDelay);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, m_attackRange, attackLayer);
        foreach (Collider2D collider2D in hitEnemies){
            IAttackable attackable = collider2D.gameObject.GetComponent<IAttackable>();
            if(attackable == null) continue;
            attackable.GetDamage(m_damage);
        }
    }

    IEnumerator AttackCoolDown()
    {
        m_canAttack = false;
        yield return new WaitForSeconds(1f / m_attackSpeed);
        m_canAttack = true;
    }
}
