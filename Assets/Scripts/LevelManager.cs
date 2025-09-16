using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Puzzle Node Tracking")]
    private int player1Nodes = 0;
    private int player2Nodes = 0;

    [Header("Node Requirements")]
    public int totalNodesPlayer1 = 3;
    public int totalNodesPlayer2 = 3;

    [Header("Timer Settings")]
    public float timeLimit = 60f; // seconds
    private float timer;
    private bool gameActive = true;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText; // 👈 drag your TMP Text here in the inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        timer = timeLimit;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!gameActive) return;

        // countdown
        timer -= Time.deltaTime;
        if (timer < 0f) timer = 0f;

        UpdateTimerUI();

        if (timer <= 0f)
        {
            Debug.Log("Time is up! Both players lose.");
            GameOver();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    // Called when a player collects a node
    public void RegisterNodeComplete(int playerId)
    {
        if (playerId == 1) player1Nodes++;
        else if (playerId == 2) player2Nodes++;
    }

    // Called if a node gets uncollected or reset
    public void RegisterNodeIncomplete(int playerId)
    {
        if (playerId == 1 && player1Nodes > 0) player1Nodes--;
        else if (playerId == 2 && player2Nodes > 0) player2Nodes--;
    }

    // Called when a player presses E at the reactor
    public void TryFixReactor(int playerId)
    {
        if (!gameActive) return;

        if (playerId == 1 && player1Nodes >= totalNodesPlayer1)
        {
            Debug.Log("Player 1 Wins!");
            PlayerWin("WinScreenP1");
        }
        else if (playerId == 2 && player2Nodes >= totalNodesPlayer2)
        {
            Debug.Log("Player 2 Wins!");
            PlayerWin("WinScreenP2");
        }
        else
        {
            Debug.Log("Player " + playerId + " tried to fix reactor without all nodes.");
        }
    }

    public static int winnerId = 0; // 0 = none, 1 = player1, 2 = player2

    private void PlayerWin(string sceneName)
    {
        gameActive = false;

        // Store the winner before loading the win scene
        winnerId = (sceneName == "WinScreenP1") ? 1 : 2;

        SceneManager.LoadScene("WinScene");
    }

    private void GameOver()
    {
        gameActive = false;
        SceneManager.LoadScene("GameOver");
    }
}
