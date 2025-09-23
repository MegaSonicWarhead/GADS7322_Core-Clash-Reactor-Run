using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableCrate : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 initialPosition;

    [Header("Reset Settings")]
    public float resetInterval = 30f; // seconds

    private bool isOnPressurePlate = false;
    private bool isPressurePlateActivated = false;

    // --- Carrying ---
    private bool isCarried = false;
    private Transform carrier; // reference to player carrying the crate
    public float throwForce = 8f; // force when thrown
    public Vector3 carryOffset = new Vector3(0.8f, 0.5f, 0); // offset from player

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (isCarried && carrier != null)
        {
            // Get the player's sprite renderer to check facing
            SpriteRenderer playerRenderer = carrier.GetComponent<SpriteRenderer>();
            float side = 1f; // default right

            if (playerRenderer != null && playerRenderer.flipX)
                side = -1f; // left

            // Apply offset on the correct side
            Vector3 offset = new Vector3(carryOffset.x * side, carryOffset.y, 0);
            transform.position = carrier.position + offset;
        }
    }

    public void PickUp(Transform player)
    {
        if (isCarried)
        {
            Debug.LogWarning("Tried to pick up crate, but it's already carried!");
            return;
        }

        Debug.Log("Crate picked up by " + player.name);

        isCarried = true;
        carrier = player;

        rb.isKinematic = true;
        rb.simulated = false; // disable physics while carried
    }

    public void Throw()
    {
        if (!isCarried) return;

        isCarried = false;
        rb.isKinematic = false;
        rb.simulated = true;

        // Determine facing direction from player sprite flip
        float facingDir = 1f;
        var playerRenderer = carrier.GetComponent<SpriteRenderer>();
        if (playerRenderer != null && playerRenderer.flipX)
            facingDir = -1f;

        // Place crate slightly on the correct side of player to avoid collision
        transform.position = carrier.position + new Vector3(facingDir * carryOffset.x, carryOffset.y, 0);

        // Throw forward and slightly upward
        Vector2 throwDir = new Vector2(facingDir, 1f).normalized;

        rb.velocity = Vector2.zero; // reset velocity
        rb.AddForce(throwDir * throwForce, ForceMode2D.Impulse);

        Debug.Log("Crate thrown. Dir=" + throwDir + " Force=" + (throwDir * throwForce));

        carrier = null;
    }

    public void Drop()
    {
        if (!isCarried) return;

        isCarried = false;
        rb.isKinematic = false;
        rb.simulated = true;

        // Place crate at current carrier position with offset
        SpriteRenderer playerRenderer = carrier.GetComponent<SpriteRenderer>();
        float side = 1f;
        if (playerRenderer != null && playerRenderer.flipX)
            side = -1f;

        transform.position = carrier.position + new Vector3(carryOffset.x * side, carryOffset.y, 0);

        Debug.Log("Crate dropped by " + carrier.name);

        carrier = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PressurePlate"))
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
        //StartCoroutine(ResetLoop());
    }

    //private IEnumerator ResetLoop()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(resetInterval);

    //        // Don’t reset while carried
    //        if (!isCarried && (!isOnPressurePlate || !isPressurePlateActivated))
    //        {
    //            rb.velocity = Vector2.zero;
    //            rb.angularVelocity = 0f;
    //            transform.position = initialPosition;
    //        }
    //    }
    //}

    public void SetPressurePlateStatus(bool isActivated)
    {
        isPressurePlateActivated = isActivated;
    }

    public bool IsCarried()
    {
        return isCarried;
    }
}
