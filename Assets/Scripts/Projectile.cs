using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int m_damage = 2;
    public bool explode = false;
    public float explosionRange = 1;
    [SerializeField] ParticleSystem ps;
    [SerializeField] GameObject hitTrigger;
    public LayerMask damageMask;
    public LayerMask hitMask;

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.transform.tag == "Enemy"){
            collision.gameObject.GetComponent<Enemy>().GetDamage(m_damage);
        }

        if(explode){
            if (collision.transform.tag == "Terrain"){
                StartCoroutine(Explode());
                return;
            }
        }
        GameObject.Destroy(this.gameObject);
    }

    IEnumerator Explode(){
        ps.Play();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRange, damageMask);
        foreach(Collider2D collider2D in hitEnemies) {
            collider2D.gameObject.GetComponent<Enemy>().GetDamage(m_damage);
        }
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
