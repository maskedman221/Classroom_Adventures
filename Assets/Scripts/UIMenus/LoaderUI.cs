using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Image progressFill;

    public void SetProgress(float progress)
    {
        if (progressFill != null)
            progressFill.fillAmount = Mathf.Clamp01(progress);
    }
}