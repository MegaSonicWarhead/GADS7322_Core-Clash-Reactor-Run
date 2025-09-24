using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public Transform target;          // The player
    public float speed = 3f;          // Walking speed
    public float jumpForce = 7f;      // Jump strength
    public float lifeTime = 30f;      // Demon lifetime
    public int damage = 10;           // Damage to player
    public float chaseRange = 15f;    // How close the player must be for the demon to chase
    public LayerMask groundLayer;     // Layer to detect ground/platforms
    public Transform groundCheck;     // Check if demon is on ground
    public float groundCheckRadius = 0.2f;
    public Transform frontCheck;      // Check in front for edges
    public float edgeCheckDistance = 0.5f;
    public float obstacleCheckDistance = 0.3f;
    public float platformDropDistance = 1f; // How far down it can drop safely

    private Rigidbody2D rb;
    private float timer;
    private bool isGrounded;
    private bool movingRight = true;
    private SpriteRenderer sr;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 3f;
    public float maxSpawnDistance = 8f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        timer = lifeTime;

        // Spawn near the player but not directly on them
        if (target != null)
        {
            Vector2 spawnOffset = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, maxSpawnDistance);
            transform.position = (Vector2)target.position + spawnOffset;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) Destroy(gameObject);

        CheckGrounded();

        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);

            if (distance <= chaseRange)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }

        FlipSprite();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Patrol()
    {
        HandleMovement(movingRight ? 1 : -1);
    }

    private void ChasePlayer()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        // Face the player
        movingRight = direction.x > 0;

        // Move toward the player
        HandleMovement(direction.x);
    }

    private void HandleMovement(float horizontal)
    {
        // Check for obstacles ahead
        RaycastHit2D obstacleHit = Physics2D.Raycast(frontCheck.position, Vector2.right * Mathf.Sign(horizontal), obstacleCheckDistance, groundLayer);
        // Check for edge
        RaycastHit2D groundHit = Physics2D.Raycast(frontCheck.position, Vector2.down, edgeCheckDistance, groundLayer);

        if ((groundHit.collider == null || obstacleHit.collider != null) && isGrounded)
        {
            // Change direction if hitting obstacle or edge
            movingRight = !movingRight;
            horizontal = movingRight ? 1 : -1;
        }

        // Move horizontally
        Vector2 velocity = rb.velocity;
        velocity.x = horizontal * speed;
        rb.velocity = velocity;

        // Jump logic
        if (isGrounded)
        {
            // Jump if player is higher
            if (target != null && target.position.y > transform.position.y + 0.5f)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            // Jump down if player is below but within a safe drop distance
            else if (target != null && transform.position.y - target.position.y > 0.5f)
            {
                RaycastHit2D dropHit = Physics2D.Raycast(groundCheck.position, Vector2.down, platformDropDistance, groundLayer);
                if (dropHit.collider == null)
                {
                    StartCoroutine(DropDown());
                }
            }
        }
    }

    private IEnumerator DropDown()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
            yield return new WaitForSeconds(0.3f); // adjust for platform height
            col.enabled = true;
        }
    }

    private void FlipSprite()
    {
        if (sr != null)
        {
            sr.flipX = !movingRight; // Flip sprite depending on moving direction
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        PlayerController player = col.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"{player.name} took {damage} damage from a Demon!");
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (frontCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(frontCheck.position, frontCheck.position + Vector3.down * edgeCheckDistance);
            Gizmos.DrawLine(frontCheck.position, frontCheck.position + Vector3.right * (movingRight ? 1 : -1) * obstacleCheckDistance);
        }
    }
}
