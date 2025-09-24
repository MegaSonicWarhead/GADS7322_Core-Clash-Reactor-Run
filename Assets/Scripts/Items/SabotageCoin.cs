using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SabotageCoin : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip collectSound; // Sound to play once on pickup
    private AudioSource audioSource;
    private bool collected = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return; // Prevent double collection

        // Check if the collider has a PlayerController
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            collected = true;

            // Increment the correct player's coins
            SabotageSystem.Instance.AddCoin(player.playerId);

            // Play sound once
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Destroy the coin
            Destroy(gameObject);
        }
    }
}
