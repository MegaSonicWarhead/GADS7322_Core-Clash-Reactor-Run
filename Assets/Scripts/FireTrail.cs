using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    public Transform target;
    public float followTime = 3f;
    public float damagePerSecond = 10f;

    private float timer;

    private void Start()
    {
        timer = followTime;
    }

    private void Update()
    {
        if (target != null && timer > 0)
        {
            transform.position = new Vector3(target.position.x, target.position.y - 0.5f, target.position.z);
            timer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform == target)
        {
            // Apply damage to player script (replace with your player health system)
            Debug.Log($"{target.name} is standing in fire!");
        }
    }
}
