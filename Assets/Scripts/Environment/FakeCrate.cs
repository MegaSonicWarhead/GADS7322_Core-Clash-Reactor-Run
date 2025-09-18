using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCrate : MonoBehaviour
{
    private bool armed = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (armed && collision.CompareTag("Player"))
        {
            Debug.Log("Boom! Fake crate exploded.");
            Destroy(gameObject);
        }
    }

    // Fake crates still press plates but after a delay they self-destruct
    private void Start()
    {
        Invoke("Explode", 20f);
    }

    private void Explode()
    {
        Debug.Log("Fake crate decayed.");
        Destroy(gameObject);
    }
}
