using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    [Header("Web Settings")]
    public float slowMultiplier = 0.5f;   // how much the player is slowed
    public float duration = 4f;           // how long the web follows
    private Transform targetPlayer;

    private void Start()
    {
        Destroy(gameObject, duration); // auto-destroy after duration
    }

    private void Update()
    {
        if (targetPlayer != null)
        {
            // Stick to the target player's position
            transform.position = targetPlayer.position;
        }
    }

    public void Initialize(Transform player)
    {
        targetPlayer = player;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null && other.transform == targetPlayer)
        {
            pc.moveSpeed *= slowMultiplier;
            Debug.Log($"{pc.name} is slowed by a spider web!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null && other.transform == targetPlayer)
        {
            // Restore original speed
            pc.moveSpeed /= slowMultiplier;
            Debug.Log($"{pc.name} escaped the spider web!");
        }
    }
}
