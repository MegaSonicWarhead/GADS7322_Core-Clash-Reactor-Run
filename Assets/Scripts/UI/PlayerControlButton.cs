using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlButton : MonoBehaviour
{
    [Header("Panels")]
    public GameObject objectivePanel;
    public GameObject playerControlsPanel;

    // Call this method from the Button's OnClick event
    public void ShowPlayerControls()
    {
        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (playerControlsPanel != null)
            playerControlsPanel.SetActive(true);
    }
}
