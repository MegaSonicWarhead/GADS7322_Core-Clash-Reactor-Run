using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackout : MonoBehaviour
{
    public CanvasGroup blackoutOverlay;
    public float blackoutDuration = 4f;
    public float interval = 15f;

    private void Start()
    {
        StartCoroutine(BlackoutRoutine());
    }

    private IEnumerator BlackoutRoutine()
    {
        while (true)
        {
            // wait until next blackout cycle
            yield return new WaitForSeconds(interval);

            // Find overlay UI (no namespace now)
            var overlay = FindObjectOfType<BlackoutOverlay>();
            if (overlay != null)
            {
                overlay.ShowWarning();
            }

            // warning period before blackout
            yield return new WaitForSeconds(2f);

            if (overlay != null) overlay.SetBlackout(true);

            yield return new WaitForSeconds(blackoutDuration);

            if (overlay != null) overlay.SetBlackout(false);
        }
    }

    private IEnumerator FadeIn()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            blackoutOverlay.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            blackoutOverlay.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
    }
}
