using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    [Header("Panels")]
    public GameObject objectivePanel;
    public GameObject playerControlsPanel;

    // Call this from the Back button's OnClick event
    public void BackToObjectives()
    {
        if (playerControlsPanel != null)
            playerControlsPanel.SetActive(false);

        if (objectivePanel != null)
            objectivePanel.SetActive(true);
    }
}
