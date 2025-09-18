using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Door : PuzzleElement
{
    private BoxCollider2D col;
    private SpriteRenderer sr;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        Close(); // start closed
        UnityEngine.Debug.Log($"Door Awake: {name}");
    }

    public override void Activate()
    {
        // Open door
        col.enabled = false;   // disable collision
        sr.enabled = false;    // hide door sprite
        UnityEngine.Debug.Log($"{name} opened!");
    }

    public override void Deactivate()
    {
        // Close door
        col.enabled = true;    // enable collision
        sr.enabled = true;     // show door sprite
        UnityEngine.Debug.Log($"{name} closed!");
    }

    private void Close()
    {
        col.enabled = true;
        sr.enabled = true;
    }
}
