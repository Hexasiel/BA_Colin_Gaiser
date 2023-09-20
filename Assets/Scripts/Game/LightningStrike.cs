using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [Header("Charge Variables")]
    [SerializeField] float m_chargeTime;
    [SerializeField] Color m_chargeStartColor;
    [SerializeField] Color m_chargeEndColor;
    [SerializeField] float m_chargeStartScale;
    [SerializeField] float m_chargeEndScale;

    [Header("Explosion Variables")]
    [SerializeField] int m_explosionDamage;
    [SerializeField] float m_explosionRadius;
    [SerializeField] LayerMask m_hitLayer;

    [Header("Effect Variables")]
    [SerializeField] float m_lightningExpansionTime;
    [SerializeField] float m_lightningFadeTime;

    [Header("External References")]
    [SerializeField] SpriteRenderer m_lightningSprite;
    [SerializeField] SpriteRenderer chargeIndicator;

    [Header("Wwise")] 
    public AK.Wwise.Event wwEvent_Charge;
    public AK.Wwise.Event wwEvent_Explode;
    
    
    private void Start()
    {
        StartCoroutine(Charge());
    }

    void Strike()
    {
        wwEvent_Explode.Post(gameObject);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, m_explosionRadius, m_hitLayer);
        foreach (Collider2D collider2D in hitObjects)
        {
            //Enemy enemy = collider2D.GetComponent<Enemy>();
            //if (enemy) enemy.GetDamage(m_explosionDamage);
            IAttackable attackable = collider2D.GetComponent<IAttackable>();
            if(attackable != null) attackable.GetDamage(m_explosionDamage);
        }
    }

    IEnumerator LightningEffect() {
        
        float passedTime = 0;
        while(passedTime < m_lightningExpansionTime)
        {
            m_lightningSprite.material.SetFloat("_StrikeProgress", passedTime / m_lightningExpansionTime);
            passedTime += Time.deltaTime;
            yield return null;
        }
        Strike();
        passedTime = 0;
        while (passedTime < m_lightningFadeTime)
        {
            m_lightningSprite.material.SetFloat("_FadeProgress", passedTime / m_lightningFadeTime);
            passedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator Charge(){
        float chargeTimePassed = 0f;
        wwEvent_Charge.Post(gameObject);
        while(chargeTimePassed < m_chargeTime)
        {
            float progress = chargeTimePassed/ m_chargeTime;
            chargeIndicator.color = Color.Lerp(m_chargeStartColor, m_chargeEndColor,chargeTimePassed);
            float newScale = Mathf.Lerp(m_chargeStartScale, m_chargeEndScale, progress);
            chargeIndicator.transform.localScale = new Vector3( newScale, newScale/5, 1);
            chargeTimePassed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(LightningEffect());
    }
}
