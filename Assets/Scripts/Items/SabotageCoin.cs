using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabotageCoin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider has a PlayerController
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            // Increment the correct player's coins
            SabotageSystem.Instance.AddCoin(player.playerId);

            // Destroy the coin
            Destroy(gameObject);
        }
    }
}
