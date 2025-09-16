using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float speed = 8f;
    public float explosionRadius = 1.5f;

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

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
            if (hit.transform == target)
            {
                Debug.Log($"{target.name} got hit by a missile!");
            }
        }

        Destroy(gameObject);
    }
}
