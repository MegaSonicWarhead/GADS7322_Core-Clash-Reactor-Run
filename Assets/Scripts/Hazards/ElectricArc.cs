using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricArc : MonoBehaviour
{
    public float onTime = 2f;
    public float offTime = 2f;
    public int damage = 20; // Damage dealt to the player

    private bool active = true;
    private float timer;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Make sure it loops
        audioSource.playOnAwake = false; // Don’t auto-play
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (active && timer > onTime) ToggleArc(false);
        else if (!active && timer > offTime) ToggleArc(true);
    }

    private void ToggleArc(bool state)
    {
        active = state;
        timer = 0f;

        // Enable/disable visuals
        var sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = state;

        // Enable/disable collider
        var col = gameObject.GetComponent<Collider2D>();
        if (col != null) col.enabled = state;

        // Handle sound
        if (audioSource != null)
        {
            if (state && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else if (!state && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && collision.CompareTag("Player"))
        {
            // Deal damage
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            // Optional: Knockback effect
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
            }
        }
    }
}
