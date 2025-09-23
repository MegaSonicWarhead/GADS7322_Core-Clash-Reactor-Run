using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleNode : PuzzleElement
{
    private bool isPowered = false;
    private int collectedBy = 0;
    private SpriteRenderer sr;
    public string rodColor; // "Green", "Red", "Blue"

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.LogWarning($"{name} has no SpriteRenderer!");
    }

    // Public read-only property
    public bool IsPowered => isPowered;

    public override void Activate()
    {
        if (!isPowered && collectedBy != 0)
        {
            isPowered = true;

            // Hide the node visually
            if (sr != null) sr.enabled = false;

            // Tell LevelManager that the node is complete
            LevelManager.Instance.RegisterNodeComplete(collectedBy, rodColor);

            // 🔑 Also unlock the rod UI
            LevelManager.Instance.UnlockRod(collectedBy, rodColor);

            Debug.Log($"Puzzle Node ({rodColor}) activated by Player {collectedBy}");
        }
    }

    public override void Deactivate()
    {
        if (isPowered && collectedBy != 0)
        {
            isPowered = false;

            // Show the node again
            if (sr != null) sr.enabled = true;

            // Inform LevelManager that node is incomplete
            LevelManager.Instance.RegisterNodeIncomplete(collectedBy, rodColor);

            // Reset
            collectedBy = 0;

            Debug.Log($"Puzzle Node ({rodColor}) deactivated");
        }
    }

    public void SetCollectedBy(int playerId)
    {
        collectedBy = playerId;
    }
}
