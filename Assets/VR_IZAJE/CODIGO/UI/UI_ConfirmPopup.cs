using UnityEngine;
using UnityEngine.UI;

public class UI_ConfirmPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public Text messageText;
    public Button confirmButton;

    private System.Action onConfirm;

    void Start()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClick);
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void Show(string message, System.Action callback)
    {
        onConfirm = callback;
        if (messageText != null)
            messageText.text = message;
        if (popupPanel != null)
            popupPanel.SetActive(true);
        if (confirmButton != null)
            confirmButton.Select();
    }

    void OnConfirmClick()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
        if (onConfirm != null)
            onConfirm.Invoke();
    }

    void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirmClick);
    }
}
