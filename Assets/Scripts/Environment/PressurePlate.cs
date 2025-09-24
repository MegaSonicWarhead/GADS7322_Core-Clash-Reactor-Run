using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : PuzzleElement
{
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private Color activeColor = Color.green;
    private Color defaultColor;

    [Header("Target Element")]
    [SerializeField] private PuzzleElement target; // e.g. a Door
    [SerializeField] private PushableCrate crate; // Reference to the PushableCrate script

    [Header("Audio")]
    [SerializeField] private AudioClip activateSound; // Sound played once when activated
    private AudioSource audioSource;

    private void Start()
    {
        defaultColor = indicator.color;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Crate"))
        {
            Activate();
            if (collision.CompareTag("Crate") && crate != null)
            {
                crate.SetPressurePlateStatus(true);  // Notify the crate that the plate is activated
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Crate"))
        {
            Deactivate();
            if (collision.CompareTag("Crate") && crate != null)
            {
                crate.SetPressurePlateStatus(false);  // Notify the crate that the plate is deactivated
            }
        }
    }

    public override void Activate()
    {
        Debug.Log($"{name} PressurePlate Activated!");
        indicator.color = activeColor;

        if (activateSound != null)
        {
            audioSource.PlayOneShot(activateSound);
        }

        if (target != null)
        {
            Debug.Log($"{name} is telling {target.name} to Activate()");
            target.Activate();
        }
        else
        {
            Debug.LogWarning($"{name} has no target assigned!");
        }
    }

    public override void Deactivate()
    {
        Debug.Log($"{name} PressurePlate Deactivated!");
        indicator.color = defaultColor;

        if (target != null)
        {
            Debug.Log($"{name} is telling {target.name} to Deactivate()");
            target.Deactivate();
        }
    }
}
