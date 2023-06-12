using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] float m_chargeTime;
    [SerializeField] Color m_chargeStartColor;
    [SerializeField] Color m_chargeEndColor;
    [SerializeField] float m_chargeStartScale;
    [SerializeField] float m_chargeEndScale;

    [SerializeField] SpriteRenderer chargeIndicator;
    [SerializeField] LayerMask hitLayer;

    private void Start()
    {
        StartCoroutine(Charge());
    }

    //IEnumerator Strike()
    //{
    //    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, attackMask);
    //    foreach (Collider2D collider2D in hitEnemies)
    //    {
    //        Vector2 knockback = (collider2D.transform.position - transform.position).normalized * attackKnockback;
    //        collider2D.gameObject.GetComponent<Enemy>().GetDamage(attackDamage, knockback);
    //    }
    //}

    IEnumerator Charge(){
        float chargeTimePassed = 0f;
        while(chargeTimePassed < m_chargeTime)
        {
            float progress = chargeTimePassed/ m_chargeTime;
            chargeIndicator.color = Color.Lerp(m_chargeStartColor, m_chargeEndColor,chargeTimePassed);
            float newScale = Mathf.Lerp(m_chargeStartScale, m_chargeEndScale, progress);
            chargeIndicator.transform.localScale = new Vector3( newScale, newScale/5, 1);
            chargeTimePassed += Time.deltaTime;
            yield return null;
        }
    }
}
