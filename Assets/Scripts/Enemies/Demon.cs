using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform target;
    public float speed = 3f;
    public float lifeTime = 30f;
    public int damage = 10;

    private float timer;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Spawn Settings")]
    public float spawnHeight = 3f;
    public float spawnOffsetX = 2f;

    [Header("Audio Settings")]
    public AudioClip spawnSound;   // Assign your spawn sound in inspector
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Make sure AudioSource exists
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 1f;
        audioSource.spatialBlend = 0f; // 0 = 2D sound
    }

    private void Start()
    {
        timer = lifeTime;

        // Spawn above player
        if (target != null)
        {
            Vector2 spawnPos = (Vector2)target.position + new Vector2(Random.Range(-spawnOffsetX, spawnOffsetX), spawnHeight);
            transform.position = spawnPos;
        }

        // Play spawn sound once
        if (spawnSound != null)
        {
            audioSource.clip = spawnSound;
            audioSource.Play();
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) Destroy(gameObject);

        if (target != null)
        {
            MoveTowardsPlayer();
        }

        FlipSprite();
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }

    private void FlipSprite()
    {
        if (sr != null)
            sr.flipX = rb.velocity.x < 0;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        PlayerController player = col.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
