using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float lifeTime = 5f;

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
        if (col.transform == target)
        {
            Debug.Log($"{target.name} is being attacked by an enemy!");
        }
    }
}
