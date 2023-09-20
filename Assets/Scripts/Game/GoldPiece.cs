using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPiece : MonoBehaviour
{
    public static event Action OnGoldPieceCollected;
    [Header("Wwise")] 
    public AK.Wwise.Event wwEvent_HitGround;
    public AK.Wwise.Event wwEvent_PickUp;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player") {
            wwEvent_PickUp.Post(gameObject);
            OnGoldPieceCollected?.Invoke();
            Destroy(gameObject);
        }

        wwEvent_HitGround.Post(gameObject);
    }
}
