using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : PuzzleElement
{
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private PuzzleConnector connector;
    [SerializeField] private WireColor wireColor = WireColor.Red;

    private bool isActive;
    private Color defaultColor;

    private void Start()
    {
        defaultColor = indicator.color;
    }

    // Remove Update() completely. Interaction is handled by PlayerController

    public void Toggle()
    {
        isActive = !isActive;
        indicator.color = isActive ? activeColor : defaultColor;
        connector.SendSignal(isActive, wireColor);
    }

    public override void Activate()
    {
        isActive = true;
        indicator.color = activeColor;
        connector.SendSignal(true, wireColor);
    }

    public override void Deactivate()
    {
        isActive = false;
        indicator.color = defaultColor;
        connector.SendSignal(false, wireColor);
    }
}
