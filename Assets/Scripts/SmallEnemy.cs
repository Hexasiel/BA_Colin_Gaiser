using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmallEnemy : Enemy
{
    [SerializeField] GameObject bulletPrefab;
    private void Update() {

        DetermineTarget();
        Move();
        if (m_targetInRange){
            if (m_canAttack){
                StartCoroutine(AttackCoolDown());
                Attack();
            }
        }
    }
    protected override void Move()
    {
        
        if (target == null)
            return;
        base.Move();
        if (!m_targetInRange)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target.transform.position, m_speed * Time.deltaTime);
            newPos.y = transform.position.y;
            transform.position = newPos;
        }

        Vector3 targetVector = (target.transform.position - transform.position);
        if(targetVector.x >= 0 )
            sprite.flipX = false;
        else
            sprite.flipX = true;
        if (targetVector.magnitude < m_attackRange)
            m_targetInRange = true;
        else
            m_targetInRange = false;

    }

    protected override void Attack()
    {
        if (target == null)
            return;
        base.Attack();
        Building bd = target.GetComponent<Building>();
        if (bd == null) { Debug.LogWarning("Trying to attack an non Building"); return; }


        Vector3 directon = target.transform.position;
        directon.z = 0;
        directon = (directon - transform.position).normalized;

        Rigidbody2D bulletrb = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletrb.AddForce(directon * 2000);
    }

    IEnumerator AttackCoolDown()
    {
        m_canAttack = false;
        yield return new WaitForSeconds(1f / m_attackSpeed);
        m_canAttack= true;
    }
}
