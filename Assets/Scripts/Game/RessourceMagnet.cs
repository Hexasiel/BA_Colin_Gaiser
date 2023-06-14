using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceMagnet : MonoBehaviour
{
    Collider2D effector;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("HITrg");
        if (collision.gameObject.layer == 11)
        {
            Debug.Log("HIT");
            Vector3 forceVector = transform.position - collision.transform.position;
            forceVector = forceVector.normalized;
            collision.GetComponent<Rigidbody>().AddForce(forceVector);
        }
    }
}
