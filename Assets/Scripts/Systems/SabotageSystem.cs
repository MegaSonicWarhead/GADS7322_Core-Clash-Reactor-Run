using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;

public class SabotageSystem : MonoBehaviour
{
    public static SabotageSystem Instance;

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
        if (!target) return;

        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = target.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
            GameObject demonObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            Demon demon = demonObj.GetComponent<Demon>();
            if (demon != null)
            {
                demon.target = target;
            }
        }
    }

    private IEnumerator BlackoutRoutine(int targetPlayer)
    {
        GameObject panel = targetPlayer == 1 ? blackoutPanelP1 : blackoutPanelP2;

        if (panel != null)
        {
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
        // Both players share the tag "Player", so find by playerId
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
