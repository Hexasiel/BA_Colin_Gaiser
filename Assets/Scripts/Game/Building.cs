using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour, IAttackable {

    public static event Action<int> OnBuildingDamaged;
    public static event Action OnBuildingDestroyed;
    public static event Action<int> OnBuildingHealed;

    public static int count;

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
        count++;
    }
    
    public void ReceiveHeal(int health)
    {
        if (healthbar == null || gameObject == null) return;

        if (m_health + health > m_maxHealth[m_level]) {
            OnBuildingHealed?.Invoke(m_maxHealth[m_level] - m_health);
            m_health = m_maxHealth[m_level];
            Debug.Log(gameObject);
            healthbar.gameObject.SetActive(false);
        }
        else {
            m_health += health;
            OnBuildingHealed?.Invoke(health);
        }
        healthbar.value = (float)m_health / (float)m_maxHealth[m_level];
    }

    void IAttackable.GetDamage(int dmg)
    {
        OnBuildingDamaged?.Invoke(dmg);
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
        m_health = m_maxHealth[m_level] - m_maxHealth[m_level - 1] + m_health;
        healthbar.value = (float)m_health / (float)m_maxHealth[m_level];
    }

    protected virtual void Die()
    {
        Shrine.OnHealingTriggered -= ReceiveHeal;
        OnBuildingDestroyed?.Invoke();
        parentMenu.ShowBanner(true);
        parentMenu.SetButtonsEnabled(new bool[] { true, true, true, true, true, true, false, false });
        count--;
        Destroy(gameObject);
    }
}
