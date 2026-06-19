using UnityEngine;
using TMPro;

public class UI_StepConfirm3D : MonoBehaviour
{
    public GameObject panel;
    public TextMeshPro text3D;
    public Button3D confirmButton;

    private System.Action onConfirm;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClick);
    }

    public void Show(string message, System.Action callback)
    {
        onConfirm = callback;
        if (text3D != null)
            text3D.text = message;
        if (panel != null)
            panel.SetActive(true);
    }

    void OnConfirmClick()
    {
        if (panel != null)
            panel.SetActive(false);
        if (onConfirm != null)
            onConfirm.Invoke();
    }

    void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirmClick);
    }
}
