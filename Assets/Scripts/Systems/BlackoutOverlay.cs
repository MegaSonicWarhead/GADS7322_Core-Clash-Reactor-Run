using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutOverlay : MonoBehaviour
{
    public CanvasGroup blackoutCanvas;
    public Text warningText;

    public void ShowWarning()
    {
        warningText.text = "⚠ Blackout Incoming!";
        warningText.enabled = true;
        Invoke("HideWarning", 2f);
    }

    private void HideWarning()
    {
        warningText.enabled = false;
    }

    public void SetBlackout(bool active)
    {
        blackoutCanvas.alpha = active ? 1f : 0f;
    }
}
