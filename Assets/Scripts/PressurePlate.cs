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

    private void Start()
    {
        defaultColor = indicator.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Crate"))
            Activate();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Crate"))
            Deactivate();
    }

    public override void Activate()
    {
        Debug.Log($"{name} PressurePlate Activated!");
        indicator.color = activeColor;

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
