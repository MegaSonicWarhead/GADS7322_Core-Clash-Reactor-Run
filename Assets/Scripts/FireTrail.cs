using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    [Header("Settings")]
    public float followTime = 3f;           // How long the fire follows
    public float damagePerSecond = 10f;     // Damage per second
    public float followSpeed = 5f;          // How fast the fire catches up
    public float positionDelay = 0.2f;      // Delay between trail positions
    public GameObject fireResiduePrefab;    // Prefab for the residue fire
    public float residueLifetime = 2f;      // How long each residue stays

    private Transform target;
    private float timer;
    private Queue<Vector3> pastPositions = new Queue<Vector3>();
    private float recordTimer;

    public void Initialize(Transform followTarget)
    {
        target = followTarget;
        timer = followTime;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Record target positions for delayed movement
        recordTimer += Time.deltaTime;
        if (recordTimer >= positionDelay)
        {
            Vector3 pos = target.position;
            pastPositions.Enqueue(pos);

            // Spawn residue at the current fire position
            SpawnResidue(pos);

            recordTimer = 0f;
        }

        // Move toward the oldest recorded position
        if (pastPositions.Count > 0)
        {
            Vector3 nextPos = pastPositions.Peek();
            transform.position = Vector3.MoveTowards(transform.position, nextPos, followSpeed * Time.deltaTime);

            // Remove the position once reached
            if (Vector3.Distance(transform.position, nextPos) < 0.01f)
                pastPositions.Dequeue();
        }
    }

    private void SpawnResidue(Vector3 position)
    {
        if (fireResiduePrefab != null)
        {
            Vector3 spawnPos = new Vector3(position.x, position.y, -0.1f); // slightly in front
            GameObject residue = Instantiate(fireResiduePrefab, spawnPos, Quaternion.identity);
            residue.name = "FireResidue_Debug";
            Debug.Log("Residue spawned at " + spawnPos);
            Destroy(residue, residueLifetime);
        }
        else
        {
            Debug.LogWarning("FireResiduePrefab is not assigned!");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Only damage the target player
        if (other.transform == target)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
            }
        }
    }
}
