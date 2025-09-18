using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBarrier : PuzzleElement
{
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private PuzzleConnector[] connectors; // assign the 2 switches for THIS player’s barrier

    private bool destroyed = false;

    public override void Activate()
    {
        if (destroyed) return;

        // Check if ALL assigned connectors are active
        bool allActive = true;
        foreach (var connector in connectors)
        {
            if (!connector.IsActive())
            {
                allActive = false;
                break;
            }
        }

        // Only break if all required switches are active
        if (allActive)
        {
            destroyed = true;

            if (breakEffect)
                Instantiate(breakEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    public override void Deactivate()
    {
        // no-op, once broken it stays gone
    }
}
