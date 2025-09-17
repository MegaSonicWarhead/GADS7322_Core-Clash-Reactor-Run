using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float lifeTime = 5f;
    public int damage = 10; // damage applied on hit

    private float timer;

    private void Start()
    {
        timer = lifeTime;
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }

        timer -= Time.deltaTime;
        if (timer <= 0) Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Check if collided object has a PlayerController
        PlayerController player = col.gameObject.GetComponent<PlayerController>();

        if (player != null && col.transform == target)
        {
            player.TakeDamage(damage);
            Debug.Log($"{target.name} took {damage} damage from a Demon!");
            Destroy(gameObject); // demon disappears after hitting
        }
    }
}
