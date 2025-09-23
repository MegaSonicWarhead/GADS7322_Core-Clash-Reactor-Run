using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private SpriteRenderer spriteRenderer; // <- NEW

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
    public Slider healthBar;
    public Vector3 healthBarOffset = new Vector3(0, 1.5f, 0);

    [Header("Panels")]
    public GameObject sabotageItemPanel;

    [Header("Interaction")]
    public float interactRange = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;

    private float inputX;
    private bool jumpPressed;

    public int playerId = 1;

    private static bool player1Dead = false;
    private static bool player2Dead = false;

    // Add field
    private PushableCrate carriedCrate;
    public float carrySpeedMultiplier = 0.5f; // reduces speed while carrying

    // --- NEW ---
    private Vector3 startPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (sabotageItemPanel != null)
            sabotageItemPanel.SetActive(false);

        // Store starting position
        startPosition = transform.position;
    }

    private void Update()
    {
        HandleInput();
        UpdateHealthBarPosition();
        UpdateAnimations();
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
        if (playerId == 1)
        {
            inputX = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space))
                jumpPressed = true;

            if (Input.GetKeyDown(KeyCode.E))
                TryInteract();

            if (Input.GetKeyDown(KeyCode.Q))
                ToggleSabotagePanel();

            if (Input.GetKeyDown(KeyCode.LeftShift) && carriedCrate != null)
            {
                Debug.Log("Player " + playerId + " dropped crate");
                carriedCrate.Drop();
                carriedCrate = null;
                moveSpeed /= carrySpeedMultiplier; // restore speed
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) TrySabotage(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) TrySabotage(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) TrySabotage(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) TrySabotage(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) TrySabotage(4);

            // --- Reset key for Player 1 ---
            if (Input.GetKeyDown(KeyCode.R)) ResetPosition();
        }
        else if (playerId == 2)
        {
            if (Input.GetKey(KeyCode.Keypad4)) inputX = -1f;
            else if (Input.GetKey(KeyCode.Keypad6)) inputX = 1f;
            else inputX = 0f;

            if (Input.GetKeyDown(KeyCode.Keypad0))
                jumpPressed = true;

            if (Input.GetKeyDown(KeyCode.Keypad7))
                TryInteract();

            if (Input.GetKeyDown(KeyCode.Keypad9))
                ToggleSabotagePanel();

            if (Input.GetKeyDown(KeyCode.RightShift) && carriedCrate != null)
            {
                Debug.Log("Player " + playerId + " dropped crate");
                carriedCrate.Drop();
                carriedCrate = null;
                moveSpeed /= carrySpeedMultiplier;
            }

            if (Input.GetKeyDown(KeyCode.Alpha6)) TrySabotage(0);
            if (Input.GetKeyDown(KeyCode.Alpha7)) TrySabotage(1);
            if (Input.GetKeyDown(KeyCode.Alpha8)) TrySabotage(2);
            if (Input.GetKeyDown(KeyCode.Alpha9)) TrySabotage(3);
            if (Input.GetKeyDown(KeyCode.Alpha0)) TrySabotage(4);

            // --- Reset key for Player 2 (Keypad2) ---
            if (Input.GetKeyDown(KeyCode.Keypad2)) ResetPosition();
        }
    }

    private void ToggleSabotagePanel()
    {
        if (sabotageItemPanel != null)
        {
            sabotageItemPanel.SetActive(!sabotageItemPanel.activeSelf);
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

        // --- Flip character sprite ---
        if (spriteRenderer != null && inputX != 0)
        {
            spriteRenderer.flipX = inputX < 0;
        }
    }

    // ------------------------
    // Animations
    // ------------------------
    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(inputX));
        animator.SetBool("IsGrounded", isGrounded);
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

        if (playerId == 1) player1Dead = true;
        else if (playerId == 2) player2Dead = true;

        if (player1Dead && player2Dead)
        {
            Debug.Log("Both players are dead. Loading GameOver...");
            SceneManager.LoadScene("GameOver");
        }
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
        Debug.Log("Player " + playerId + " pressed interact");

        // --- Already carrying something? Then throw immediately ---
        if (carriedCrate != null)
        {
            Debug.Log("Player " + playerId + " is throwing crate");
            carriedCrate.Throw();
            carriedCrate = null;
            moveSpeed /= carrySpeedMultiplier; // restore speed
            return;
        }

        // --- Otherwise look for interactables ---
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Switch"))
            {
                Debug.Log("Player " + playerId + " interacting with Switch");
                var switchComponent = hit.GetComponent<Switch>();
                if (switchComponent != null)
                {
                    switchComponent.Toggle();
                    return;
                }
            }
            else if (hit.CompareTag("PuzzleNode"))
            {
                Debug.Log("Player " + playerId + " interacting with PuzzleNode");
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
                Debug.Log("Player " + playerId + " interacting with Reactor");
                LevelManager.Instance.TryFixReactor(playerId);
                return;
            }
            else if (hit.CompareTag("Crate"))
            {
                var crate = hit.GetComponent<PushableCrate>();
                if (crate != null)
                {
                    Debug.Log("Player " + playerId + " picked up crate");
                    crate.PickUp(transform);
                    carriedCrate = crate;
                    moveSpeed *= carrySpeedMultiplier;
                    return;
                }
            }
        }

        Debug.Log("Player " + playerId + " interact found nothing in range");
    }

    // ------------------------
    // Reset Position
    // ------------------------
    private void ResetPosition()
    {
        Debug.Log("Player " + playerId + " reset to start position");
        transform.position = startPosition;
        rb.velocity = Vector2.zero; // stop any momentum
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
