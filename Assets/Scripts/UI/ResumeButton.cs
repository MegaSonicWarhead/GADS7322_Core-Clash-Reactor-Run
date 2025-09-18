using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public GameObject pausePanel; // assign the PausePanel in the Inspector

    public void ResumeGame()
    {
        Time.timeScale = 1f; // unpause
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
}
