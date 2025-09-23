using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartGameButton : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad = "Level1";

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnStartGameClicked);
    }

    private void OnStartGameClicked()
    {
        // ✅ Always reset timescale before reloading
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name is empty! Please assign a scene to load.");
        }
    }
}
