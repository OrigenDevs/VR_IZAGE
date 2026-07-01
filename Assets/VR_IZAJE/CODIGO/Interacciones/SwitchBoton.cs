using UnityEngine;

public class SwitchBoton : MonoBehaviour
{
    public Button3D boton;
    public GameObject[] objetosModoInterior;
    public GameObject[] objetosModoExterior;
    public bool modoInteriorActivo;

    void OnEnable()
    {
        if (boton == null) boton = GetComponent<Button3D>();
        if (boton != null) boton.onClick.AddListener(Cambiar);
    }

    void OnDisable()
    {
        if (boton != null) boton.onClick.RemoveListener(Cambiar);
    }

    void Cambiar()
    {
        modoInteriorActivo = !modoInteriorActivo;

        var activar = modoInteriorActivo ? objetosModoInterior : objetosModoExterior;
        var desactivar = modoInteriorActivo ? objetosModoExterior : objetosModoInterior;

        foreach (var obj in activar)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in desactivar)
            if (obj != null) obj.SetActive(false);
    }
}
