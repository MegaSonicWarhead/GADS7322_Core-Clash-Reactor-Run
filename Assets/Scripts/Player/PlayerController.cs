using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar; // World Space slider
    public Vector3 healthBarOffset = new Vector3(0, 1.5f, 0);

    [Header("Interaction")]
    public float interactRange = 1f;
    //public LayerMask interactableLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    private float inputX;
    private bool jumpPressed;

    public int playerId = 1; // 1 = P1, 2 = P2

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateHealthBarPosition();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // ------------------------
    // Input Handling
    // ------------------------
    private void HandleInput()
    {
        // --- Player 1 Controls ---
        if (playerId == 1)
        {
            inputX = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space))
                jumpPressed = true;

            if (Input.GetKeyDown(KeyCode.E))
                TryInteract();

            // Sabotages (keys 1–5)
            if (Input.GetKeyDown(KeyCode.Alpha1)) TrySabotage(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) TrySabotage(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) TrySabotage(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) TrySabotage(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) TrySabotage(4);
        }
        // --- Player 2 Controls (Numpad) ---
        else if (playerId == 2)
        {
            if (Input.GetKey(KeyCode.Keypad4)) inputX = -1f;
            else if (Input.GetKey(KeyCode.Keypad6)) inputX = 1f;
            else inputX = 0f;

            if (Input.GetKeyDown(KeyCode.Keypad0))
                jumpPressed = true;

            if (Input.GetKeyDown(KeyCode.Keypad7))
                TryInteract();

            // Sabotages (keys 6–0)
            if (Input.GetKeyDown(KeyCode.Alpha6)) TrySabotage(0);
            if (Input.GetKeyDown(KeyCode.Alpha7)) TrySabotage(1);
            if (Input.GetKeyDown(KeyCode.Alpha8)) TrySabotage(2);
            if (Input.GetKeyDown(KeyCode.Alpha9)) TrySabotage(3);
            if (Input.GetKeyDown(KeyCode.Alpha0)) TrySabotage(4);
        }
    }

    // ------------------------
    // Movement
    // ------------------------
    private void HandleMovement()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        jumpPressed = false;
    }

    // ------------------------
    // Health
    // ------------------------
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Debug.Log("Player " + playerId + " died!");
    }

    private void UpdateHealthBarPosition()
    {
        if (healthBar != null)
        {
            healthBar.transform.position = transform.position + healthBarOffset;
        }
    }

    // ------------------------
    // Interactions
    // ------------------------
    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Switch"))
            {
                var switchComponent = hit.GetComponent<Switch>();
                if (switchComponent != null)
                {
                    switchComponent.Toggle();
                    return;
                }
            }
            else if (hit.CompareTag("PuzzleNode"))
            {
                var node = hit.GetComponent<PuzzleNode>();
                if (node != null && !node.IsPowered)
                {
                    node.SetCollectedBy(playerId);
                    node.Activate();
                    return;
                }
            }
            else if (hit.CompareTag("Reactor"))
            {
                LevelManager.Instance.TryFixReactor(playerId);
                return;
            }
        }
    }

    // ------------------------
    // Sabotages
    // ------------------------
    private void TrySabotage(int sabotageIndex)
    {
        SabotageSystem.Instance.TriggerSabotage(playerId, sabotageIndex);
    }

    // ------------------------
    // Debug Gizmos
    // ------------------------
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
