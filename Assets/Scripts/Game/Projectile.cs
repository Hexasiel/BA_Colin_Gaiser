using System;
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

    [Header("Wwise")] 
    public AK.Wwise.Event wwEvent_Fired;
    public AK.Wwise.Event wwEvent_Hit;
    public AK.Wwise.Event wwEvent_Explode;

    private void Awake() {
        wwEvent_Fired.Post(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.transform.tag == "Enemy") {
            wwEvent_Hit.Post(gameObject);
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

    IEnumerator Explode() {
        wwEvent_Explode.Post(gameObject);
        ps.Play();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRange, damageMask);
        foreach(Collider2D collider2D in hitEnemies) {
            collider2D.gameObject.GetComponent<Enemy>().GetDamage(m_damage);
        }
        GetComponent<Rigidbody2D>().velocity= Vector3.zero;
        GetComponent<CircleCollider2D>().enabled= false;
        GetComponent<SpriteRenderer>().enabled= false;
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
