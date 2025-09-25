using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float speed = 8f;
    public float explosionRadius = 1.5f;
    public int damage = 20; // missile damage

    [Header("Audio Sources")]
    public AudioSource spawnSource;     // plays when missile spawns
    public AudioSource explodeSource;   // plays when missile explodes
    public bool spawnedBySabotage = false;

    [Header("Audio Clips")]
    public AudioClip spawnClip;
    public AudioClip explodeClip;

    private bool hasPlayedSpawnSound = false;
    private bool hasExploded = false;

    private void Start()
    {
        if (!spawnedBySabotage)
        {
            Debug.LogWarning("Missile exists in scene without being spawned via SabotageSystem! Destroying...");
            Destroy(gameObject);
            return;
        }

        PlaySpawnSound();
    }

    private void PlaySpawnSound()
    {
        if (!hasPlayedSpawnSound && spawnSource != null && spawnClip != null)
        {
            spawnSource.PlayOneShot(spawnClip);
            hasPlayedSpawnSound = true;
        }
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Rotate missile to face target
        Vector3 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // Move forward
        transform.position += dir * speed * Time.deltaTime;

        // Explode when close
        if (!hasExploded && Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        // Play explosion sound at missile position
        if (explodeClip != null)
        {
            AudioSource.PlayClipAtPoint(explodeClip, transform.position);
        }

        // Apply damage in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null && hit.transform == target)
            {
                player.TakeDamage(damage);
                Debug.Log($"{target.name} took {damage} damage from a missile!");
            }
        }

        // Destroy missile immediately; sound persists
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize explosion radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
