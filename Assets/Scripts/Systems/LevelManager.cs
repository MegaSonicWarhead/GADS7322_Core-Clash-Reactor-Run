using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject pausePanel; // 👈 drag your PausePanel here

    [Header("Player Rod UI")]
    public Image p1GreenRodImage;
    public Image p1RedRodImage;
    public Image p1BlueRodImage;

    public Image p2GreenRodImage;
    public Image p2RedRodImage;
    public Image p2BlueRodImage;

    [Header("UI Feedback")]
    [SerializeField] private TMPro.TextMeshProUGUI reactorMessageP1; // Player 1
    [SerializeField] private TMPro.TextMeshProUGUI reactorMessageP2; // Player 2
    [SerializeField] private float messageDuration = 2f; // seconds

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        timer = timeLimit;
        UpdateTimerUI();

        if (pausePanel != null)
            pausePanel.SetActive(false);


        if (reactorMessageP1 != null)
            reactorMessageP1.gameObject.SetActive(false);

        if (reactorMessageP2 != null)
            reactorMessageP2.gameObject.SetActive(false);

        // Hide rods at the beginning
        p1GreenRodImage.gameObject.SetActive(false);
        p1RedRodImage.gameObject.SetActive(false);
        p1BlueRodImage.gameObject.SetActive(false);

        p2GreenRodImage.gameObject.SetActive(false);
        p2RedRodImage.gameObject.SetActive(false);
        p2BlueRodImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        // -----------------------
        // Handle Pause Toggle
        // -----------------------
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        if (!gameActive || isPaused) return;

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

    // -----------------------
    // Pause / Resume
    // -----------------------
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // pause physics + updates
            if (pausePanel != null) pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; // resume
            if (pausePanel != null) pausePanel.SetActive(false);
        }
    }

    // -----------------------
    // Nodes + Win Logic
    // -----------------------
    public void RegisterNodeComplete(int playerId, string rodColor)
    {
        if (playerId == 1)
        {
            switch (rodColor)
            {
                case "Green": if (p1GreenRodImage != null) p1GreenRodImage.enabled = true; break;
                case "Red": if (p1RedRodImage != null) p1RedRodImage.enabled = true; break;
                case "Blue": if (p1BlueRodImage != null) p1BlueRodImage.enabled = true; break;
            }
            player1Nodes++;
        }
        else if (playerId == 2)
        {
            switch (rodColor)
            {
                case "Green": if (p2GreenRodImage != null) p2GreenRodImage.enabled = true; break;
                case "Red": if (p2RedRodImage != null) p2RedRodImage.enabled = true; break;
                case "Blue": if (p2BlueRodImage != null) p2BlueRodImage.enabled = true; break;
            }
            player2Nodes++;
        }
    }

    private Coroutine messageCoroutineP1;
    private Coroutine messageCoroutineP2;

    public void ShowReactorMessage(int playerId, string message)
    {
        if (playerId == 1 && reactorMessageP1 != null)
        {
            if (messageCoroutineP1 != null) StopCoroutine(messageCoroutineP1);
            messageCoroutineP1 = StartCoroutine(ShowMessageRoutine(reactorMessageP1, message));
        }
        else if (playerId == 2 && reactorMessageP2 != null)
        {
            if (messageCoroutineP2 != null) StopCoroutine(messageCoroutineP2);
            messageCoroutineP2 = StartCoroutine(ShowMessageRoutine(reactorMessageP2, message));
        }
    }

    private IEnumerator ShowMessageRoutine(TMPro.TextMeshProUGUI textField, string message)
    {
        textField.text = message;
        textField.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        textField.gameObject.SetActive(false);
    }

    public void RegisterNodeIncomplete(int playerId, string rodColor)
    {
        if (playerId == 1)
        {
            switch (rodColor)
            {
                case "Green": if (p1GreenRodImage != null) p1GreenRodImage.enabled = false; break;
                case "Red": if (p1RedRodImage != null) p1RedRodImage.enabled = false; break;
                case "Blue": if (p1BlueRodImage != null) p1BlueRodImage.enabled = false; break;
            }
            if (player1Nodes > 0) player1Nodes--;
        }
        else if (playerId == 2)
        {
            switch (rodColor)
            {
                case "Green": if (p2GreenRodImage != null) p2GreenRodImage.enabled = false; break;
                case "Red": if (p2RedRodImage != null) p2RedRodImage.enabled = false; break;
                case "Blue": if (p2BlueRodImage != null) p2BlueRodImage.enabled = false; break;
            }
            if (player2Nodes > 0) player2Nodes--;
        }
    }

    public void UnlockRod(int playerId, string rodColor)
    {
        if (playerId == 1)
        {
            if (rodColor == "Green") p1GreenRodImage.gameObject.SetActive(true);
            else if (rodColor == "Red") p1RedRodImage.gameObject.SetActive(true);
            else if (rodColor == "Blue") p1BlueRodImage.gameObject.SetActive(true);
        }
        else if (playerId == 2)
        {
            if (rodColor == "Green") p2GreenRodImage.gameObject.SetActive(true);
            else if (rodColor == "Red") p2RedRodImage.gameObject.SetActive(true);
            else if (rodColor == "Blue") p2BlueRodImage.gameObject.SetActive(true);
        }
    }

    public void TryFixReactor(int playerId)
    {
        if (!gameActive) return;

        if (playerId == 1)
        {
            if (player1Nodes >= totalNodesPlayer1)
            {
                Debug.Log("Player 1 Wins!");
                PlayerWin("WinScreenP1");
            }
            else
            {
                Debug.Log("Player 1 does not have all rods to fix the reactor!");
                ShowReactorMessage(1, "You don't have all rods to fix the reactor!");
            }
        }
        else if (playerId == 2)
        {
            if (player2Nodes >= totalNodesPlayer2)
            {
                Debug.Log("Player 2 Wins!");
                PlayerWin("WinScreenP2");
            }
            else
            {
                Debug.Log("Player 2 does not have all rods to fix the reactor!");
                ShowReactorMessage(2, "You don't have all rods to fix the reactor!");
            }
        }
    }

    public static int winnerId = 0; // 0 = none, 1 = player1, 2 = player2

    private void PlayerWin(string sceneName)
    {
        gameActive = false;
        winnerId = (sceneName == "WinScreenP1") ? 1 : 2;
        SceneManager.LoadScene("WinScene");
    }

    private void GameOver()
    {
        gameActive = false;
        SceneManager.LoadScene("GameOver");
    }
}
