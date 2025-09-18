using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleElement : MonoBehaviour
{
    protected bool isDisabled = false;

    public abstract void Activate();
    public abstract void Deactivate();

    public virtual void Disable(float duration)
    {
        if (!isDisabled) StartCoroutine(DisableRoutine(duration));
    }

    private System.Collections.IEnumerator DisableRoutine(float duration)
    {
        isDisabled = true;
        yield return new WaitForSeconds(duration);
        isDisabled = false;
    }
}
