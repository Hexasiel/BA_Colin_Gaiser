using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingMenu parentMenu;
    public int m_health;
    public int[] m_maxHealth;
    public int[] m_upgradeCost;
    public int m_level;
    public int m_maxLevel;
    public Sprite[] m_sprites;

    public void GetDamage(int dmg)
    {
        m_health -= dmg;
        if(m_health <= 0)
        {
            Die();
        }
    }

    public virtual void Upgrade()
    {
        if(m_level < m_maxLevel)
        {
            m_level += 1;
        }
        GetComponent<SpriteRenderer>().sprite = m_sprites[m_level];
        m_health = m_maxHealth[m_level];
    }

    protected virtual void Die()
    {
        parentMenu.ShowBanner(true);
        Destroy(gameObject);
    }
}
