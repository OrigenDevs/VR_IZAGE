using UnityEngine;
using TMPro;
using DG.Tweening;

public class VRSubtitleManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Header("Transition Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.4f;

    [Header("Duration Settings")]
    [Tooltip("Tiempo exacto en segundos que el texto se quedará estático en pantalla.")]
    [SerializeField] private float textDuration = 3.0f;

    private Sequence subtitleSequence;

    private void Start()
    {
        if (subtitleText != null)
        {
            subtitleText.alpha = 0f;
            subtitleText.text = "";
        }
    }

    public void DisplaySubtitleText(string textToDisplay)
    {
        if (subtitleText == null || string.IsNullOrEmpty(textToDisplay)) return;

        if (subtitleSequence != null && subtitleSequence.IsActive())
        {
            subtitleSequence.Kill();
        }

        // Formatear párrafos con los puntos
        string formattedText = textToDisplay.Replace(". ", ".\n");
        formattedText = formattedText.Replace(".", ".\n");

        if (formattedText.EndsWith(".\n"))
        {
            formattedText = formattedText.Substring(0, formattedText.Length - 1);
        }

        subtitleText.text = formattedText;

        subtitleSequence = DOTween.Sequence();

        subtitleSequence
            // 1. Fade In
            .Append(subtitleText.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad))

            // 2. Tiempo fijo en segundos (Configurable desde el Inspector)
            .AppendInterval(textDuration)

            // 3. Fade Out
            .Append(subtitleText.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad))

            // 4. Limpiar
            .OnComplete(() => subtitleText.text = "");
    }
}