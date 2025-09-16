using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreenUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;

    private void Start()
    {
        if (winText == null) return;

        if (LevelManager.winnerId == 1)
        {
            winText.text = "Player 1 Wins!";
        }
        else if (LevelManager.winnerId == 2)
        {
            winText.text = "Player 2 Wins!";
        }
        else
        {
            winText.text = "Draw!";
        }
    }
}
