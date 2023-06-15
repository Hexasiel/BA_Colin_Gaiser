using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour, IAttackable
{
    public BuildingMenu parentMenu;
    public int m_health;
    public int[] m_maxHealth;
    public int[] m_upgradeCost;
    public int m_level;
    public int m_maxLevel;
    public Sprite[] m_sprites;
    [SerializeField] protected Slider healthbar;

    private void Awake()
    {
        Shrine.OnHealingTriggered += ReceiveHeal;
    }
    
    void ReceiveHeal(int health)
    {
        m_health += health;
        if (m_health > m_maxHealth[m_level]) {
            m_health = m_maxHealth[m_level];
            healthbar.gameObject.SetActive(false);
        }
        PlayerController.instance.gameUI.UpdateUI();

    }

    void IAttackable.GetDamage(int dmg)
    {
        m_health -= dmg;
        healthbar.gameObject.SetActive(true);
        healthbar.value = (float)m_health / (float)m_maxHealth[m_level];
        if (m_health <= 0)
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
