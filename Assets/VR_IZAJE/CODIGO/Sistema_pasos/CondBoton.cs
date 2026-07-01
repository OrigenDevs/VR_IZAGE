using UnityEngine;

public class CondBoton : MonoBehaviour
{
    public DialogPlayer dialogPlayer;
    public Button3D boton;

    void OnEnable()
    {
        if (boton == null)
            boton = GetComponent<Button3D>();
        if (boton != null)
            boton.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        if (boton != null)
            boton.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        gameObject.SetActive(false);
        if (dialogPlayer != null)
            dialogPlayer.Avanzar();
    }
}
