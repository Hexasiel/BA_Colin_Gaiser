using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int m_damage = 2;
    public bool explode = false;
    [SerializeField] ParticleSystem ps;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().GetDamage(m_damage);
        }

        if(explode)
        {
            if (collision.transform.tag == "Terrain")
            {
                Explode();
                return;
            }
        }
        GameObject.Destroy(this.gameObject);
    }

    void Explode()
    {
        ps.gameObject.SetActive(true);
    }
}
