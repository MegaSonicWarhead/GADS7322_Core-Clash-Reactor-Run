using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : PuzzleElement
{
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private PuzzleConnector connector;
    [SerializeField] private WireColor wireColor = WireColor.Red;
    [SerializeField] private AudioClip activateSound; // Sound to play once when active

    private bool isActive;
    private Color defaultColor;
    private AudioSource audioSource;

    private void Start()
    {
        defaultColor = indicator.color;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Remove Update() completely. Interaction is handled by PlayerController

    public void Toggle()
    {
        isActive = !isActive;
        indicator.color = isActive ? activeColor : defaultColor;
        connector.SendSignal(isActive, wireColor);

        if (isActive && activateSound != null)
        {
            audioSource.PlayOneShot(activateSound);
        }
    }

    public override void Activate()
    {
        if (!isActive) // Prevents playing sound multiple times if already active
        {
            isActive = true;
            indicator.color = activeColor;
            connector.SendSignal(true, wireColor);

            if (activateSound != null)
            {
                audioSource.PlayOneShot(activateSound);
            }
        }
    }

    public override void Deactivate()
    {
        isActive = false;
        indicator.color = defaultColor;
        connector.SendSignal(false, wireColor);
    }
}
