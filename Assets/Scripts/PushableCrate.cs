using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableCrate : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 initialPosition;

    [Header("Reset Settings")]
    public float resetInterval = 25f; // seconds

    // Track if the crate is on a pressure plate
    private bool isOnPressurePlate = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        initialPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Optional: add sound effect
        }
        else if (collision.collider.CompareTag("PressurePlate"))
        {
            isOnPressurePlate = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PressurePlate"))
        {
            isOnPressurePlate = false;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ResetLoop());
    }

    private IEnumerator ResetLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(resetInterval);

            if (!isOnPressurePlate)
            {
                // Stop all movement
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;

                // Reset position
                transform.position = initialPosition;
            }
        }
    }
}
