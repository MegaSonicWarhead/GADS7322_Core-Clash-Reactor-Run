using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class SabotageSystem : MonoBehaviour
{
    public static SabotageSystem Instance;

    public Tilemap levelTilemap;

    [Header("UI")]
    public TextMeshProUGUI player1CoinsText;
    public TextMeshProUGUI player2CoinsText;
    public GameObject blackoutPanelP1;
    public GameObject blackoutPanelP2;

    [Header("Prefabs")]
    public GameObject spiderWebPrefab;
    public GameObject missilePrefab;
    public GameObject enemyPrefab;

    [Header("Costs")]
    public int spiderWebCost = 1;
    public int missileCost = 2;
    public int blackoutCost = 3;
    public int enemyCost = 2;

    [Header("Audio")]
    public AudioSource blackoutSource;
    public AudioClip blackoutClip;

    private int player1Coins = 0;
    private int player2Coins = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerSabotage(int playerId, int sabotageIndex)
    {
        int targetPlayer = playerId == 1 ? 2 : 1;

        switch (sabotageIndex)
        {
            case 0: if (SpendCoins(playerId, spiderWebCost)) SpawnSpiderWeb(targetPlayer); break;
            case 1: if (SpendCoins(playerId, missileCost)) SpawnMissile(targetPlayer); break;
            case 2: if (SpendCoins(playerId, blackoutCost)) StartCoroutine(BlackoutRoutine(targetPlayer)); break;
            case 3: if (SpendCoins(playerId, enemyCost)) SpawnEnemies(targetPlayer); break;
            case 4: Debug.Log($"Reserved sabotage slot for Player {playerId}"); break;
        }
    }

    // --- Prefab Spawners ---
    private void SpawnSpiderWeb(int targetPlayer)
    {
        Transform target = GetPlayerTransform(targetPlayer);
        if (!target) return;

        GameObject webObj = Instantiate(spiderWebPrefab, target.position, Quaternion.identity);
        SpiderWeb web = webObj.GetComponent<SpiderWeb>();
        if (web != null)
        {
            web.Initialize(target); // Follow the target player
        }
    }

    private void SpawnMissile(int targetPlayer)
    {
        Transform target = GetPlayerTransform(targetPlayer);
        if (!target) return;

        Vector3 spawnPos = target.position + Vector3.up * 8f;
        GameObject missileObj = Instantiate(missilePrefab, spawnPos, Quaternion.identity);

        Missile missile = missileObj.GetComponent<Missile>();
        if (missile != null)
        {
            missile.target = target;
        }
    }

    private void SpawnEnemies(int targetPlayer)
    {
        Transform target = GetPlayerTransform(targetPlayer);
        if (target == null) return;
        if (enemyPrefab == null || levelTilemap == null) return;

        float spawnOffsetX = 2f;
        float spawnHeight = 10f;

        Vector3 spawnPos = target.position + new Vector3(Random.Range(-spawnOffsetX, spawnOffsetX), spawnHeight, 0f);

        Bounds bounds = levelTilemap.localBounds;
        spawnPos.x = Mathf.Clamp(spawnPos.x, bounds.min.x + 0.5f, bounds.max.x - 0.5f);
        spawnPos.y = Mathf.Clamp(spawnPos.y, bounds.min.y + 0.5f, bounds.max.y - 0.5f);

        Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.25f, LayerMask.GetMask("Ground"));
        if (hit != null)
        {
            for (int i = 0; i < 6; i++)
            {
                spawnPos.y += 0.5f;
                hit = Physics2D.OverlapCircle(spawnPos, 0.25f, LayerMask.GetMask("Ground"));
                if (hit == null) break;
            }
        }

        GameObject demonObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // Assign target
        Demon demon = demonObj.GetComponent<Demon>();
        if (demon != null)
        {
            demon.target = target;

            // Immediately play spawn sound here
            if (demon.spawnSound != null)
            {
                AudioSource src = demon.GetComponent<AudioSource>();
                if (src == null) src = demonObj.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 0f; // 2D
                src.PlayOneShot(demon.spawnSound);
            }
        }
    }

    private IEnumerator BlackoutRoutine(int targetPlayer)
    {
        GameObject panel = targetPlayer == 1 ? blackoutPanelP1 : blackoutPanelP2;

        if (panel != null)
        {
            // Play blackout sound once
            if (blackoutSource != null && blackoutClip != null)
            {
                blackoutSource.PlayOneShot(blackoutClip);
            }

            panel.SetActive(true);
            yield return new WaitForSeconds(3f);
            panel.SetActive(false);
        }
    }

    // --- Utils ---
    private bool SpendCoins(int playerId, int cost)
    {
        if (playerId == 1 && player1Coins >= cost)
        {
            player1Coins -= cost;
            UpdateUI();
            return true;
        }
        else if (playerId == 2 && player2Coins >= cost)
        {
            player2Coins -= cost;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddCoin(int playerId)
    {
        if (playerId == 1) player1Coins++;
        else player2Coins++;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (player1CoinsText) player1CoinsText.text = $"P1 Coins: {player1Coins}";
        if (player2CoinsText) player2CoinsText.text = $"P2 Coins: {player2Coins}";
    }

    private Transform GetPlayerTransform(int id)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null && pc.playerId == id)
                return player.transform;
        }

        return null;
    }

    private Vector3 FindValidSpawnPosition(Vector3 center, float minRadius, float maxRadius, int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            // Pick a random point in an annulus (ring)
            Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            Vector3 candidate = center + new Vector3(offset.x, offset.y, 0);

            // Check if the position is free (not inside walls/floors)
            Collider2D hit = Physics2D.OverlapCircle(candidate, 0.5f, LayerMask.GetMask("Ground"));
            if (hit == null)
            {
                return candidate; // Valid position
            }
        }

        // Couldn�t find valid spot
        return Vector3.zero;
    }
}
