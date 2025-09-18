using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleConnector : MonoBehaviour
{
    [Header("Connection")]
    public WireColor wireColor = WireColor.Red;
    public PuzzleElement[] targets;

    private bool isActive = false;

    public void SendSignal(bool active, WireColor color)
    {
        if (color != wireColor) return;

        isActive = active;

        foreach (var target in targets)
        {
            if (active) target.Activate();
            else target.Deactivate();
        }
    }

    public bool IsActive()
    {
        return isActive;
    }
}
