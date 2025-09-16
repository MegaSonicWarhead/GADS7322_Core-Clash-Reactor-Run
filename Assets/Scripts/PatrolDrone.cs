using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrolDrone : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;

    [Header("Movement")]
    public float speed = 2f;

    [Header("Detection & Shooting")]
    public float detectionRange = 3f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireCooldown = 1f;
    public int projectileDamage = 10;

    private Rigidbody2D rb;
    private Transform player;
    private int currentTargetIndex = 0;
    private List<Transform> patrolPoints = new List<Transform>();
    private bool forward = true;
    private float lastFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        patrolPoints = new List<Transform> { pointA, pointB, pointC, pointD };
        currentTargetIndex = 0;
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            SearchForPlayer();
        }

        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            // Stop and look at player
            LookAtPlayer();
            TryShoot();
        }
        else
        {
            player = null; // Reset player if out of range
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Count == 0)
            return;

        Transform target = patrolPoints[currentTargetIndex];

        // Move towards the target
        Vector2 newPos = Vector2.MoveTowards(rb.position, target.position, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Switch target if close enough
        if (Vector2.Distance(rb.position, target.position) < 0.1f)
        {
            if (forward)
            {
                currentTargetIndex++;
                if (currentTargetIndex >= patrolPoints.Count)
                {
                    currentTargetIndex = patrolPoints.Count - 2; // Start going backward
                    forward = false;
                }
            }
            else
            {
                currentTargetIndex--;
                if (currentTargetIndex < 0)
                {
                    currentTargetIndex = 1; // Start going forward again
                    forward = true;
                }
            }
        }

        // Optional: flip sprite based on movement direction
        if (rb.velocity.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(rb.velocity.x), 1f, 1f);
    }

    void SearchForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                player = hit.transform;
                break;
            }
        }
    }

    void LookAtPlayer()
    {
        if (player == null)
            return;

        // Flip sprite based on player position
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else
            transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void TryShoot()
    {
        if (Time.time - lastFireTime < fireCooldown || projectilePrefab == null || firePoint == null)
            return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (player.position - firePoint.position).normalized;

        // Simple projectile movement
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
            p.SetDirection(dir, projectileDamage);

        lastFireTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (pointA != null) Gizmos.DrawSphere(pointA.position, 0.2f);
        if (pointB != null) Gizmos.DrawSphere(pointB.position, 0.2f);
        if (pointC != null) Gizmos.DrawSphere(pointC.position, 0.2f);
        if (pointD != null) Gizmos.DrawSphere(pointD.position, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
