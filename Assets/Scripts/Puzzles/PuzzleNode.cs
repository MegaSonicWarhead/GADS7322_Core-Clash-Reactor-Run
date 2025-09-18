using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleNode : PuzzleElement
{
    private bool isPowered = false;
    private int collectedBy = 0;
    private SpriteRenderer sr;

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
            Debug.Log($"Puzzle Node activated by Player {collectedBy}");

            if (sr != null) sr.enabled = false; // hide node visually

            LevelManager.Instance.RegisterNodeComplete(collectedBy);
        }
    }

    public override void Deactivate()
    {
        if (isPowered && collectedBy != 0)
        {
            isPowered = false;
            Debug.Log($"Puzzle Node deactivated by Player {collectedBy}");

            if (sr != null) sr.enabled = true; // show node again

            LevelManager.Instance.RegisterNodeIncomplete(collectedBy);
            collectedBy = 0;
        }
    }

    public void SetCollectedBy(int playerId)
    {
        collectedBy = playerId;
    }
}
