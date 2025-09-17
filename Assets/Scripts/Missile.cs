using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float speed = 8f;
    public float explosionRadius = 1.5f;
    public int damage = 20; // missile damage

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        // Rotate missile to face target
        Vector3 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // so missile looks "up"

        // Move forward
        transform.position += dir * speed * Time.deltaTime;

        // Explode when close
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            Explode();
        }
    }

    void Explode()
    {
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

        Destroy(gameObject);
    }
}
