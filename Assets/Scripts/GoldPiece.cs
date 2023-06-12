using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPiece : MonoBehaviour
{
    public static event Action OnGoldPieceCollected;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            OnGoldPieceCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
