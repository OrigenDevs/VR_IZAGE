using UnityEngine;

public class SistemaAutopartes : MonoBehaviour
{
    public GameObject parteSeleccionada;
    public Transform padreInicial;
    public Transform padreObservacion;
    public GameObject[] objetosModoInicial;
    public GameObject[] objetosModoObservacion;
    public float velocidad = 4f;

    private bool modoInteriorActivo;
    private Coroutine rutinaMovimiento;

    void Start()
    {
        if (parteSeleccionada != null && padreInicial != null)
            parteSeleccionada.transform.SetParent(padreInicial, true);
        AplicarObjetos(false);
    }

    public void Cambiar()
    {
        modoInteriorActivo = !modoInteriorActivo;

        if (rutinaMovimiento != null)
            StopCoroutine(rutinaMovimiento);

        AplicarObjetos(modoInteriorActivo);

        if (parteSeleccionada != null)
        {
            var padre = modoInteriorActivo ? padreObservacion : padreInicial;
            if (padre != null)
            {
                parteSeleccionada.transform.SetParent(padre, true);
                rutinaMovimiento = StartCoroutine(MoverALocalCero());
            }
        }
    }

    public void Devolverse()
    {
        if (!modoInteriorActivo) return;
        modoInteriorActivo = false;

        if (rutinaMovimiento != null)
            StopCoroutine(rutinaMovimiento);

        AplicarObjetos(false);

        if (parteSeleccionada != null && padreInicial != null)
        {
            parteSeleccionada.transform.SetParent(padreInicial, true);
            rutinaMovimiento = StartCoroutine(MoverALocalCero());
        }
    }

    void AplicarObjetos(bool observacion)
    {
        var activar = observacion ? objetosModoObservacion : objetosModoInicial;
        var desactivar = observacion ? objetosModoInicial : objetosModoObservacion;

        foreach (var obj in desactivar)
            if (obj != null) obj.SetActive(false);

        foreach (var obj in activar)
            if (obj != null) obj.SetActive(true);
    }

    System.Collections.IEnumerator MoverALocalCero()
    {
        var t = parteSeleccionada.transform;
        Vector3 posInicio = t.localPosition;
        Quaternion rotInicio = t.localRotation;

        float progreso = 0f;
        while (progreso < 1f)
        {
            progreso += Time.deltaTime * velocidad;
            t.localPosition = Vector3.Lerp(posInicio, Vector3.zero, progreso);
            t.localRotation = Quaternion.Slerp(rotInicio, Quaternion.identity, progreso);
            yield return null;
        }

        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }
}
