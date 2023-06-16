using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageTower : Building {
    //References
    protected GameObject target;
    [SerializeField] GameObject bulletPrefab;

    //Attributes
    [SerializeField] bool m_targetInRange = false;
    [SerializeField] float[] m_attackRange;
    [SerializeField] float[] m_attackSpeed;
    [SerializeField] int[] m_attackDamage;
    [SerializeField] bool m_canAttack = true;

    private void Update() {
        FindTarget();
        if (m_targetInRange) {
            if (m_canAttack) {
                StartCoroutine(AttackCoolDown());
                Attack();
            }
        }
    }

    void FindTarget() {
        target = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length < 1) {
            m_targetInRange = false;
            return;
        }

        foreach (GameObject enemy in enemies) {
            if (enemy.GetComponent<FlyingEnemy>() != null) { continue; }
            if (target == null || (enemy.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude) {
                target = enemy;
            }
        }
        if(target!= null) { Debug.LogWarning("Could not find Enemies."); m_targetInRange = false; return; }
        Vector3 targetVector = (target.transform.position - transform.position);
        if (targetVector.magnitude < m_attackRange[m_level])
            m_targetInRange = true;
        else
            m_targetInRange = false;
    }

    IEnumerator AttackCoolDown() {
        m_canAttack = false;
        yield return new WaitForSeconds(1f / m_attackSpeed[m_level]);
        m_canAttack = true;
    }

    void Attack() {
        Vector3 directon = GetGroundPos(target.transform.position);
        directon.z = 0;
        directon = (directon - transform.position).normalized;


        Rigidbody2D bulletrb = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletrb.AddForce(directon * 2000);

        Projectile projectile = bulletrb.GetComponent<Projectile>();
        if (projectile != null) { projectile.m_damage = m_attackDamage[m_level]; }
    }

    Vector3 GetGroundPos(Vector3 pos) {

        LayerMask mask = LayerMask.GetMask("Terrain");

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 100, mask);

        if (hit.collider != null) {
            pos.y = hit.point.y;
        }
        return pos;
    }
}
