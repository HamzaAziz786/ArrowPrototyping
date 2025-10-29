using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image fillImage;

    [Header("Loading Settings")]
    [SerializeField] private float fillDuration = 2f;  // time to fill completely
    [SerializeField] private Ease fillEase = Ease.OutCubic;

    private bool isLoading = false;

    private void Start()
    {
        // Optional: Start loading automatically on scene start
        StartLoading();
    }

    /// <summary>
    /// Starts the loading animation and hides the panel afterward.
    /// </summary>
    public void StartLoading(System.Action onComplete = null)
    {
        if (isLoading) return;

        isLoading = true;
        loadingPanel.SetActive(true);
        fillImage.fillAmount = 0f;

        // Animate fill amount from 0 → 1
        DOTween.To(
            () => fillImage.fillAmount,
            x => fillImage.fillAmount = x,
            1f,
            fillDuration
        )
        .SetEase(fillEase)
        .OnComplete(() =>
        {
            // Hide loading panel after fill completes
            loadingPanel.SetActive(false);
            isLoading = false;
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// Instantly hides the loading panel.
    /// </summary>
    public void HideInstant()
    {
        DOTween.Kill(fillImage);
        fillImage.fillAmount = 1f;
        loadingPanel.SetActive(false);
        isLoading = false;
    }
}
