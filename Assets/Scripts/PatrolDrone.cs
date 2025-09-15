using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrolDrone : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Transform currentTarget;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTarget = pointA;
    }

    void FixedUpdate()
    {
        if (currentTarget == null)
            return;

        // Move using Rigidbody for physics stability
        Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget.position, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Switch target when close
        if (Vector2.Distance(rb.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }
}
