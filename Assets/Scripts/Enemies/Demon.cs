using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public Transform target;          // The player
    public int ownerPlayerId;
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

    [Header("Stuck Detection")]
    public float stuckCheckInterval = 0.5f;
    public float stuckThreshold = 0.1f; // minimal movement to count as "moving"
    public float maxStuckTime = 1.5f;   // how long demon can be stuck before reacting

    private float stuckTimer = 0f;
    private Vector2 lastPosition;
    private bool isStuck = false;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 3f;
    public float maxSpawnDistance = 8f;

    private static Dictionary<int, Demon> activeDemons = new Dictionary<int, Demon>();

    private void Start()
    {
        if (activeDemons.ContainsKey(ownerPlayerId))
        {
            Destroy(gameObject);
            return;
        }
        activeDemons[ownerPlayerId] = this;

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

        CheckStuck();
    }

    private void CheckStuck()
    {
        float distanceMoved = Mathf.Abs(transform.position.x - lastPosition.x);

        if (distanceMoved < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= maxStuckTime)
            {
                if (isGrounded)
                {
                    // Try jump first
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    Debug.Log("Demon jumped to try unstuck");
                }
                else
                {
                    // Flip direction if not grounded or jump didn't help
                    movingRight = !movingRight;
                    Debug.Log("Demon flipped direction to try unstuck");
                }

                stuckTimer = 0f; // Reset timer after attempting to fix
            }
        }
        else
        {
            stuckTimer = 0f; // Reset timer if moving
        }

        lastPosition = transform.position;
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
        Vector2 direction = (target.position - transform.position);

        // Determine horizontal direction to target
        float horizontal = Mathf.Sign(direction.x);
        movingRight = horizontal > 0;

        HandleMovement(horizontal);
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
            if (target != null)
            {
                float verticalDifference = target.position.y - transform.position.y;

                if (verticalDifference > 1f)
                {
                    // Check if wall/ledge in front (optional, to avoid jumping into walls)
                    RaycastHit2D wallCheck = Physics2D.Raycast(frontCheck.position, Vector2.right * Mathf.Sign(horizontal), 0.3f, groundLayer);
                    if (!wallCheck.collider)
                    {
                        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    }
                }
                else if (verticalDifference < -1f)
                {
                    // Target is below
                    TryDropThroughPlatform();
                }
            }
        }
    }

    private void TryDropThroughPlatform()
    {
        PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
        Collider2D col = GetComponent<Collider2D>();

        if (col != null && effector != null)
        {
            StartCoroutine(DropDownPlatform(effector, col));
        }
    }

    private IEnumerator DropDownPlatform(PlatformEffector2D effector, Collider2D col)
    {
        // Disable collision from top
        effector.rotationalOffset = 180f;
        yield return new WaitForSeconds(0.3f); // Time to fall
        effector.rotationalOffset = 0f;
    }

    private void FlipSprite()
    {
        if (sr != null && rb != null)
        {
            sr.flipX = rb.velocity.x < 0;
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
    private void OnDestroy()
    {
        if (activeDemons.ContainsKey(ownerPlayerId) && activeDemons[ownerPlayerId] == this)
        {
            activeDemons.Remove(ownerPlayerId);
        }
    }

    // Helper for SabotageSystem
    public static bool HasActiveDemon(int playerId)
    {
        return activeDemons.ContainsKey(playerId);
    }
}
