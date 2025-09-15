using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricArc : MonoBehaviour
{
    public float onTime = 2f;
    public float offTime = 2f;

    private bool active = true;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (active && timer > onTime) ToggleArc(false);
        else if (!active && timer > offTime) ToggleArc(true);
    }

    private void ToggleArc(bool state)
    {
        active = state;
        timer = 0f;
        gameObject.GetComponent<SpriteRenderer>().enabled = state;
        gameObject.GetComponent<Collider2D>().enabled = state;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && collision.CompareTag("Player"))
        {
            // Knock back or damage player
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        }
    }
}
