using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SabotageHitUI : MonoBehaviour
{
    public Text hitText;

    public void ShowHit(string effect)
    {
        hitText.text = $"Sabotage: {effect}!";
        hitText.enabled = true;
        CancelInvoke();
        Invoke("Hide", 2f);
    }

    private void Hide()
    {
        hitText.enabled = false;
    }
}
