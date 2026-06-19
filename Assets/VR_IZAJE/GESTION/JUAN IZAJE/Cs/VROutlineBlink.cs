using UnityEngine;
using DG.Tweening;

public class VREyeBlinkCurtain : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("La mitad negra superior (Párpado de arriba).")]
    [SerializeField] private RectTransform upperLid;

    [Tooltip("La mitad negra inferior (Párpado de abajo).")]
    [SerializeField] private RectTransform lowerLid;

    [Header("Blink Settings")]
    [SerializeField] private float closeDuration = 0.04f;
    [SerializeField] private float openDuration = 0.07f;

    [Header("Position Settings")]
    [Tooltip("Posición Y del párpado superior cuando el ojo está abierto (fuera de pantalla).")]
    [SerializeField] private float upperOpenY = 300f;

    [Tooltip("Posición Y del párpado inferior cuando el ojo está abierto (fuera de pantalla).")]
    [SerializeField] private float lowerOpenY = -300f;

    private Sequence blinkSequence;

    private void Start()
    {
        // Inicializar con los ojos abiertos
        ResetLids();
    }

    private void ResetLids()
    {
        if (upperLid != null) upperLid.anchoredPosition = new Vector2(0f, upperOpenY);
        if (lowerLid != null) lowerLid.anchoredPosition = new Vector2(0f, lowerOpenY);
    }

    // Esta función la sigue llamando el Signal Emitter desde el Timeline
    public void TriggerBlink(float secondsClosed)
    {
        if (upperLid == null || lowerLid == null) return;

        if (blinkSequence != null && blinkSequence.IsActive())
        {
            blinkSequence.Kill();
        }

        blinkSequence = DOTween.Sequence();

        blinkSequence
            // 1. Cierre tipo telón: Ambos se encuentran en Y = 0 de golpe
            .Append(upperLid.DOAnchorPosY(0f, closeDuration).SetEase(Ease.OutQuad))
            .Join(lowerLid.DOAnchorPosY(0f, closeDuration).SetEase(Ease.OutQuad))

            // 2. Mantener cerrado (Pantalla totalmente negra el tiempo del Timeline)
            .AppendInterval(secondsClosed)

            // 3. Apertura rápida: Regresan a sus posiciones ocultas fuera de la pantalla
            .Append(upperLid.DOAnchorPosY(upperOpenY, openDuration).SetEase(Ease.InQuad))
            .Join(lowerLid.DOAnchorPosY(lowerOpenY, openDuration).SetEase(Ease.InQuad));
    }
}