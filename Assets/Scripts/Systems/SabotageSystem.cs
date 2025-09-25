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
    public int enemyCost = 4;

    [Header("Audio")]
    public AudioSource blackoutSource;
    public AudioClip blackoutClip;

    private int player1Coins = 0;
    private int player2Coins = 0;

    // Prevents multiple triggers in the same frame
    private bool canTrigger = true;

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
            case 0: // Spider web
                if (!SpendCoins(playerId, spiderWebCost))
                {
                    Debug.Log($"Player {playerId} failed sabotage {sabotageIndex} (not enough coins).");
                    return;
                }
                StartCoroutine(HandleSabotage(() => SpawnSpiderWeb(targetPlayer)));
                break;

            case 1: // Missile
                if (!SpendCoins(playerId, missileCost))
                {
                    Debug.Log($"Player {playerId} failed sabotage {sabotageIndex} (not enough coins).");
                    return;
                }
                StartCoroutine(HandleSabotage(() => SpawnMissile(targetPlayer)));
                break;

            case 2: // Blackout
                if (!SpendCoins(playerId, blackoutCost))
                {
                    Debug.Log($"Player {playerId} failed sabotage {sabotageIndex} (not enough coins).");
                    return;
                }
                StartCoroutine(HandleSabotage(() => StartCoroutine(BlackoutRoutine(targetPlayer))));
                break;

            case 3: // Enemy
                if (!SpendCoins(playerId, enemyCost))
                {
                    Debug.Log($"Player {playerId} failed sabotage {sabotageIndex} (not enough coins).");
                    return;
                }
                StartCoroutine(HandleSabotage(() => SpawnEnemies(targetPlayer)));
                break;

            case 4:
                Debug.Log($"Reserved sabotage slot for Player {playerId}");
                break;

            default:
                Debug.LogWarning("Invalid sabotage index: " + sabotageIndex);
                break;
        }
    }

    private IEnumerator HandleSabotage(System.Action sabotageAction)
    {
        if (!canTrigger) yield break;
        canTrigger = false;

        sabotageAction?.Invoke();

        yield return new WaitForSeconds(0.2f); // small cooldown
        canTrigger = true;
    }

    private IEnumerator ResetTrigger()
    {
        yield return null; // wait one frame
        canTrigger = true;
    }

    // --- Prefab Spawners ---
    private void SpawnSpiderWeb(int targetPlayer)
    {
        Transform target = GetPlayerTransform(targetPlayer);
        if (!target) return;

        GameObject webObj = Instantiate(spiderWebPrefab, target.position, Quaternion.identity);
        SpiderWeb web = webObj.GetComponent<SpiderWeb>();
        if (web != null)
            web.Initialize(target);
    }

    private void SpawnMissile(int targetPlayer)
    {
        Transform target = GetPlayerTransform(targetPlayer);
        if (!target) return;

        Debug.Log(">>> MISSILE SPAWNED by sabotage! <<<");

        Vector3 spawnPos = target.position + Vector3.up * 8f;
        GameObject missileObj = Instantiate(missilePrefab, spawnPos, Quaternion.identity);
        Missile missile = missileObj.GetComponent<Missile>();
        if (missile != null)
        {
            missile.target = target;
            missile.spawnedBySabotage = true; // ✅ mark as legit
        }
    }

    private void SpawnEnemies(int targetPlayer)
    {
        Transform target = GetPlayerTransform(targetPlayer);
        if (!target || enemyPrefab == null || levelTilemap == null) return;

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

        Demon demon = demonObj.GetComponent<Demon>();
        if (demon != null)
            demon.target = target;
    }

    private IEnumerator BlackoutRoutine(int targetPlayer)
    {
        GameObject panel = targetPlayer == 1 ? blackoutPanelP1 : blackoutPanelP2;

        if (panel != null)
        {
            if (blackoutSource != null && blackoutClip != null)
                blackoutSource.PlayOneShot(blackoutClip);

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
            Debug.Log($"Player 1 spent {cost} coins. Remaining: {player1Coins}");
            return true;
        }
        else if (playerId == 2 && player2Coins >= cost)
        {
            player2Coins -= cost;
            UpdateUI();
            Debug.Log($"Player 2 spent {cost} coins. Remaining: {player2Coins}");
            return true;
        }

        Debug.Log($"Player {playerId} does NOT have enough coins to spend {cost}!");
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
}
