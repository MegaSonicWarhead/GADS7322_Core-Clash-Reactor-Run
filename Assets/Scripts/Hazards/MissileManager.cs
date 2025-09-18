using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{
    [Header("References")]
    public GameObject missilePrefab;
    public Transform player1;
    public Transform player2;

    [Header("Spawn Settings")]
    public float spawnHeight = 8f;

    void Update()
    {
        // Player 1 sabotages Player 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnMissile(player2);
        }
        // Player 2 sabotages Player 1
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SpawnMissile(player1);
        }
    }

    void SpawnMissile(Transform target)
    {
        if (target == null || missilePrefab == null) return;

        // Spawn missile above target, facing downward
        Vector3 spawnPos = target.position + Vector3.up * spawnHeight;
        GameObject missile = Instantiate(missilePrefab, spawnPos, Quaternion.identity);

        // Assign target
        missile.GetComponent<Missile>().target = target;
    }
}
